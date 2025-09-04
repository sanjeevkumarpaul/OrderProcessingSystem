# PowerShell script to start OrderProcessingSystem services in separate windows
# File: start.ps1

Write-Host "🚀 Starting OrderProcessingSystem Services..." -ForegroundColor Green
Write-Host "This script will launch 3 services in separate PowerShell windows:"
Write-Host "  1. Authentication Service - Port 5270" -ForegroundColor Cyan
Write-Host "  2. API Service - Port 5269" -ForegroundColor Cyan
Write-Host "  3. UI Service - Port 5253" -ForegroundColor Cyan
Write-Host ""

# Get the current directory (parent directory since script is in WINOS folder)
$SCRIPT_PATH = Get-Location
$ROOT_PATH = Split-Path $SCRIPT_PATH -Parent
Write-Host "Script Path: $SCRIPT_PATH" -ForegroundColor Blue
Write-Host "Root Path: $ROOT_PATH" -ForegroundColor Yellow

# Define project paths
$AUTH_PATH = Join-Path $ROOT_PATH "OrderProcessingSystem.Authentication"
$API_PATH = Join-Path $ROOT_PATH "OrderProcessingSystem.API"
$UI_PATH = Join-Path $ROOT_PATH "OrderProcessingSystem.UI"

# Function to check if directory exists
function Test-ProjectPath {
    param(
        [string]$Path,
        [string]$ServiceName
    )
    
    if (Test-Path $Path) {
        Write-Host "✅ Found: $ServiceName at $Path" -ForegroundColor Green
        return $true
    } else {
        Write-Host "❌ Missing: $ServiceName at $Path" -ForegroundColor Red
        return $false
    }
}

# Verify all project paths exist
Write-Host "🔍 Checking project directories..." -ForegroundColor Yellow
$AuthExists = Test-ProjectPath $AUTH_PATH "Authentication Service"
$ApiExists = Test-ProjectPath $API_PATH "API Service"
$UiExists = Test-ProjectPath $UI_PATH "UI Service"

