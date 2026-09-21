// Program.cs
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ColorPickerTool
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Fixes coordinate mismatch on high-DPI (4K/Retina) screens
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Color capturedColor;
            using (var selector = new SelectionForm())
            {
                if (selector.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                capturedColor = selector.SelectedColor;
            }

            string hex = $"#{capturedColor.R:X2}{capturedColor.G:X2}{capturedColor.B:X2}";
            Clipboard.SetText(hex);

            using var dialog = new ColorDialog
            {
                FullOpen = true,
                AnyColor = true,
                Color = capturedColor
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Color selected = dialog.Color;
                string newHex = $"#{selected.R:X2}{selected.G:X2}{selected.B:X2}";
                Clipboard.SetText(newHex);
            }
        }
    }

    public class SelectionForm : Form
    {
        private readonly Bitmap _screenSnapshot;
        public Color SelectedColor { get; private set; }

        public SelectionForm()
        {
            // 1. Capture screen FIRST to avoid capturing the selection window itself
            Rectangle bounds = SystemInformation.VirtualScreen;
            _screenSnapshot = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(_screenSnapshot))
            {
                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
            }

            // 2. Setup form to be as smooth as possible
            this.DoubleBuffered = true;
            this.BackColor = Color.Black; // Dark base to avoid white flash
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Bounds = bounds;
            this.TopMost = true;
            this.Cursor = Cursors.Cross;
            this.ShowInTaskbar = false;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Skip painting background to avoid the white flicker
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Manually draw the snapshot for maximum control and smoothness
            e.Graphics.DrawImageUnscaled(_screenSnapshot, 0, 0);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            // Extract color from the snapshot at the exact click location
            if (e.X >= 0 && e.X < _screenSnapshot.Width && e.Y >= 0 && e.Y < _screenSnapshot.Height)
            {
                SelectedColor = _screenSnapshot.GetPixel(e.X, e.Y);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _screenSnapshot?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
