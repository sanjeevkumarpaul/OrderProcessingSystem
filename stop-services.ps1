# Stop all OrderProcessingSystem services
# File: stop-services.ps1

Write-Host "🛑 Stopping OrderProcessingSystem Services..." -ForegroundColor Red

# Kill dotnet processes related to OrderProcessingSystem
Write-Host "Stopping Authentication Service..." -ForegroundColor Yellow
Get-Process | Where-Object {$_.ProcessName -eq "dotnet" -and $_.Path -like "*OrderProcessingSystem.Authentication*"} | Stop-Process -Force

Write-Host "Stopping API Service..." -ForegroundColor Yellow  
Get-Process | Where-Object {$_.ProcessName -eq "dotnet" -and $_.Path -like "*OrderProcessingSystem.API*"} | Stop-Process -Force

Write-Host "Stopping UI Service..." -ForegroundColor Yellow
Get-Process | Where-Object {$_.ProcessName -eq "dotnet" -and $_.Path -like "*OrderProcessingSystem.UI*"} | Stop-Process -Force

# Alternative approach - kill by port
Write-Host "Checking ports..." -ForegroundColor Gray

# Function to kill process by port
function Stop-ProcessByPort($port) {
    $processId = (netstat -ano | findstr ":$port" | ForEach-Object {($_ -split '\s+')[-1]} | Select-Object -First 1)
    if ($processId) {
        Write-Host "Stopping process on port $port (PID: $processId)" -ForegroundColor Yellow
        Stop-Process -Id $processId -Force -ErrorAction SilentlyContinue
    }
}

Stop-ProcessByPort 5270  # Authentication
Stop-ProcessByPort 5000  # API  
Stop-ProcessByPort 5253  # UI

Write-Host "✅ All services stopped!" -ForegroundColor Green
Read-Host "Press Enter to close"
