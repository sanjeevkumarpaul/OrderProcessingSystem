# Quick start script - minimal output
# File: quick-start.ps1

$authPath = Join-Path (Get-Location) "OrderProcessingSystem.Authentication"
$apiPath = Join-Path (Get-Location) "OrderProcessingSystem.API" 
$uiPath = Join-Path (Get-Location) "OrderProcessingSystem.UI"

Write-Host "Starting OrderProcessingSystem..." -ForegroundColor Green

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$authPath'; dotnet run"
Start-Sleep 2
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$apiPath'; dotnet run"  
Start-Sleep 2
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$uiPath'; dotnet run"

Write-Host "Services starting... Check PowerShell windows." -ForegroundColor Yellow
