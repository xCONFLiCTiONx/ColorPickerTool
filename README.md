<img src="icon.png"  width="64" align="left" style="margin-right: 20px; border-radius: 10px;">

# ColorPickerTool

ColorPickerTool is a lightweight system tray utility for Windows that allows you to quickly pick colors from your screen, manage custom colors across sessions, and copy hex codes to the clipboard.

This tool is designed as an extension for [xToolsMenu](https://github.com/xCONFLiCTiONx/xToolsMenu).

## Features

- **System Tray Integration:** Runs quietly in the background as a system tray icon.
- **Single Instance:** Enforced via a `Mutex` to prevent multiple background instances from running simultaneously.
- **Quick Actions:**
  - **Left-click** the tray icon to instantly **Grab Color** from the screen.
  - **Right-click** the tray menu for options: **Grab Color**, **Pick Color**, and **Exit**.
- **Full-Screen Capture:** Captures your entire virtual screen (multi-monitor support) with High-DPI (4K/Retina) scaling.
- **Clipboard Integration & Esc Shortcut:** 
  - Automatically copies the selected hex code (e.g., `#FFFFFF`) to your clipboard.
  - Pressing `Esc` during screen selection uses the color already in your clipboard and opens the color picker pre-loaded with it.
- **Persistent Custom Colors:** Automatically saves and restores your 16 custom color slots (`custom_colors.json`) across sessions.
- **Refinement Dialog:** Opens the standard Windows Color Dialog after picking or picking directly, allowing fine-tuning and custom color additions.

## Requirements

- **Operating System:** Windows
- **Runtime:** [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

## How to Use

1. Launch `ColorPickerTool.exe`. It runs resident in your system tray.
2. **Left-click** the tray icon (or select **Grab Color** from the tray menu) to capture the screen.
3. Click on any pixel with the crosshair cursor, or press `Esc` to use the color currently in your clipboard.
4. The hex code is copied to your clipboard, and the Color Dialog opens pre-loaded with your saved custom colors.
5. Right-click the tray icon to select **Pick Color** or **Exit**.

## Integration with xToolsMenu

To use this with [xToolsMenu](https://github.com/xCONFLiCTiONx/xToolsMenu), simply add `ColorPickerTool.exe` to your tools configuration or place it in the designated extensions folder.

---

Created by [xCONFLiCTiONx](https://github.com/xCONFLiCTiONx)
