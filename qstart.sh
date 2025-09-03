#!/bin/bash

# Quick start script for macOS - minimal output
# File: quick-start.sh

ROOT_PATH=$(pwd)
AUTH_PATH="$ROOT_PATH/OrderProcessingSystem.Authentication"
API_PATH="$ROOT_PATH/OrderProcessingSystem.API"
UI_PATH="$ROOT_PATH/OrderProcessingSystem.UI"

echo "🚀 Starting OrderProcessingSystem..."

osascript -e "tell app \"Terminal\" to do script \"cd '$AUTH_PATH' && dotnet run\""
sleep 2
osascript -e "tell app \"Terminal\" to do script \"cd '$API_PATH' && dotnet run\""
sleep 2
osascript -e "tell app \"Terminal\" to do script \"cd '$UI_PATH' && dotnet run\""

echo "✅ Services starting... Check Terminal windows."
