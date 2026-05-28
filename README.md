# 🌍 WordClock Taskbar

A sleek, real-time multi-timezone clock that displays on your Windows 11 taskbar with customizable colors and themes.

[![Latest Release](https://img.shields.io/github/v/release/satriawandicky/WordClockTaskbar?label=latest%20release)](https://github.com/satriawandicky/WordClockTaskbar/releases/latest)
[![Download Windows x64](https://img.shields.io/badge/download-Windows%20x64-blue)](https://github.com/satriawandicky/WordClockTaskbar/releases/download/v1.0.2/WordClockTaskbar-v1.0.2-win-x64.exe)

**Latest download:** [WordClockTaskbar-v1.0.2-win-x64.exe](https://github.com/satriawandicky/WordClockTaskbar/releases/download/v1.0.2/WordClockTaskbar-v1.0.2-win-x64.exe)<br>
**Release page:** [GitHub Releases](https://github.com/satriawandicky/WordClockTaskbar/releases/latest)

## 📋 Table of Contents
- [What is WordClock?](#what-is-wordclock)
- [Key Features](#key-features)
- [Installation](#installation)
- [How to Use](#how-to-use)
- [Customization](#customization)

---

## 🎯 What is WordClock?

**WordClock** is a lightweight taskbar application that displays real-time for **up to 5 different timezones** directly on your Windows 11 taskbar. The application runs in the background (system tray) and does not require opening a large window or terminal.

### Use Cases:
- ✅ Coordinate with teams across different timezones
- ✅ Monitor real-time hours in various countries
- ✅ Check local times without opening other apps
- ✅ Customizable to match your color preferences

**Size:** Frameless, compact bar (~300-488x32px)<br>
**Memory:** Minimal (~30-50 MB)  
**OS:** Windows 11 (tested on latest builds)

---

## ✨ Key Features

### 1. **Multi-Timezone Display**
   - Displays up to **5 timezones simultaneously**
   - **24-hour** format (HH:mm)
   - Shows compact labels and time; GMT offset is available on hover
   - Updates in real-time every second

### 2. **Customizable Timezones**
   - Choose from **all Windows timezone IDs**
   - Edit custom labels (e.g., "US" → "New York")
   - Reorder timezones using ↑↓ buttons
   - Automatically saves configuration to JSON

### 3. **Custom Colors & Themes**
   - Change **background**, **text**, and **label** colors
   - Hex color format (`#FFFFFF`, `#FF0000`, etc.)
   - Default: **White text** on a dark background
   - Real-time preview

### 4. **System Tray Integration**
   - **Tray Icon** for quick access
   - **Context menu:**
     - Position (Left / Center / Right)
     - Settings (customization)
     - Show/Hide Window
     - Start with Windows (auto-launch)
     - Exit

### 5. **Window Management**
   - **Frameless, always-on-top** window
   - Reasserts topmost state when another window or overlay covers it
   - **Drag-to-move** functionality
   - **Hidden from Alt+Tab** (WS_EX_TOOLWINDOW)
   - **Prevents accidental close** (hides instead)

---

## 📥 Installation

### **Option 1: Download & Run (Recommended)**

1. **Download the latest Windows x64 executable:**
   [WordClockTaskbar-v1.0.2-win-x64.exe](https://github.com/satriawandicky/WordClockTaskbar/releases/download/v1.0.2/WordClockTaskbar-v1.0.2-win-x64.exe)

2. **Run (double-click):**
   - The app will run immediately in the system tray
   - No installation or administrator rights required
   - The release build is self-contained, so .NET does not need to be installed separately

3. **(Optional) Move to a permanent location:**
   - Copy `WordClockTaskbar.exe` to your Desktop or preferred folder
   - Double-click anytime to launch

All public downloads are available from the [latest GitHub Release](https://github.com/satriawandicky/WordClockTaskbar/releases/latest).

### **Option 2: Auto-Start with Windows**

1. Run the app once
2. Right-click the tray icon → **"Start with Windows"** ✓
3. The app will automatically run on boot

### **Option 3: Build from Source (Developer)**

```bash
# Clone or open the project folder
cd WordClockTaskbar

# Build
dotnet build

# Run
dotnet run

# Publish (create standalone exe)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

**Requirements:** .NET 8 SDK

---

## 🎮 How to Use

### **Default View**
```
┌─────────────────────────────────────────┐
│ US 14:30   UK 19:30   IN 00:00         │
└─────────────────────────────────────────┘
```
- **Label:** Timezone identifier (US, UK, IN, etc.)
- **Time:** HH:mm (24-hour format)
- **GMT:** Offset from GMT is shown in the hover tooltip

### **Interaction:**

#### **Right-Click → Position**
- **Left:** Position on the left side of the taskbar
- **Center:** Position in the center of the taskbar
- **Right:** Position near the system clock (default)

#### **Right-Click → Settings**
Open customization window:
- ➕ **Add Timezone** - Add a new timezone
- ⬆️ **Move Up** - Change order
- ⬇️ **Move Down** - Change order
- ✕ **Remove** - Delete a timezone
- 💾 **Save** - Save configuration

#### **Right-Click → Show/Hide Window**
- Toggle window visibility
- Remains running in the background

#### **Right-Click → Start with Windows**
- Enable auto-launch on boot
- Toggle on/off as needed

#### **Drag Window**
- Click & drag the clock bar to reposition

---

## 🎨 Customization

### **Settings Window**

#### **1. Timezone Management**
```
┌─ Timezones (max 5) ─────────────────┐
│ Label    | Timezone ID Dropdown | ↑↓✕  │
│ "US"     | Eastern Standard ... | ↑↓✕  │
│ "UK"     | GMT Standard Time   | ↑↓✕  │
│ "IN"     | India Standard Time | ↑↓✕  │
│                    [Add Timezone]      │
└─────────────────────────────────────┘
```

**Timezone ID Examples:**
- `Eastern Standard Time` (US)
- `GMT Standard Time` (UK)
- `India Standard Time` (India)
- `Central European Standard Time` (EU)
- `Tokyo Standard Time` (Japan)
- Etc. (all Windows timezone IDs available)

#### **2. Theme Colors**
```
┌─ Theme Colors ──────────────────────┐
│ Background: #E6202020               │
│ Text Color: #FFFFFF (white)         │
│ Label Color: #FFFFFF (white)        │
└─────────────────────────────────────┘
```

**Hex Color Format:** `#RRGGBB` or `#AARRGGBB`

**Examples:**
| Color | Hex Value |
|-------|-----------|
| White | `#FFFFFF` |
| Black | `#000000` |
| Red   | `#FF0000` |
| Green | `#00FF00` |
| Blue  | `#0000FF` |
| Gold  | `#FFD700` |
| Orange| `#FFA500` |

**Default Config:**
```json
{
  "Timezones": [
    { "Label": "US", "TimezoneId": "Eastern Standard Time", "Order": 0 },
    { "Label": "UK", "TimezoneId": "GMT Standard Time", "Order": 1 },
    { "Label": "IN", "TimezoneId": "India Standard Time", "Order": 2 }
  ],
  "Theme": {
    "BackgroundColor": "#E6202020",
    "TextColor": "#FFFFFF",
    "LabelColor": "#FFFFFF",
    "UseDarkMode": true
  }
}
```

**Config Location:**
```
C:\Users\[YourUsername]\AppData\Roaming\WordClockTaskbar\wordclock-config.json
```

---

## 🔧 Troubleshooting

### **App doesn't appear on the taskbar?**
- Check the system tray (bottom right, ^ button to expand)
- Right-click icon → "Show Window"

### **Timezone doesn't update?**
- Restart the application
- Ensure the timezone ID is valid (check Settings)

### **Color doesn't change?**
- Ensure the hex format is valid (`#RRGGBB`)
- Restart the app after saving settings
- Check: `wordclock-config.json` in AppData

### **App closes automatically?**
- This is normal if you close the window
- The window hides, but the app stays running in the background
- Right-click tray → "Show Window" to display it again

---

## 📱 Screenshot Example

**Clock on taskbar:**
```
[System icons] [WordClock: US 14:30   UK 19:30   IN 00:00] [System clock]
```

**Settings Window:**
- Timezone list with edit/reorder/remove
- Color picker for background, text, label
- Save button to apply changes

---

## 📝 Technical Details

**Tech Stack:**
- **.NET 8 WPF** - UI Framework
- **C# 12** - Language
- **System.Windows.Forms.NotifyIcon** - Tray integration
- **Windows API (P/Invoke)** - Taskbar positioning
- **JSON (System.Text.Json)** - Config persistence

**Architecture:**
- MVVM pattern (Model-View-ViewModel)
- Timezone management via TimeZoneInfo
- Real-time updates via DispatcherTimer (1s interval)
- Theme support via hex color conversion

**File Structure:**
```
WordClockTaskbar/
├── Models/
│   ├── TimezoneClockModel.cs
│   └── TimezoneConfig.cs
├── ViewModels/
│   ├── ClockViewModel.cs
│   └── SettingsViewModel.cs
├── Helpers/
│   ├── TaskbarHelper.cs
│   └── ThemeHelper.cs
├── MainWindow.xaml
├── SettingsWindow.xaml
├── App.xaml
└── Resources/
    └── Styles.xaml
```

---

## 📄 License

Free to use & modify for personal/commercial use.

---

## 🤝 Support

For issues or feature requests:
- Edit the config file directly at `AppData\Roaming\WordClockTaskbar\wordclock-config.json`
- Restart the application to apply changes
- Check event logs if any errors occur

---

**Happy clocking! ⏰**
