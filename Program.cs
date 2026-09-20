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
            // Capture the color under the mouse cursor
            var cursorPos = Cursor.Position;
            using var bmp = new Bitmap(1, 1);
            using (var g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(cursorPos.X, cursorPos.Y, 0, 0, new Size(1, 1));
            }
            Color capturedColor = bmp.GetPixel(0, 0);
            string hex = $"#{capturedColor.R:X2}{capturedColor.G:X2}{capturedColor.B:X2}";

            // Copy captured color to clipboard in HEX format
            Clipboard.SetText(hex);
            Console.WriteLine($"Captured color: {hex} (copied to clipboard)");

            // Open a color picker initialized with the captured color
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
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
                Console.WriteLine($"Selected color: {newHex} (copied to clipboard)");
            }
            else
            {
                Console.WriteLine("Color picker cancelled.");
            }
        }
    }
}
