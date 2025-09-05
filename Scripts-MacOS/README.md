# 🍎 macOS Scripts

This folder contains shell scripts for running OrderProcessingSystem on macOS.

## Files:

### `start.sh` - Main Shell Start Script
- Builds all three projects with detailed error reporting
- Starts services in separate Terminal windows using AppleScript
- Comprehensive validation and troubleshooting
- Colored output for better visibility

### `stop.sh` - Main Shell Stop Script  
- Stops all services by port (5270, 5000, 5253)
- Uses `lsof` and `kill` to terminate processes
- **NEW: Automatically closes Terminal windows running services**
- Cleans up any remaining dotnet processes

## How to Use:

### Option 1: From the root directory (Recommended)
```bash
# These launcher scripts will automatically call the MACOS versions
./start.sh    # Starts all services
./stop.sh     # Stops all services
```

### Option 2: Run directly from MACOS folder
```bash
# From the root OrderProcessingSystem directory
./MACOS/start.sh
./MACOS/stop.sh
```

### Option 3: From Terminal in MACOS folder
```bash
cd MACOS
./start.sh
./stop.sh
```

## Requirements:
- macOS 10.14+ (Mojave or later)
- .NET SDK installed
- Terminal app
- Bash shell (default on macOS)

## Services Started:
- 🔐 Authentication Service: http://localhost:5270
- 🌐 API Service: http://localhost:5000  
- 🎨 UI Service: http://localhost:5253

## Features:
- ✅ Builds all projects before starting
- ✅ Opens each service in separate Terminal windows
- ✅ Validates project directories exist
- ✅ Detailed error reporting with troubleshooting steps
- ✅ Sequential startup with proper delays
- ✅ Colored output for easy reading

## Terminal Windows:
The script will open 3 separate Terminal windows, one for each service. To stop a specific service, simply close its Terminal window or press Ctrl+C in that window.

## Troubleshooting:
- If you get permission errors, make sure scripts are executable: `chmod +x *.sh`
- If Terminal windows don't open, check macOS security settings for Terminal automation
- For build errors, check that .NET SDK is properly installed: `dotnet --version`
