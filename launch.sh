#!/bin/bash

# Universal Cross-Platform Launcher for OrderProcessingSystem
# File: launch.sh
# 
# This script automatically detects your operating system and runs the appropriate
# platform-specific start script from either WINOS or MACOS folder.

echo "🌍 OrderProcessingSystem - Universal Cross-Platform Launcher"
echo ""

# Function to detect the operating system
detect_os() {
    case "$(uname -s)" in
        Darwin*)
            echo "🍎 Detected: macOS"
            return 0  # macOS
            ;;
        Linux*)
            echo "🐧 Detected: Linux"
            return 1  # Linux (treat as Unix-like)
            ;;
        CYGWIN*|MINGW*|MSYS*)
            echo "🖥️  Detected: Windows (Git Bash/MSYS2)"
            return 2  # Windows via Git Bash
            ;;
        *)
            echo "❓ Unknown OS: $(uname -s)"
            echo "Defaulting to Unix-like behavior..."
            return 1  # Default to Unix-like
            ;;
    esac
}

# Function to run macOS/Linux version
run_unix_version() {
    local action=$1
    echo "📂 Using MACOS folder for Unix-like system..."
    
    if [ ! -f "MACOS/${action}.sh" ]; then
        echo "❌ Error: MACOS/${action}.sh not found!"
        echo "Make sure you're running this from the OrderProcessingSystem root directory."
        exit 1
    fi
    
    echo "▶️  Executing: ./MACOS/${action}.sh"
    chmod +x "MACOS/${action}.sh"
    "./MACOS/${action}.sh"
}

# Function to run Windows version (via Git Bash)
run_windows_version() {
    local action=$1
    echo "📂 Using WINOS folder for Windows system..."
    
    # Check if running in Git Bash/MSYS2 environment
    if command -v powershell.exe >/dev/null 2>&1; then
        echo "🔧 PowerShell detected - using PowerShell script..."
        if [ ! -f "WINOS/${action}.ps1" ]; then
            echo "❌ Error: WINOS/${action}.ps1 not found!"
            exit 1
        fi
        powershell.exe -ExecutionPolicy Bypass -File "WINOS/${action}.ps1"
    elif [ -f "WINOS/${action}.bat" ]; then
        echo "🔧 Using batch file..."
        cmd.exe /c "WINOS\\${action}.bat"
    else
        echo "❌ Error: No suitable Windows script found!"
        echo "Looking for either WINOS/${action}.ps1 or WINOS/${action}.bat"
        exit 1
    fi
}

# Main execution logic
main() {
    # Determine action from script name or parameter
    local script_name=$(basename "$0" .sh)
    local action=""
    
    # Check for short flags first
    case "$1" in
        --s|--start)
            action="start"
            ;;
        --x|--stop)
            action="stop"
            ;;
        --q|--qstart|--quick)
            action="qstart"
            ;;
        start|stop|qstart)
            action="$1"
            ;;
        "")
            # No parameter provided
            case "$script_name" in
                launch)
                    action="start"  # Default action
                    ;;
                start|stop|qstart)
                    action=$script_name
                    ;;
                *)
                    action="start"  # Default fallback
                    ;;
            esac
            ;;
        *)
            # Unknown parameter - show error and help
            echo "❌ Unknown parameter: $1"
            echo ""
            show_help
            exit 1
            ;;
    esac
    
    echo "🎯 Action: $action"
    echo ""
    
    # Validate action
    case "$action" in
        start|stop)
            ;;
        qstart)
            # qstart only works on Unix-like systems
            echo "⚡ Quick start mode (Unix-like only)"
            ;;
        *)
            echo "❌ Invalid action: $action"
            echo "Valid actions: start, stop, qstart"
            exit 1
            ;;
    esac
    
    # Detect OS and run appropriate script
    detect_os
    local os_type=$?
    
    case $os_type in
        0) # macOS
            run_unix_version "$action"
            ;;
        1) # Linux or default Unix-like
            if [ "$action" = "qstart" ]; then
                echo "⚡ Quick start available on this system"
            fi
            run_unix_version "$action"
            ;;
        2) # Windows
            if [ "$action" = "qstart" ]; then
                echo "⚠️  Quick start not available on Windows - using full start"
                action="start"
            fi
            run_windows_version "$action"
            ;;
    esac
}

# Help function
show_help() {
    echo "🚀 OrderProcessingSystem Universal Launcher"
    echo ""
    echo "Usage:"
    echo "  ./launch.sh [action]"
    echo ""
    echo "Actions:"
    echo "  start, --s, --start      - Start all services (default)"
    echo "  stop, --x, --stop        - Stop all services"
    echo "  qstart, --q, --qstart    - Quick start (Unix-like only, no build validation)"
    echo "  --quick                  - Same as qstart"
    echo ""
    echo "Examples:"
    echo "  ./launch.sh              # Start services (default)"
    echo "  ./launch.sh start        # Start services"  
    echo "  ./launch.sh --s          # Start services (short flag)"
    echo "  ./launch.sh stop         # Stop services"
    echo "  ./launch.sh --x          # Stop services (short flag)"
    echo "  ./launch.sh qstart       # Quick start (macOS/Linux only)"
    echo "  ./launch.sh --q          # Quick start (short flag)"
    echo ""
    echo "Platform Detection:"
    echo "  🍎 macOS    -> Uses MACOS/ folder"
    echo "  🐧 Linux    -> Uses MACOS/ folder"
    echo "  🖥️  Windows  -> Uses WINOS/ folder"
}

# Check for help flag
if [ "$1" = "--help" ] || [ "$1" = "-h" ]; then
    show_help
    exit 0
fi

# Run main function
main "$@"
