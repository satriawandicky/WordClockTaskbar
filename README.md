# 🌍 WordClock Taskbar

A sleek, real-time multi-timezone clock that displays on your Windows 11 taskbar with customizable colors and themes.

[![Latest Release](https://img.shields.io/github/v/release/satriawandicky/WordClockTaskbar?label=latest%20release)](https://github.com/satriawandicky/WordClockTaskbar/releases/latest)
[![Download Setup Installer](https://img.shields.io/badge/download-Setup%20Installer-blue)](https://github.com/satriawandicky/WordClockTaskbar/raw/master/releases/WordClockTaskbar-Setup-v1.1.0.exe)

**Latest download:** [WordClockTaskbar-Setup-v1.1.0.exe](https://github.com/satriawandicky/WordClockTaskbar/raw/master/releases/WordClockTaskbar-Setup-v1.1.0.exe)<br>
**Release page:** [GitHub Releases](https://github.com/satriawandicky/WordClockTaskbar/releases/latest)

## 📋 Table of Contents
- [What is WordClock?](#what-is-wordclock)
- [Key Features](#key-features)
- [Installation](#installation)
- [How to Use](#how-to-use)
- [Customization](#customization)

---

## 🎯 What is WordClock?

**WordClock** is a lightweight taskbar application that displays real-time for **up to 5 different timezones** directly on your Windows 11 taskbar in a clean, stacked 2-row text widget. The application runs in the background (system tray) and does not require opening a large window or terminal.

### Use Cases:
- ✅ Coordinate with teams across different timezones
- ✅ Monitor real-time hours in various countries
- ✅ Check local times without opening other apps
- ✅ Customizable to match your color preferences

**Size:** Frameless, compact bar (~80-280x36px)<br>
**Memory:** Minimal (~30-50 MB)  
**OS:** Windows 11 (tested on latest builds)

---

## ✨ Key Features

### 1. **Multi-Timezone 2-Row Stacked Display**
   - Displays up to **5 timezones simultaneously** in 2 stacked rows (e.g. US top, UK bottom)
   - Clean string-only text (no flags needed)
   - **24-hour** format (HH:mm)
   - Shows compact labels and time; GMT offset is available on hover
   - Updates in real-time every second

### 2. **Windows Search Integration**
   - Setup installer registers Start Menu shortcut automatically
   - Easily search and launch **WordClock Taskbar** directly from Windows Search (Win + S)

### 3. **Customizable Timezones**
   - Choose from **all Windows timezone IDs**
   - Edit custom labels (e.g., "US" → "NY")
   - Reorder timezones using ↑↓ buttons
   - Automatically saves configuration to JSON

### 4. **Custom Colors & Themes**
   - Change **background**, **text**, and **label** colors
   - Hex color format (`#FFFFFF`, `#FF0000`, etc.)
   - Default: **White text** on a dark background
   - Real-time preview

### 5. **System Tray Integration**
   - **Tray Icon** for quick access
   - **Context menu:**
     - Position (Left / Center / Right)
     - Settings (customization)
     - Show/Hide Window
     - Start with Windows (auto-launch)
     - Exit

### 6. **Window Management**
   - **Frameless, always-on-top** window
   - Reasserts topmost state when another window or overlay covers it
   - **Drag-to-move** functionality
   - **Hidden from Alt+Tab** (WS_EX_TOOLWINDOW)
   - **Prevents accidental close** (hides instead)

---

## 📥 Installation

### **Option 1: Setup Installer (Recommended)**

1. **Download the latest Windows x64 installer:**
   [WordClockTaskbar-Setup-v1.1.0.exe](https://github.com/satriawandicky/WordClockTaskbar/raw/master/releases/WordClockTaskbar-Setup-v1.1.0.exe)

2. **Run Installer:**
   - Registers WordClock in Windows Search and Start Menu
   - Option to launch automatically on Windows startup
   - The app runs in the system tray

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

### **Default 2-Row Stacked View**
```
┌───────────────────────────┐
│ US 14:30   IN 00:00       │
│ UK 19:30   JP 03:30       │
└───────────────────────────┘
```
- **Label:** Timezone identifier (US, UK, IN, etc.)
- **Time:** HH:mm (24-hour format)
- **GMT:** Offset from GMT is shown in the hover tooltip
