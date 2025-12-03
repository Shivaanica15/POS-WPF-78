# ============================================================================
# ValidateMySQLPassword.ps1
# Validates MySQL root password by attempting connection
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$PortFile,
    
    [Parameter(Mandatory=$true)]
    [string]$PasswordFile
)

Write-Host "Validating MySQL password..." -ForegroundColor Cyan

# Read port from file
if (-not (Test-Path $PortFile)) {
    Write-Host "ERROR: Port file not found: $PortFile" -ForegroundColor Red
    exit 1
}

$port = Get-Content $PortFile -ErrorAction SilentlyContinue
if ([string]::IsNullOrWhiteSpace($port)) {
    $port = "3306"
}
$port = $port.Trim()

# Read password from file
if (-not (Test-Path $PasswordFile)) {
    Write-Host "ERROR: Password file not found: $PasswordFile" -ForegroundColor Red
    exit 1
}

$password = Get-Content $PasswordFile -ErrorAction SilentlyContinue
if ([string]::IsNullOrWhiteSpace($password)) {
    Write-Host "Password is empty - skipping validation (no password set)" -ForegroundColor Yellow
    exit 0
}

Write-Host "Validating password for MySQL on port: $port" -ForegroundColor Gray

try {
    # Try to find MySQL client
    $mysqlPath = "mysql.exe"
    $mysqlFound = $false
    
    # Check PATH
    $env:Path -split ';' | ForEach-Object {
        $testPath = Join-Path $_ "mysql.exe"
        if (Test-Path $testPath) {
            $mysqlPath = $testPath
            $mysqlFound = $true
        }
    }
    
    # Check common XAMPP locations
    if (-not $mysqlFound) {
        $commonPaths = @(
            "C:\xampp\mysql\bin\mysql.exe",
            "C:\Program Files\xampp\mysql\bin\mysql.exe",
            "C:\Program Files (x86)\xampp\mysql\bin\mysql.exe"
        )
        
        foreach ($testPath in $commonPaths) {
            if (Test-Path $testPath) {
                $mysqlPath = $testPath
                $mysqlFound = $true
                break
            }
        }
    }
    
    if (-not $mysqlFound) {
        Write-Host "ERROR: MySQL client not found" -ForegroundColor Red
        exit 1
    }
    
    # Test connection
    $testQuery = "SELECT 1"
    
    # Try connection with password first
    $passwordEscaped = $password -replace '"', '\"'
    $result = & $mysqlPath -h localhost -P $port -u root -p"$passwordEscaped" -e "$testQuery" 2>&1
    
    # If that fails, try without password (empty password)
    if ($LASTEXITCODE -ne 0 -or $result -match "ERROR" -or $result -match "Access denied") {
        Write-Host "Trying connection without password..." -ForegroundColor Gray
        $result = & $mysqlPath -h localhost -P $port -u root -e "$testQuery" 2>&1
    }
    
    if ($LASTEXITCODE -eq 0 -or ($result -match "1" -and $result -notmatch "ERROR" -and $result -notmatch "Access denied")) {
        Write-Host "MySQL password validated successfully!" -ForegroundColor Green
        exit 0
    }
    else {
        Write-Host "ERROR: Invalid MySQL password" -ForegroundColor Red
        Write-Host "Error: $result" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "ERROR: Failed to validate MySQL password: $_" -ForegroundColor Red
    exit 1
}

