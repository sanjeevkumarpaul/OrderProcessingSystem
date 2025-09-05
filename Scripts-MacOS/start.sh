#!/bin/bash

# Shell script to start OrderProcessingSystem services in separate terminal windows
# File: start-services.sh

echo "🚀 Starting OrderProcessingSystem Services..."
echo "This script will launch 3 services in separate terminal windows:"
echo "  1. Authentication Service - Port 5270"
echo "  2. API Service - Port 5000"
echo "  3. UI Service - Port 5253"
echo ""

# Get the current directory (parent directory since script is in MACOS folder)
SCRIPT_DIR=$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)
ROOT_PATH=$(dirname "$SCRIPT_DIR")
echo "Script Directory: $SCRIPT_DIR"
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
echo "🔨 Building all projects..."
BUILD_SUCCESS=true
BUILD_ERRORS=()

# Function to build project with detailed error reporting
build_project() {
    local project_path=$1
    local project_name=$2
    local project_icon=$3
    
    echo "${project_icon} Building ${project_name}..."
    cd "$project_path"
    
    # First, restore packages to ensure dependencies are available
    echo "📦 Restoring packages for ${project_name}..."
    RESTORE_OUTPUT=$(dotnet restore --verbosity quiet 2>&1)
    RESTORE_EXIT_CODE=$?
    
    if [ $RESTORE_EXIT_CODE -ne 0 ]; then
        echo "❌ Package restore failed for ${project_name}!"
        echo "📋 Restore error details:"
        echo "$RESTORE_OUTPUT" | head -5 | sed 's/^/   /'
        BUILD_ERRORS+=("${project_name}: Package restore failed")
        BUILD_SUCCESS=false
        return $RESTORE_EXIT_CODE
    fi
    
    # Build the project
    echo "🔨 Compiling ${project_name}..."
    BUILD_OUTPUT=$(dotnet build --no-restore --verbosity normal 2>&1)
    BUILD_EXIT_CODE=$?
    
    if [ $BUILD_EXIT_CODE -eq 0 ]; then
        echo "✅ ${project_name} build successful"
        
        # Check for warnings
        WARNING_COUNT=$(echo "$BUILD_OUTPUT" | grep -c "warning" || true)
        if [ $WARNING_COUNT -gt 0 ]; then
            echo "⚠️  ${project_name} has ${WARNING_COUNT} warnings"
            # Show first few warnings for context
            echo "$BUILD_OUTPUT" | grep "warning" | head -3 | sed 's/^/   📝 /'
        fi
        
        # Check for successful compilation message
        if echo "$BUILD_OUTPUT" | grep -q "Build succeeded"; then
            echo "🎯 ${project_name} compilation verified"
        fi
    else
        echo "❌ ${project_name} build failed!"
        echo "📋 Build error details:"
        
        # Show compilation errors with better filtering
        echo "$BUILD_OUTPUT" | grep -E "(error|Error|fail|Failed)" | grep -v "warning" | head -8 | sed 's/^/   🚨 /'
        
        # Show any specific build errors
        ERROR_LINES=$(echo "$BUILD_OUTPUT" | grep -E "CS[0-9]{4}:" | head -5)
        if [ -n "$ERROR_LINES" ]; then
            echo "   📍 Specific errors:"
            echo "$ERROR_LINES" | sed 's/^/      /'
        fi
        
        # Store error for summary
        BUILD_ERRORS+=("${project_name}: Build compilation failed")
        BUILD_SUCCESS=false
        
        echo ""
    fi
    
    return $BUILD_EXIT_CODE
}

# Build each project
build_project "$AUTH_PATH" "Authentication Service" "🔐"
build_project "$API_PATH" "API Service" "🌐" 
build_project "$UI_PATH" "UI Service" "🎨"

# Return to root directory
cd "$ROOT_PATH"

