# 🌍 WordClock Taskbar

A sleek, real-time multi-timezone clock that displays on your Windows 11 taskbar with customizable colors and themes.

## 📋 Daftar Isi
- [Apa Itu WordClock?](#apa-itu-wordclock)
- [Fitur Utama](#fitur-utama)
- [Instalasi](#instalasi)
- [Cara Menggunakan](#cara-menggunakan)
- [Kustomisasi](#kustomisasi)

---

## 🎯 Apa Itu WordClock?

**WordClock** adalah aplikasi taskbar kecil yang menampilkan waktu real-time untuk **hingga 5 timezone berbeda** langsung di taskbar Windows 11 Anda. Aplikasi ini berjalan di background (system tray) dan tidak memerlukan membuka window besar atau terminal.

### Gunakan Untuk:
- ✅ Koordinasi dengan tim di timezone berbeda
- ✅ Monitor waktu real-time di berbagai negara
- ✅ Cek waktu lokal tanpa buka aplikasi lain
- ✅ Customizable sesuai preferensi warna

**Ukuran:** Frameless, compact bar (~420x32px)  
**Memory:** Minimal (~30-50 MB)  
**OS:** Windows 11 (tested on latest builds)

---

## ✨ Fitur Utama

### 1. **Multi-Timezone Display**
   - Tampil hingga **5 timezone sekaligus**
   - Format **24-jam** (HH:mm)
   - Tampil **GMT offset** (misal: `GMT+2`, `GMT+5:30`, `GMT-5`)
   - Update real-time setiap detik

### 2. **Customizable Timezone**
   - Pilih dari **semua Windows timezone ID**
   - Edit label custom (misal: "US" → "New York")
   - Reorder timezone dengan ↑↓ buttons
   - Simpan config otomatis ke JSON

### 3. **Custom Colors & Theme**
   - Ubah warna **background**, **text**, **label**
   - Format hex color (`#FFFFFF`, `#FF0000`, dll)
   - Default: **Putih text** di dark background
   - Real-time preview

### 4. **System Tray Integration**
   - **Tray Icon** untuk quick access
   - **Context menu:**
     - Position (Left / Center / Right)
     - Settings (customization)
     - Show/Hide Window
     - Start with Windows (auto-launch)
     - Exit

### 5. **Window Management**
   - **Frameless, always-on-top** window
   - **Drag-to-move** functionality
   - **Hide dari Alt+Tab** (WS_EX_TOOLWINDOW)
   - **Prevent accidental close** (hide instead)

---

## 📥 Instalasi

### **Option 1: Download & Run (Recommended)**

1. **Download executable:**
   ```
   C:\Users\[YourUsername]\OneDrive\Documents\Nimbalyst\WordClockTaskbar\bin\Release\net8.0-windows\win-x64\publish\WordClockTaskbar.exe
   ```

2. **Jalankan (double-click):**
   - Aplikasi langsung berjalan di system tray
   - Tidak perlu install atau administrator

3. **(Optional) Copy ke lokasi tetap:**
   - Copy `WordClockTaskbar.exe` ke Desktop atau folder favorit
   - Double-click kapan saja untuk jalankan

### **Option 2: Auto-Start dengan Windows**

1. Jalankan app sekali
2. Right-click tray icon → **"Start with Windows"** ✓
3. App akan otomatis run saat boot

### **Option 3: Build dari Source (Developer)**

```bash
# Clone atau buka folder project
cd WordClockTaskbar

# Build
dotnet build

# Run
dotnet run

# Publish (create standalone exe)
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true
```

**Requirements:** .NET 8 SDK

---

## 🎮 Cara Menggunakan

### **Tampilan Default**
```
┌─────────────────────────────────────────┐
│ US 14:30 GMT-5 | UK 19:30 GMT+0 | IN 00:00 GMT+5:30 │
└─────────────────────────────────────────┘
```
- **Label:** Timezone identifier (US, UK, IN, dll)
- **Time:** Waktu HH:mm (format 24-jam)
- **GMT:** Offset dari GMT (misal: +5:30, -5, +2)

### **Interaksi:**

#### **Right-Click → Position**
- **Left:** Posisi di kiri taskbar
- **Center:** Posisi di tengah taskbar
- **Right:** Posisi di dekat system clock (default)

#### **Right-Click → Settings**
Buka window untuk customize:
- ➕ **Add Timezone** - Tambah timezone baru
- ⬆️ **Move Up** - Ubah urutan
- ⬇️ **Move Down** - Ubah urutan
- ✕ **Remove** - Hapus timezone
- 💾 **Save** - Simpan config

#### **Right-Click → Show/Hide Window**
- Toggle visibility window
- Tetap running di background

#### **Right-Click → Start with Windows**
- Enable auto-launch saat boot
- Toggle on/off sesuai kebutuhan

#### **Drag Window**
- Click & drag clock bar untuk reposition

---

## 🎨 Kustomisasi

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
- Dll (all Windows timezone IDs available)

#### **2. Theme Colors**
```
┌─ Theme Colors ──────────────────────┐
│ Background: #E6202020               │
│ Text Color: #FFFFFF (putih)         │
│ Label Color: #FFFFFF (putih)        │
└─────────────────────────────────────┘
```

**Hex Color Format:** `#RRGGBB` atau `#AARRGGBB`

**Contoh:**
| Warna | Hex Value |
|-------|-----------|
| Putih | `#FFFFFF` |
| Hitam | `#000000` |
| Merah | `#FF0000` |
| Hijau | `#00FF00` |
| Biru | `#0000FF` |
| Emas | `#FFD700` |
| Orange | `#FFA500` |

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

**Lokasi Config:**
```
C:\Users\[YourUsername]\AppData\Roaming\WordClockTaskbar\wordclock-config.json
```

---

## 🔧 Troubleshooting

### **App tidak muncul di taskbar?**
- Cek system tray (kanan bawah, tombol ^ untuk expand)
- Right-click icon → "Show Window"

### **Timezone tidak update?**
- Restart aplikasi
- Pastikan timezone ID valid (lihat Settings)

### **Warna tidak berubah?**
- Pastikan format hex valid (`#RRGGBB`)
- Restart app setelah save settings
- Check: `wordclock-config.json` di AppData

### **App close sendiri?**
- Ini normal jika Anda close window
- Window akan hide, app tetap running di background
- Right-click tray → "Show Window" untuk tampilkan lagi

---

## 📱 Screenshot Contoh

**Clock pada taskbar:**
```
[System icons] [WordClock: US 14:30 GMT-5 | UK 19:30 GMT+0 | IN 00:00 GMT+5:30] [System clock]
```

**Settings Window:**
- Timezone list dengan edit/reorder/remove
- Color picker untuk background, text, label
- Save button untuk apply

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

Untuk masalah atau feature request:
- Edit config file langsung di `AppData\Roaming\WordClockTaskbar\wordclock-config.json`
- Restart aplikasi untuk apply changes
- Check event log jika ada error

---

**Happy clocking! ⏰**
