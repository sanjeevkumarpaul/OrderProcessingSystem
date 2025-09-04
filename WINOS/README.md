# 🖥️ Windows Scripts

This folder contains PowerShell and batch scripts for running OrderProcessingSystem on Windows.

## Files:

### `start.ps1` - Main PowerShell Start Script
- Builds all three projects
- Starts services in separate PowerShell windows
- Comprehensive error handling and validation
- Colored output for better visibility

### `stop.ps1` - Main PowerShell Stop Script
- Stops all services by port (5270, 5000, 5253)
- Cleans up any remaining dotnet processes
- **NEW: Automatically closes PowerShell windows running services**
- Verifies ports are freed after stopping

### `start.bat` - Windows Batch Launcher
- Easy double-click execution
- Bypasses PowerShell execution policy restrictions
- Calls the main PowerShell script

### `stop.bat` - Windows Batch Launcher
- Easy double-click execution for stopping services
- Calls the main PowerShell stop script

## How to Use:

### Option 1: Double-click the .bat files (Easiest)
```
start.bat    ← Double-click to start all services
stop.bat     ← Double-click to stop all services
```

### Option 2: Run from PowerShell
```powershell
# From the root OrderProcessingSystem directory
.\WINOS\start.ps1
.\WINOS\stop.ps1
```

### Option 3: From Command Prompt
```cmd
# From the root OrderProcessingSystem directory
WINOS\start.bat
WINOS\stop.bat
```

## Requirements:
- Windows 10/11 or Windows Server
- .NET SDK installed
- PowerShell (comes with Windows)

## Services Started:
- 🔐 Authentication Service: http://localhost:5270
- 🌐 API Service: http://localhost:5000  
- 🎨 UI Service: http://localhost:5253

## Troubleshooting:
If you get execution policy errors, run this in PowerShell as Administrator:
```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```
