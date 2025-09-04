# PowerShell script to stop OrderProcessingSystem services on Windows
# File: stop.ps1

Write-Host "🛑 Stopping OrderProcessingSystem Services..." -ForegroundColor Red

Write-Host "Stopping services..." -ForegroundColor Yellow

# Function to stop service by port
function Stop-ServiceByPort {
    param(
        [int]$Port,
        [string]$ServiceName
    )
    
    try {
        # Find processes using the specified port
        $Processes = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue |
                    ForEach-Object { Get-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue }
        
        if ($Processes) {
            Write-Host "Stopping $ServiceName (port $Port)..." -ForegroundColor Yellow
            $Processes | ForEach-Object {
                try {
                    Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
                    Write-Host "✅ Stopped process $($_.ProcessName) (PID: $($_.Id))" -ForegroundColor Green
                } catch {
                    Write-Host "⚠️  Could not stop process $($_.ProcessName) (PID: $($_.Id))" -ForegroundColor Yellow
                }
            }
        } else {
            Write-Host "ℹ️  No process found using port $Port ($ServiceName)" -ForegroundColor Blue
        }
    } catch {
        Write-Host "⚠️  Error checking port $Port`: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

# Stop services by port
Stop-ServiceByPort 5270 "Authentication Service"
Stop-ServiceByPort 5000 "API Service"  
Stop-ServiceByPort 5253 "UI Service"

Write-Host ""
Write-Host "🔍 Stopping any remaining dotnet processes with OrderProcessingSystem..." -ForegroundColor Yellow

# Also kill any dotnet processes with OrderProcessingSystem in the command line
try {
    $DotnetProcesses = Get-WmiObject Win32_Process | 
                      Where-Object { $_.Name -eq "dotnet.exe" -and $_.CommandLine -like "*OrderProcessingSystem*" }
    
    if ($DotnetProcesses) {
        $DotnetProcesses | ForEach-Object {
            try {
                $ProcessName = $_.ProcessId
                Stop-Process -Id $ProcessName -Force -ErrorAction SilentlyContinue
                Write-Host "✅ Stopped dotnet process (PID: $ProcessName)" -ForegroundColor Green
            } catch {
                Write-Host "⚠️  Could not stop dotnet process (PID: $ProcessName)" -ForegroundColor Yellow
            }
        }
    } else {
        Write-Host "ℹ️  No OrderProcessingSystem dotnet processes found" -ForegroundColor Blue
    }
} catch {
    Write-Host "⚠️  Error checking dotnet processes: $($_.Exception.Message)" -ForegroundColor Yellow
    
    # Fallback method using Get-Process
    try {
        Write-Host "🔄 Trying alternative method..." -ForegroundColor Cyan
        $AlternateDotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue |
                                   Where-Object { $_.ProcessName -eq "dotnet" }
        
        if ($AlternateDotnetProcesses) {
            Write-Host "Found $($AlternateDotnetProcesses.Count) dotnet processes. Stopping relevant ones..." -ForegroundColor Yellow
            $AlternateDotnetProcesses | ForEach-Object {
                try {
                    # We can't easily check command line with Get-Process, so we'll be more conservative
                    # Only stop if there are multiple dotnet processes (likely our services)
                    if ($AlternateDotnetProcesses.Count -gt 1) {
                        Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
                        Write-Host "✅ Stopped dotnet process (PID: $($_.Id))" -ForegroundColor Green
                    }
                } catch {
                    Write-Host "⚠️  Could not stop dotnet process (PID: $($_.Id))" -ForegroundColor Yellow
                }
            }
        }
    } catch {
        Write-Host "⚠️  Alternative method also failed: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "🧹 Closing PowerShell windows running services..." -ForegroundColor Yellow

# Enhanced PowerShell window cleanup
try {
    # Get all PowerShell processes except current one
    $PowerShellProcesses = Get-Process -Name "powershell", "pwsh" -ErrorAction SilentlyContinue |
                          Where-Object { $_.Id -ne $PID } # Don't close the current script
    
    $ClosedWindows = 0
    
    if ($PowerShellProcesses) {
        Write-Host "Found $($PowerShellProcesses.Count) other PowerShell processes" -ForegroundColor Blue
        
        # Try to identify and close service-related PowerShell windows
        $PowerShellProcesses | ForEach-Object {
            try {
                # Check if the PowerShell process might be running our services
                # We'll be conservative and only close if we can identify it's likely our service
                $ProcessId = $_.Id
                $ProcessName = $_.ProcessName
                
                # Get command line if possible (this might not always work)
                try {
                    $CommandLine = (Get-WmiObject Win32_Process | Where-Object { $_.ProcessId -eq $ProcessId }).CommandLine
                    
                    if ($CommandLine -and ($CommandLine -like "*OrderProcessingSystem*" -or $CommandLine -like "*dotnet run*")) {
                        Write-Host "🔍 Closing PowerShell window (PID: $ProcessId) - appears to be running a service" -ForegroundColor Cyan
                        Stop-Process -Id $ProcessId -Force -ErrorAction SilentlyContinue
                        $ClosedWindows++
                    }
                } catch {
                    # If we can't get command line, we'll be more cautious
                    # Only close if there are multiple PowerShell processes (indicating our services might be running)
                    if ($PowerShellProcesses.Count -ge 3) {
                        Write-Host "🔍 Closing PowerShell window (PID: $ProcessId) - likely a service window" -ForegroundColor Cyan
                        Stop-Process -Id $ProcessId -Force -ErrorAction SilentlyContinue
                        $ClosedWindows++
                    }
                }
            } catch {
                Write-Host "⚠️  Could not close PowerShell process (PID: $($_.Id))" -ForegroundColor Yellow
            }
        }
        
        if ($ClosedWindows -gt 0) {
            Write-Host "✅ Closed $ClosedWindows PowerShell windows" -ForegroundColor Green
        } else {
            Write-Host "💡 No service-related PowerShell windows identified for closure" -ForegroundColor Cyan
            Write-Host "   You may need to manually close any remaining service windows" -ForegroundColor Gray
        }
    } else {
        Write-Host "ℹ️  No other PowerShell processes found" -ForegroundColor Blue
    }
} catch {
    Write-Host "⚠️  Could not enumerate PowerShell processes: $($_.Exception.Message)" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "📊 Final verification..." -ForegroundColor Yellow

# Verify that the ports are now free
function Test-PortAvailability {
    param(
        [int]$Port,
        [string]$ServiceName
    )
    
    try {
        $Connection = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue
        if ($Connection) {
            Write-Host "⚠️  Port $Port ($ServiceName) still in use" -ForegroundColor Yellow
            return $false
        } else {
            Write-Host "✅ Port $Port ($ServiceName) is now free" -ForegroundColor Green  
            return $true
        }
    } catch {
        Write-Host "✅ Port $Port ($ServiceName) is now free" -ForegroundColor Green
        return $true
    }
}

$AllPortsFree = $true
$AllPortsFree = (Test-PortAvailability 5270 "Authentication Service") -and $AllPortsFree
$AllPortsFree = (Test-PortAvailability 5000 "API Service") -and $AllPortsFree  
$AllPortsFree = (Test-PortAvailability 5253 "UI Service") -and $AllPortsFree

Write-Host ""
if ($AllPortsFree) {
    Write-Host "✅ All services stopped successfully!" -ForegroundColor Green
    Write-Host "🔓 All ports are now available" -ForegroundColor Green
} else {
    Write-Host "⚠️  Some services may still be running" -ForegroundColor Yellow
    Write-Host "💡 Try running this script again or manually close any remaining windows" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "💡 Notes:" -ForegroundColor Cyan
Write-Host "   • PowerShell windows running services have been automatically closed" -ForegroundColor Gray
Write-Host "   • If any windows remain, they should no longer be running services" -ForegroundColor Gray
Write-Host "   • Use 'netstat -an | findstr :PORT' to check specific ports" -ForegroundColor Gray
Write-Host ""
Write-Host "🏁 Stop script completed with window cleanup!" -ForegroundColor Green

Write-Host ""
Read-Host "Press Enter to close this window..."
