#!/bin/bash

# Stop all OrderProcessingSystem services on macOS
# File: stop-services.sh

echo "🛑 Stopping OrderProcessingSystem Services..."

# Kill dotnet processes by name and path
echo "Stopping services..."

# Enhanced function to find and stop OrderProcessingSystem services
stop_service_smart() {
    local expected_port=$1
    local service_name=$2
    local found=false
    
    echo "🔍 Looking for $service_name..."
    
    # First, check the expected port
    if lsof -ti:$expected_port > /dev/null 2>&1; then
        echo "Found $service_name on expected port $expected_port"
        local pid=$(lsof -ti:$expected_port)
        kill -9 $pid 2>/dev/null
        echo "✅ $service_name stopped (port $expected_port)"
        found=true
    else
        # If not on expected port, look for dotnet processes with this service name
        local dotnet_pids=$(pgrep -f "dotnet.*OrderProcessingSystem.*$service_name" 2>/dev/null)
        
        if [ -n "$dotnet_pids" ]; then
            echo "Found $service_name processes (not on expected port): $dotnet_pids"
            for pid in $dotnet_pids; do
                # Get the actual port this process is using
                local actual_port=$(lsof -Pan -p $pid -i | grep LISTEN | awk '{print $9}' | cut -d: -f2 | head -1)
                if [ -n "$actual_port" ]; then
                    echo "Found $service_name running on port $actual_port (PID: $pid)"
                else
                    echo "Found $service_name process (PID: $pid)"
                fi
                kill -9 $pid 2>/dev/null
                echo "✅ $service_name stopped (PID: $pid)"
                found=true
            done
        fi
    fi
    
    if [ "$found" = false ]; then
        echo "ℹ️  No $service_name process found"
    fi
}

# Stop services intelligently (checks expected ports and finds processes by name)
stop_service_smart 5270 "Authentication"
stop_service_smart 5269 "API"
stop_service_smart 5253 "UI"

echo ""
echo "🧹 Final cleanup - stopping any remaining OrderProcessingSystem processes..."

# Kill any dotnet processes that might still be running with OrderProcessingSystem
pkill -f "dotnet.*OrderProcessingSystem" 2>/dev/null && echo "✅ Stopped remaining dotnet processes" || echo "ℹ️  No remaining dotnet processes found"

# Also kill any processes that might be running on common alternate ports
for port in 5269 5268 5267 5001 5002; do
    if lsof -ti:$port > /dev/null 2>&1; then
        pid=$(lsof -ti:$port)
        # Check if it's a dotnet process
        if ps -p $pid -o comm= | grep -q dotnet; then
            echo "Found dotnet process on alternate port $port, stopping..."
            kill -9 $pid 2>/dev/null
            echo "✅ Stopped process on port $port"
        fi
    fi
done

echo ""
echo "🔍 Closing Terminal windows running OrderProcessingSystem services..."

# Simple but effective approach - close any terminal tabs with dotnet processes
closed_count=$(osascript -e '
tell application "Terminal"
    set closedCount to 0
    set windowList to every window
    
    repeat with win in windowList
        try
            set tabList to every tab of win
            set tabsToClose to {}
            
            repeat with currentTab in tabList
                try
                    set tabProcesses to processes of currentTab
                    
                    if tabProcesses contains "dotnet" then
                        set tabsToClose to tabsToClose & {currentTab}
                    end if
                on error
                end try
            end repeat
            
            repeat with tabToClose in tabsToClose
                try
                    close tabToClose
                    set closedCount to closedCount + 1
                on error
                end try
            end repeat
            
        on error
        end try
    end repeat
    
    return closedCount
end tell
' 2>/dev/null || echo "0")

if [ "$closed_count" -gt 0 ]; then
    echo "✅ Closed $closed_count Terminal tabs running dotnet services"
else
    echo "ℹ️  No Terminal tabs with dotnet processes found"
    echo "💡 You may need to manually close any remaining Terminal windows"
fi

echo ""
echo "✅ All services stopped and terminal cleanup completed!"
echo "💡 If any Terminal windows remain open, they should no longer be running services."
