#!/bin/bash

# Shell script to start OrderProcessingSystem services in separate terminal windows
# File: start-services.sh

echo "🚀 Starting OrderProcessingSystem Services..."
echo "This script will launch 3 services in separate terminal windows:"
echo "  1. Authentication Service (Port 5270)"
echo "  2. API Service (Port 5000)"
echo "  3. UI Service (Port 5253)"
echo ""

# Get the current directory
ROOT_PATH=$(pwd)
echo "Root Path: $ROOT_PATH"

# Define project paths
AUTH_PATH="$ROOT_PATH/OrderProcessingSystem.Authentication"
API_PATH="$ROOT_PATH/OrderProcessingSystem.API"
UI_PATH="$ROOT_PATH/OrderProcessingSystem.UI"

# Function to check if directory exists
check_project_path() {
    if [ -d "$1" ]; then
        echo "✅ Found: $2 at $1"
        return 0
    else
        echo "❌ Missing: $2 at $1"
        return 1
    fi
}

# Verify all project paths exist
echo "🔍 Checking project directories..."
check_project_path "$AUTH_PATH" "Authentication Service" || AUTH_EXISTS=false
check_project_path "$API_PATH" "API Service" || API_EXISTS=false
check_project_path "$UI_PATH" "UI Service" || UI_EXISTS=false

if [ "$AUTH_EXISTS" = "false" ] || [ "$API_EXISTS" = "false" ] || [ "$UI_EXISTS" = "false" ]; then
    echo "❌ One or more project directories not found!"
    echo "Make sure you're running this script from the OrderProcessingSystem root directory."
    read -p "Press Enter to exit"
    exit 1
fi

echo ""
echo "🏁 Starting services in 3 seconds..."
sleep 3

# Start Authentication Service
echo "🔐 Starting Authentication Service..."
osascript -e "tell app \"Terminal\" to do script \"cd '$AUTH_PATH' && echo '🔐 Authentication Service Starting...' && dotnet run\""

# Wait a moment between starts
sleep 2

# Start API Service
echo "🌐 Starting API Service..."
osascript -e "tell app \"Terminal\" to do script \"cd '$API_PATH' && echo '🌐 API Service Starting...' && dotnet run\""

# Wait a moment between starts
sleep 2

# Start UI Service
echo "🎨 Starting UI Service..."
osascript -e "tell app \"Terminal\" to do script \"cd '$UI_PATH' && echo '🎨 UI Service Starting...' && dotnet run\""

echo ""
echo "🎉 All services are starting!"
echo "Check the opened Terminal windows for each service status."
echo ""
echo "📋 Service URLs:"
echo "  🔐 Authentication: http://localhost:5270"
echo "  🌐 API:            http://localhost:5000"
echo "  🎨 UI:             http://localhost:5253"
echo ""
echo "⏳ Services may take 10-30 seconds to fully start."
echo "💡 Close individual Terminal windows to stop each service."
echo ""
echo "✅ Launcher complete!"