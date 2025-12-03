# ============================================================================
# FindAvailablePort.ps1
# Detects available MySQL port (3306-3315) and writes to temp file
# ============================================================================

param(
    [string]$OutputFile = "$env:TEMP\mysql_port.txt"
)

Write-Host "Scanning for available MySQL port..." -ForegroundColor Cyan

# Function to check if port is in use
function Test-Port {
    param([int]$Port)
    
    try {
        $connection = Test-NetConnection -ComputerName localhost -Port $Port -WarningAction SilentlyContinue -InformationLevel Quiet -ErrorAction SilentlyContinue
        if ($connection) {
            return $true
        }
        
        # Also check using netstat
        $netstat = netstat -an | Select-String ":$Port\s"
        if ($netstat) {
            return $true
        }
        
        return $false
    }
    catch {
        return $false
    }
}

# Try ports 3306 through 3315
$availablePort = $null
for ($port = 3306; $port -le 3315; $port++) {
    Write-Host "Checking port $port..." -ForegroundColor Gray
    
    if (-not (Test-Port -Port $port)) {
        $availablePort = $port
        Write-Host "Port $port is available!" -ForegroundColor Green
        break
    }
    else {
        Write-Host "Port $port is in use." -ForegroundColor Yellow
    }
}

if ($null -eq $availablePort) {
    Write-Host "ERROR: No available port found in range 3306-3315!" -ForegroundColor Red
    exit 1
}

# Write port to file
try {
    Set-Content -Path $OutputFile -Value $availablePort -ErrorAction Stop
    Write-Host "Selected port $availablePort written to $OutputFile" -ForegroundColor Green
    exit 0
}
catch {
    Write-Host "ERROR: Failed to write port file: $_" -ForegroundColor Red
    exit 1
}

