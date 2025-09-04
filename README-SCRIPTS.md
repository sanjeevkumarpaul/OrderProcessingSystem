# 🚀 O## 🎯 **Super Simple Usage**

```bash
# Works on ALL platforms (Windows, macOS, Linux):
./launch.sh          # Start all services (auto-detects OS)
./launch.sh start    # Start all services
./launch.sh stop     # Stop all services  
./launch.sh qstart   # Quick start (Unix only)
./launch.sh --help   # Show help

# 🚀 NEW: Short Keys for Speed!
./launch.sh --s      # Start services (short flag)
./launch.sh --x      # Stop services (short flag)
./launch.sh --q      # Quick start (short flag)

# Windows users can also use:
launch.bat --s       # Start with short flag
launch.bat --x       # Stop with short flag
```gSystem - Universal Cross-Platform Launcher

## 🌍 **One Script, All Platforms!** 🎉

This repository uses a **Universal Launcher System** that automatically detects your operating system and runs the appropriate scripts.

## � **Super Simple Usage**

```bash
# Works on ALL platforms (Windows, macOS, Linux):
./launch.sh          # Start all services (auto-detects OS)
./launch.sh start    # Start all services
./launch.sh stop     # Stop all services  
./launch.sh qstart   # Quick start (Unix only)
./launch.sh --help   # Show help

# Windows users can also use:
launch.bat           # Same functionality as launch.sh
```

👉 **That's it! No need to remember different commands for different platforms.**

👉 **See [README-UNIVERSAL.md](README-UNIVERSAL.md) for complete details**

---

## 📁 Clean Folder Structure

```
OrderProcessingSystem/
├── 🌍 UNIVERSAL LAUNCHERS
│   ├── launch.sh                ← Main script (Unix/macOS/Windows Git Bash)
│   ├── launch.bat               ← Main script (Windows)
│   ├── README-SCRIPTS.md        ← This file
│   └── README-UNIVERSAL.md      ← Detailed universal launcher guide
│
├── 🖥️ WINOS/ (Windows Implementation)
│   ├── start.ps1               ← PowerShell start script
│   ├── stop.ps1                ← PowerShell stop script
│   ├── start.bat               ← Batch launcher
│   ├── stop.bat                ← Batch launcher  
│   └── README.md               ← Windows-specific guide
│
└── 🍎 MACOS/ (macOS/Linux Implementation)
    ├── start.sh                ← Shell start script
    ├── stop.sh                 ← Shell stop script
    ├── qstart.sh               ← Quick start (no build)
    └── README.md               ← macOS-specific guide
```
start.bat       # On Windows (double-click or run from cmd)

# Stop all services  
./stop.sh       # On macOS/Linux
stop.bat        # On Windows (double-click or run from cmd)
```

## 📊 Services Started

All scripts start the following services:

| Service | Port | URL | Description |
|---------|------|-----|-------------|
| 🔐 Authentication | 5270 | http://localhost:5270 | JWT token authentication |
| 🌐 API | 5000 | http://localhost:5000 | RESTful API endpoints |
| 🎨 UI | 5253 | http://localhost:5253 | **Main Blazor web app** |

**👉 Visit http://localhost:5253 for the main application**

## 🖥️ Platform-Specific Features

### Windows (WINOS folder):
- ✅ PowerShell scripts with colored output
- ✅ Batch file launchers for easy double-click execution
- ✅ Automatic execution policy bypass
- ✅ Windows-specific process management
- ✅ Port-based service detection and cleanup

### macOS (MACOS folder):
- ✅ Shell scripts with colored Terminal output
- ✅ AppleScript integration for new Terminal windows
- ✅ macOS-specific process management with lsof
- ✅ Comprehensive build validation
- ✅ Sequential service startup with proper delays

## 🔧 Requirements

### All Platforms:
- .NET SDK 6.0 or later
- Git (for cloning the repository)

### Platform-Specific:
- **Windows**: PowerShell, Command Prompt
- **macOS**: Terminal, Bash shell

## 💡 Usage Tips

1. **Always run from the root directory** (OrderProcessingSystem folder)
2. **Use the launcher scripts** in the root (start.sh/start.bat) - they automatically detect your OS
3. **Wait for services to fully start** before testing (15-45 seconds each)
4. **Each service opens in its own window** - close individual windows to stop specific services
5. **Use the stop scripts** for clean shutdown of all services

## 🐛 Troubleshooting

### Build Errors:
```bash
# Check .NET SDK
dotnet --version

# Clean and restore
dotnet clean
dotnet restore
```

### Permission Errors (macOS):
```bash
chmod +x start.sh stop.sh MACOS/*.sh
```

### Execution Policy (Windows):
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Port Conflicts:
If services fail to start, check if ports are in use:
```bash
# macOS/Linux
lsof -i :5000 -i :5253 -i :5270

# Windows  
netstat -an | findstr :5000
netstat -an | findstr :5253
netstat -an | findstr :5270
```

## 📋 Development Workflow

1. **Clone the repository**
2. **Run the start script** for your platform
3. **Visit http://localhost:5253** for the main UI
4. **Develop and test** your changes
5. **Run the stop script** when done

The scripts handle all the building, service coordination, and cleanup automatically!

---

🎉 **Happy coding!** For platform-specific details, check the README.md files in the WINOS and MACOS folders.