# Check if all builds were successful
if [ "$BUILD_SUCCESS" = "false" ]; then
    echo ""
    echo "❌ BUILD FAILED! Summary of errors:"
    for error in "${BUILD_ERRORS[@]}"; do
        echo "   • $error"
    done
    echo ""
    echo "🔧 Troubleshooting steps:"
    echo "   1. Check .NET SDK version: dotnet --version"
    echo "   2. Clean and restore all projects:"
    echo "      dotnet clean && dotnet restore"
    echo "   3. Run individual project builds for detailed diagnostics:"
    echo "      • cd '$AUTH_PATH' && dotnet build --verbosity detailed"
    echo "      • cd '$API_PATH' && dotnet build --verbosity detailed"  
    echo "      • cd '$UI_PATH' && dotnet build --verbosity detailed"
    echo "   4. Check for missing NuGet packages or version conflicts"
    echo "   5. Verify project references are correct"
    echo ""
    echo "🔍 Quick diagnostics:"
    echo "   • .NET SDK version: $(dotnet --version 2>/dev/null || echo 'Not installed')"
    echo "   • Current directory: $(pwd)"
    echo "   • Available commands: $(which dotnet 2>/dev/null || echo 'dotnet not in PATH')"
    echo ""
    read -p "❓ Press Enter to exit and fix build errors..."
    exit 1
fi

echo ""
echo "🎉 All builds completed successfully!"
echo "🔍 Build Summary:"
echo "   ✅ Authentication Service - Ready to start"
echo "   ✅ API Service - Ready to start"
echo "   ✅ UI Service - Ready to start"
echo ""
echo "🚀 Pre-flight check complete!"
echo "📊 System ready for launch..."
echo ""
echo "🏁 Auto-starting services in 3 seconds..."
echo "💡 Press Ctrl+C now to cancel automatic startup"
sleep 1 && echo "   🕐 2 seconds..." && sleep 1 && echo "   🕑 1 second..." && sleep 1

echo ""
echo "🔍 Final verification - checking compiled binaries..."

# Verify that build outputs exist
VERIFICATION_SUCCESS=true

verify_build_output() {
    local project_path=$1
    local project_name=$2
    
    # Look for common build output patterns
    if [ -d "$project_path/bin" ] && [ -d "$project_path/obj" ]; then
        echo "✅ $project_name build artifacts verified"
        return 0
    else
        echo "⚠️  $project_name build artifacts not found"
        VERIFICATION_SUCCESS=false
        return 1
    fi
}

verify_build_output "$AUTH_PATH" "Authentication Service"
verify_build_output "$API_PATH" "API Service" 
verify_build_output "$UI_PATH" "UI Service"

if [ "$VERIFICATION_SUCCESS" = "false" ]; then
    echo "⚠️  Warning: Some build artifacts missing, but continuing startup..."
    echo "💡 Services may take longer to start on first run"
fi

echo ""
echo "🚀 Launching services..."

# Start Authentication Service
echo "🔐 Starting Authentication Service..."
osascript -e "tell app \"Terminal\" to do script \"cd '$AUTH_PATH' && echo '🔐 Authentication Service Starting...' && echo 'Building and starting on http://localhost:5270' && dotnet run\""

# Wait for Authentication service to initialize
echo "⏳ Waiting for Authentication service to initialize..."
sleep 3

# Start API Service
echo "🌐 Starting API Service..."
osascript -e "tell app \"Terminal\" to do script \"cd '$API_PATH' && echo '🌐 API Service Starting...' && echo 'Building and starting on http://localhost:5000' && dotnet run\""

# Wait for API service to initialize
echo "⏳ Waiting for API service to initialize..."
sleep 3

# Start UI Service
echo "🎨 Starting UI Service..."
osascript -e "tell app \"Terminal\" to do script \"cd '$UI_PATH' && echo '🎨 UI Service Starting...' && echo 'Building and starting on http://localhost:5253' && echo 'This is the main application interface' && dotnet run\""

echo ""
echo "🎉 All services are launching!"
echo "📱 Three Terminal windows have been opened - one for each service."
echo ""
echo "📋 Service Information:"
echo "  🔐 Authentication Service: http://localhost:5270"
echo "     • Handles user login and JWT tokens"
echo "     • Should start first - dependency for other services"
echo ""
echo "  🌐 API Service: http://localhost:5000"
echo "     • RESTful API for order processing"
echo "     • Provides data to the UI application"
echo ""
echo "  🎨 UI Service: http://localhost:5253"
echo "     • Main Blazor web application"
echo "     • Your primary interface - visit this URL"
echo ""
echo "⏳ Initial startup may take 15-45 seconds per service."
echo "🔄 Each service will show \"Now listening on...\" when ready."
echo "💡 Close individual Terminal windows to stop each service."
echo ""
echo "🌐 Main Application: http://localhost:5253"
echo "✅ Launch sequence complete!"