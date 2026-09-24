// Program.cs
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace ColorPickerTool
{
    internal static class Program
    {
        private static Mutex? _mutex;

        [STAThread]
        static void Main()
        {
            // Ensure only one instance is running
            const string mutexName = "ColorPickerTool_SingleInstance_Mutex_F8A9B2C3";
            _mutex = new Mutex(true, mutexName, out bool createdNew);
            if (!createdNew)
            {
                // Already running
                return;
            }

            // Fixes coordinate mismatch on high-DPI (4K/Retina) screens
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new TrayApplicationContext());

            GC.KeepAlive(_mutex);
        }

        public static Color ParseColorFromClipboard()
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string text = Clipboard.GetText().Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        // Try parsing via ColorTranslator
                        try
                        {
                            return ColorTranslator.FromHtml(text);
                        }
                        catch {}

                        if (!text.StartsWith("#"))
                        {
                            try
                            {
                                return ColorTranslator.FromHtml("#" + text);
                            }
                            catch {}
                        }

                        // Try rgb(r, g, b) or comma separated
                        var nums = Regex.Matches(text, @"\d+");
                        if (nums.Count >= 3)
                        {
                            int r = Math.Clamp(int.Parse(nums[0].Value), 0, 255);
                            int g = Math.Clamp(int.Parse(nums[1].Value), 0, 255);
                            int b = Math.Clamp(int.Parse(nums[2].Value), 0, 255);
                            return Color.FromArgb(r, g, b);
                        }
                    }
                }
            }
            catch
            {
                // Clipboard or parsing error fallback
            }
            return Color.White;
        }
    }

    public static class CustomColorsManager
    {
        private static readonly string SettingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "custom_colors.json");

        public static int[] LoadCustomColors()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    string json = File.ReadAllText(SettingsFilePath);
                    int[]? colors = JsonSerializer.Deserialize<int[]>(json);
                    if (colors != null && colors.Length == 16)
                    {
                        return colors;
                    }
                }
            }
            catch
            {
                // Fallback
            }

            // Default: 16 white colors
            int[] defaults = new int[16];
            for (int i = 0; i < 16; i++)
            {
                defaults[i] = ColorTranslator.ToOle(Color.White);
            }
            return defaults;
        }

        public static void SaveCustomColors(int[] colors)
        {
            try
            {
                if (colors != null && colors.Length == 16)
                {
                    string json = JsonSerializer.Serialize(colors);
                    File.WriteAllText(SettingsFilePath, json);
                }
            }
            catch
            {
                // Ignore save errors
            }
        }

        public static void AddColor(Color newColor)
        {
            int[] colors = LoadCustomColors();
            int newOle = ColorTranslator.ToOle(newColor);

            // Shift right: index 15 drops off, 14 moves to 15, ..., 0 moves to 1, newColor goes to index 0.
            for (int i = 15; i > 0; i--)
            {
                colors[i] = colors[i - 1];
            }
            colors[0] = newOle;

            SaveCustomColors(colors);
        }
    }

    public class TrayApplicationContext : ApplicationContext
    {
        private NotifyIcon _trayIcon;
        private ContextMenuStrip _trayMenu;

        public TrayApplicationContext()
        {
            // Initialize Tray Menu
            _trayMenu = new ContextMenuStrip();
            _trayMenu.Items.Add("Grab Color", null, OnGrabColor);
            _trayMenu.Items.Add("Pick Color", null, OnPickColor);
            _trayMenu.Items.Add(new ToolStripSeparator());
            _trayMenu.Items.Add("Exit", null, OnExit);

            // Load icon
            Icon appIcon;
            try
            {
                appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Application;
            }
            catch
            {
                try
                {
                    appIcon = new Icon("icon.ico");
                }
                catch
                {
                    appIcon = SystemIcons.Application;
                }
            }

            // Initialize Tray Icon
            _trayIcon = new NotifyIcon()
            {
                Icon = appIcon,
                ContextMenuStrip = _trayMenu,
                Visible = true,
                Text = "Color Picker Tool"
            };

            // Left click triggers Grab Color
            _trayIcon.MouseClick += (sender, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    OnGrabColor(sender, e);
                }
            };
        }

        private void OnGrabColor(object? sender, EventArgs e)
        {
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

            CustomColorsManager.AddColor(capturedColor);

            using var dialog = new ColorDialog
            {
                FullOpen = true,
                AnyColor = true,
                Color = capturedColor,
                CustomColors = CustomColorsManager.LoadCustomColors()
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Color selected = dialog.Color;
                string newHex = $"#{selected.R:X2}{selected.G:X2}{selected.B:X2}";
                Clipboard.SetText(newHex);
                CustomColorsManager.SaveCustomColors(dialog.CustomColors);
            }
        }

        private void OnPickColor(object? sender, EventArgs e)
        {
            Color initialColor = Program.ParseColorFromClipboard();

            using var dialog = new ColorDialog
            {
                FullOpen = true,
                AnyColor = true,
                Color = initialColor,
                CustomColors = CustomColorsManager.LoadCustomColors()
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Color selected = dialog.Color;
                string newHex = $"#{selected.R:X2}{selected.G:X2}{selected.B:X2}";
                Clipboard.SetText(newHex);
                CustomColorsManager.SaveCustomColors(dialog.CustomColors);
            }
        }

        private void OnExit(object? sender, EventArgs e)
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
            Application.Exit();
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
                SelectedColor = Program.ParseColorFromClipboard();
                this.DialogResult = DialogResult.OK;
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
