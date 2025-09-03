#!/bin/bash

# Stop all OrderProcessingSystem services on macOS
# File: stop-services.sh

echo "🛑 Stopping OrderProcessingSystem Services..."

# Kill dotnet processes by name and path
echo "Stopping services..."

# Kill by port (more reliable on macOS)
if lsof -ti:5270 > /dev/null 2>&1; then
    echo "Stopping Authentication Service (port 5270)..."
    kill -9 $(lsof -ti:5270) 2>/dev/null
fi

if lsof -ti:5000 > /dev/null 2>&1; then
    echo "Stopping API Service (port 5000)..."
    kill -9 $(lsof -ti:5000) 2>/dev/null
fi

if lsof -ti:5253 > /dev/null 2>&1; then
    echo "Stopping UI Service (port 5253)..."
    kill -9 $(lsof -ti:5253) 2>/dev/null
fi

# Also kill any dotnet processes with OrderProcessingSystem in the path
pkill -f "dotnet.*OrderProcessingSystem" 2>/dev/null

echo "✅ All services stopped!"
echo "You may need to manually close any remaining Terminal windows."
