# ============================================================================
# StartMySQL.ps1
# Starts MySQL server with configured port
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$XamppPath,
    
    [Parameter(Mandatory=$true)]
    [string]$PortFile
)

Write-Host "Starting MySQL server..." -ForegroundColor Cyan

# Read port from file
if (-not (Test-Path $PortFile)) {
    Write-Host "ERROR: Port file not found: $PortFile" -ForegroundColor Red
    exit 1
}

$port = Get-Content $PortFile -ErrorAction Stop
Write-Host "Starting MySQL on port: $port" -ForegroundColor Green

# Paths
$mysqlPath = Join-Path $XamppPath "mysql\bin\mysqld.exe"
$myIniPath = Join-Path $XamppPath "mysql\bin\my.ini"

if (-not (Test-Path $mysqlPath)) {
    Write-Host "ERROR: MySQL executable not found at: $mysqlPath" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $myIniPath)) {
    Write-Host "ERROR: my.ini not found at: $myIniPath" -ForegroundColor Red
    exit 1
}

try {
    # Start MySQL in background
    $process = Start-Process -FilePath $mysqlPath -ArgumentList "--defaults-file=`"$myIniPath`" --standalone --console" -WindowStyle Hidden -PassThru -ErrorAction Stop
    
    Write-Host "MySQL process started (PID: $($process.Id))" -ForegroundColor Green
    
    # Wait for MySQL to initialize
    Write-Host "Waiting for MySQL to initialize..." -ForegroundColor Gray
    Start-Sleep -Seconds 5
    
    # Verify MySQL is running
    $mysqlProcess = Get-Process -Id $process.Id -ErrorAction SilentlyContinue
    if ($mysqlProcess) {
        Write-Host "MySQL server is running successfully!" -ForegroundColor Green
        exit 0
    }
    else {
        Write-Host "WARNING: MySQL process may have exited" -ForegroundColor Yellow
        exit 1
    }
}
catch {
    Write-Host "ERROR: Failed to start MySQL: $_" -ForegroundColor Red
    exit 1
}