if (-not $AuthExists -or -not $ApiExists -or -not $UiExists) {
    Write-Host "❌ One or more project directories not found!" -ForegroundColor Red
    Write-Host "Make sure you're running this script from the OrderProcessingSystem root directory." -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "🔨 Building all projects..." -ForegroundColor Yellow
$BuildSuccess = $true
$BuildErrors = @()

# Function to build project with detailed error reporting
function Build-Project {
    param(
        [string]$ProjectPath,
        [string]$ProjectName,
        [string]$ProjectIcon
    )
    
    Write-Host "$ProjectIcon Building $ProjectName..." -ForegroundColor Cyan
    Push-Location $ProjectPath
    
    try {
        # First, restore packages to ensure dependencies are available
        Write-Host "📦 Restoring packages for $ProjectName..." -ForegroundColor Blue
        $RestoreOutput = dotnet restore --verbosity quiet 2>&1
        $RestoreExitCode = $LASTEXITCODE
        
        if ($RestoreExitCode -ne 0) {
            Write-Host "❌ Package restore failed for $ProjectName!" -ForegroundColor Red
            Write-Host "📋 Restore error details:" -ForegroundColor Yellow
            $RestoreOutput | Select-Object -First 5 | ForEach-Object { Write-Host "   $_" -ForegroundColor Red }
            $script:BuildErrors += "$ProjectName`: Package restore failed"
            $script:BuildSuccess = $false
            return $RestoreExitCode
        }
        
        # Build the project
        Write-Host "🔨 Compiling $ProjectName..." -ForegroundColor Blue
        $BuildOutput = dotnet build --no-restore --verbosity normal 2>&1
        $BuildExitCode = $LASTEXITCODE
        
        if ($BuildExitCode -eq 0) {
            Write-Host "✅ $ProjectName build successful" -ForegroundColor Green
            
            # Check for warnings
            $WarningCount = ($BuildOutput | Select-String "warning").Count
            if ($WarningCount -gt 0) {
                Write-Host "⚠️  $ProjectName has $WarningCount warnings" -ForegroundColor Yellow
                # Show first few warnings for context
                $BuildOutput | Select-String "warning" | Select-Object -First 3 | ForEach-Object {
                    Write-Host "   📝 $_" -ForegroundColor Yellow
                }
            }
            
            # Check for successful compilation message
            if ($BuildOutput -match "Build succeeded") {
                Write-Host "🎯 $ProjectName compilation verified" -ForegroundColor Green
            }
        } else {
            Write-Host "❌ $ProjectName build failed!" -ForegroundColor Red
            Write-Host "📋 Build error details:" -ForegroundColor Yellow
            
            # Show compilation errors with better filtering
            $BuildOutput | Select-String -Pattern "(error|Error|fail|Failed)" | 
                Where-Object { $_ -notmatch "warning" } | 
                Select-Object -First 8 | 
                ForEach-Object { Write-Host "   🚨 $_" -ForegroundColor Red }
            
            # Show any specific build errors
            $ErrorLines = $BuildOutput | Select-String "CS\d{4}:" | Select-Object -First 5
            if ($ErrorLines) {
                Write-Host "   📍 Specific errors:" -ForegroundColor Yellow
                $ErrorLines | ForEach-Object { Write-Host "      $_" -ForegroundColor Red }
            }
            
            # Store error for summary
            $script:BuildErrors += "$ProjectName`: Build compilation failed"
            $script:BuildSuccess = $false
            
            Write-Host ""
        }
        
        return $BuildExitCode
    } finally {
        Pop-Location
    }
}

# Build each project
Build-Project $AUTH_PATH "Authentication Service" "🔐"
Build-Project $API_PATH "API Service" "🌐"
Build-Project $UI_PATH "UI Service" "🎨"

# Check if all builds were successful
if (-not $BuildSuccess) {
    Write-Host ""
    Write-Host "❌ BUILD FAILED! Summary of errors:" -ForegroundColor Red
    $BuildErrors | ForEach-Object { Write-Host "   • $_" -ForegroundColor Red }
    Write-Host ""
    Write-Host "🔧 Troubleshooting steps:" -ForegroundColor Yellow
    Write-Host "   1. Check .NET SDK version: dotnet --version"
    Write-Host "   2. Clean and restore all projects:"
    Write-Host "      dotnet clean && dotnet restore"
    Write-Host "   3. Run individual project builds for detailed diagnostics:"
    Write-Host "      • cd '$AUTH_PATH' && dotnet build --verbosity detailed"
    Write-Host "      • cd '$API_PATH' && dotnet build --verbosity detailed"
    Write-Host "      • cd '$UI_PATH' && dotnet build --verbosity detailed"
    Write-Host "   4. Check for missing NuGet packages or version conflicts"
    Write-Host "   5. Verify project references are correct"
    Write-Host ""
    Write-Host "🔍 Quick diagnostics:" -ForegroundColor Yellow
    $DotnetVersion = try { dotnet --version } catch { 'Not installed' }
    Write-Host "   • .NET SDK version: $DotnetVersion"
    Write-Host "   • Current directory: $(Get-Location)"
    Write-Host "   • Available commands: $(if (Get-Command dotnet -ErrorAction SilentlyContinue) { 'dotnet found' } else { 'dotnet not in PATH' })"
    Write-Host ""
    Read-Host "❓ Press Enter to exit and fix build errors..."
    exit 1
}

Write-Host ""
Write-Host "🎉 All builds completed successfully!" -ForegroundColor Green
Write-Host "🔍 Build Summary:" -ForegroundColor Yellow
Write-Host "   ✅ Authentication Service - Ready to start" -ForegroundColor Green
Write-Host "   ✅ API Service - Ready to start" -ForegroundColor Green
Write-Host "   ✅ UI Service - Ready to start" -ForegroundColor Green
Write-Host ""
Write-Host "🚀 Pre-flight check complete!" -ForegroundColor Green
Write-Host "📊 System ready for launch..." -ForegroundColor Blue
Write-Host ""
Write-Host "🏁 Auto-starting services in 3 seconds..." -ForegroundColor Yellow
Write-Host "💡 Press Ctrl+C now to cancel automatic startup" -ForegroundColor Cyan

Start-Sleep 1
Write-Host "   🕐 2 seconds..." -ForegroundColor Yellow
Start-Sleep 1
Write-Host "   🕑 1 second..." -ForegroundColor Yellow
Start-Sleep 1

Write-Host ""
Write-Host "🔍 Final verification - checking compiled binaries..." -ForegroundColor Yellow

# Verify that build outputs exist
$VerificationSuccess = $true

function Test-BuildOutput {
    param(
        [string]$ProjectPath,
        [string]$ProjectName
    )
    
    $BinPath = Join-Path $ProjectPath "bin"
    $ObjPath = Join-Path $ProjectPath "obj"
    
    if ((Test-Path $BinPath) -and (Test-Path $ObjPath)) {
        Write-Host "✅ $ProjectName build artifacts verified" -ForegroundColor Green
        return $true
    } else {
        Write-Host "⚠️  $ProjectName build artifacts not found" -ForegroundColor Yellow
        $script:VerificationSuccess = $false
        return $false
    }
}

Test-BuildOutput $AUTH_PATH "Authentication Service"
Test-BuildOutput $API_PATH "API Service"
Test-BuildOutput $UI_PATH "UI Service"

if (-not $VerificationSuccess) {
    Write-Host "⚠️  Warning: Some build artifacts missing, but continuing startup..." -ForegroundColor Yellow
    Write-Host "💡 Services may take longer to start on first run" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "🚀 Launching services..." -ForegroundColor Green

# Function to start service in new PowerShell window
function Start-ServiceInNewWindow {
    param(
        [string]$ServicePath,
        [string]$ServiceName,
        [string]$ServiceIcon,
        [int]$Port
    )
    
    Write-Host "$ServiceIcon Starting $ServiceName..." -ForegroundColor Cyan
    
    $Command = @"
cd '$ServicePath'
Write-Host '$ServiceIcon $ServiceName Starting...' -ForegroundColor Green
Write-Host 'Building and starting on http://localhost:$Port' -ForegroundColor Yellow
dotnet run
"@
    
    # Start in new PowerShell window
    Start-Process powershell -ArgumentList "-NoExit", "-Command", $Command
}

# Start Authentication Service
Start-ServiceInNewWindow $AUTH_PATH "Authentication Service" "🔐" 5270

# Wait for Authentication service to initialize
Write-Host "⏳ Waiting for Authentication service to initialize..." -ForegroundColor Yellow
Start-Sleep 3

# Start API Service
Start-ServiceInNewWindow $API_PATH "API Service" "🌐" 5269

# Wait for API service to initialize
Write-Host "⏳ Waiting for API service to initialize..." -ForegroundColor Yellow
Start-Sleep 3

# Start UI Service
Start-ServiceInNewWindow $UI_PATH "UI Service" "🎨" 5253

Write-Host ""
Write-Host "🎉 All services are launching!" -ForegroundColor Green
Write-Host "📱 Three PowerShell windows have been opened - one for each service." -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Service Information:" -ForegroundColor Yellow
Write-Host "  🔐 Authentication Service: http://localhost:5270" -ForegroundColor Cyan
Write-Host "     • Handles user login and JWT tokens" -ForegroundColor Gray
Write-Host "     • Should start first - dependency for other services" -ForegroundColor Gray
Write-Host ""
Write-Host "  🌐 API Service: http://localhost:

" -ForegroundColor Cyan
Write-Host "     • RESTful API for order processing" -ForegroundColor Gray
Write-Host "     • Provides data to the UI application" -ForegroundColor Gray
Write-Host ""
Write-Host "  🎨 UI Service: http://localhost:5253" -ForegroundColor Cyan
Write-Host "     • Main Blazor web application" -ForegroundColor Gray
Write-Host "     • Your primary interface - visit this URL" -ForegroundColor Gray
Write-Host ""
Write-Host "⏳ Initial startup may take 15-45 seconds per service." -ForegroundColor Yellow
Write-Host "🔄 Each service will show 'Now listening on...' when ready." -ForegroundColor Yellow
Write-Host "💡 Close individual PowerShell windows to stop each service." -ForegroundColor Cyan
Write-Host ""
Write-Host "🌐 Main Application: http://localhost:5253" -ForegroundColor Green -BackgroundColor DarkBlue
Write-Host "✅ Launch sequence complete!" -ForegroundColor Green

Write-Host ""
Read-Host "Press Enter to close this window..."
