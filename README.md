<img src="icon.png"  width="64" align="left" style="margin-right: 20px; border-radius: 10px;">

# ColorPickerTool

ColorPickerTool is a simple, lightweight utility for Windows that allows you to quickly pick any color from your screen and copy its hex code to the clipboard.

This tool is designed as an extension for [xToolsMenu](https://github.com/xCONFLiCTiONx/xToolsMenu).

## Features

- **Full-Screen Capture:** Captures your entire virtual screen (including multi-monitor setups) to pick colors from any application.
- **High-DPI Support:** Optimized for 4K and Retina displays to ensure accurate color picking.
- **Clipboard Integration:** Automatically copies the hex code (e.g., `#FFFFFF`) to your clipboard upon selection.
- **Refinement Dialog:** Opens a standard Windows Color Dialog after picking, allowing you to fine-tune the color and copy the new hex code if updated.
- **Quick Exit:** Press `Esc` to cancel the color picking process.

## Requirements

- **Operating System:** Windows
- **Runtime:** [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

## How to Use

1. Launch `ColorPickerTool.exe`.
2. Your screen will be "frozen" in a capture mode.
3. Use the crosshair cursor to click on the color you want to pick.
4. The hex code is immediately copied to your clipboard.
5. A color dialog will appear. You can further adjust the color here. If you click **OK**, the updated hex code will be copied to your clipboard.

## Integration with xToolsMenu

To use this with [xToolsMenu](https://github.com/xCONFLiCTiONx/xToolsMenu), simply add the `ColorPickerTool.exe` to your tools configuration or place it in the designated extensions folder.

---

Created by [xCONFLiCTiONx](https://github.com/xCONFLiCTiONx)
