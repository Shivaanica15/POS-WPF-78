# ============================================================================
# ResolvePortConflict.ps1
# Checks if requested MySQL port is available, if not finds an available port
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$RequestedPortFile,
    
    [Parameter(Mandatory=$true)]
    [string]$OutputFile
)

Write-Host "Resolving MySQL port conflicts..." -ForegroundColor Cyan

# Read requested port from file
if (-not (Test-Path $RequestedPortFile)) {
    Write-Host "ERROR: Requested port file not found: $RequestedPortFile" -ForegroundColor Red
    exit 1
}

$requestedPort = Get-Content $RequestedPortFile -ErrorAction Stop
$requestedPort = $requestedPort.Trim()

if ([string]::IsNullOrWhiteSpace($requestedPort)) {
    $requestedPort = "3306"
}

Write-Host "Requested MySQL port: $requestedPort" -ForegroundColor Gray

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

# Check if requested port is available
$portInt = [int]$requestedPort
if (-not (Test-Port -Port $portInt)) {
    Write-Host "Port $requestedPort is available!" -ForegroundColor Green
    $finalPort = $requestedPort
}
else {
    Write-Host "Port $requestedPort is in use. Searching for available port..." -ForegroundColor Yellow
    
    # Try ports starting from requested port + 1, up to 3315
    $finalPort = $null
    $startPort = $portInt + 1
    $endPort = 3315
    
    for ($port = $startPort; $port -le $endPort; $port++) {
        Write-Host "Checking port $port..." -ForegroundColor Gray
        
        if (-not (Test-Port -Port $port)) {
            $finalPort = $port.ToString()
            Write-Host "Found available port: $finalPort" -ForegroundColor Green
            break
        }
    }
    
    # If still no port found, try ports below requested port
    if ($null -eq $finalPort) {
        for ($port = $portInt - 1; $port -ge 3306; $port--) {
            Write-Host "Checking port $port..." -ForegroundColor Gray
            
            if (-not (Test-Port -Port $port)) {
                $finalPort = $port.ToString()
                Write-Host "Found available port: $finalPort" -ForegroundColor Green
                break
            }
        }
    }
    
    if ($null -eq $finalPort) {
        Write-Host "ERROR: No available MySQL port found in range 3306-3315!" -ForegroundColor Red
        Write-Host "Please stop other MySQL services or free up a port." -ForegroundColor Yellow
        exit 1
    }
    
    Write-Host "WARNING: Changed MySQL port from $requestedPort to $finalPort due to port conflict" -ForegroundColor Yellow
}

# Write final port to file
try {
    Set-Content -Path $OutputFile -Value $finalPort -ErrorAction Stop
    Write-Host "MySQL port resolved: $finalPort" -ForegroundColor Green
    exit 0
}
catch {
    Write-Host "ERROR: Failed to write port file: $_" -ForegroundColor Red
    exit 1
}

