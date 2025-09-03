#!/bin/bash
# Stop all OrderProcessingSystem services on macOS
# File: stop-services.sh

echo "🛑 Stopping OrderProcessingSystem Services..."

# Kill processes by name and path patterns
echo "Stopping Authentication Service..."
pkill -f "OrderProcessingSystem.Authentication"

echo "Stopping API Service..."
pkill -f "OrderProcessingSystem.API"

echo "Stopping UI Service..."  
pkill -f "OrderProcessingSystem.UI"

# Alternative: Kill by port
echo "Checking and killing processes on ports..."

# Function to kill process by port
kill_by_port() {
    local port=$1
    local pid=$(lsof -ti tcp:$port)
    if [ ! -z "$pid" ]; then
        echo "Killing process on port $port (PID: $pid)"
        kill -9 $pid 2>/dev/null
    fi
}

kill_by_port 5270  # Authentication
kill_by_port 5000  # API
kill_by_port 5253  # UI

echo "✅ All services stopped!"
read -p "Press Enter to close..."
