# ============================================================================
# TestMySQLConnection.ps1
# Tests MySQL connection with provided credentials
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$Server,
    
    [Parameter(Mandatory=$true)]
    [int]$Port,
    
    [Parameter(Mandatory=$true)]
    [string]$Username,
    
    [Parameter(Mandatory=$true)]
    [string]$Password
)

Write-Host "Testing MySQL connection..." -ForegroundColor Cyan
Write-Host "  Server: $Server" -ForegroundColor Gray
Write-Host "  Port: $Port" -ForegroundColor Gray
Write-Host "  Username: $Username" -ForegroundColor Gray

try {
    # Try to connect using MySQL command line client
    $mysqlPath = "mysql.exe"
    
    # Check if mysql.exe is in PATH
    $mysqlFound = $false
    $env:Path -split ';' | ForEach-Object {
        $testPath = Join-Path $_ "mysql.exe"
        if (Test-Path $testPath) {
            $mysqlPath = $testPath
            $mysqlFound = $true
        }
    }
    
    if (-not $mysqlFound) {
        # Try common XAMPP locations
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
        Write-Host "ERROR: MySQL client not found. Cannot test connection." -ForegroundColor Red
        exit 1
    }
    
    # Test connection
    $passwordEscaped = $Password -replace '"', '\"'
    $testQuery = "SELECT 1"
    
    $result = & $mysqlPath -h $Server -P $Port -u $Username -p"$passwordEscaped" -e "$testQuery" 2>&1
    
    if ($LASTEXITCODE -eq 0 -or $result -match "1") {
        Write-Host "MySQL connection successful!" -ForegroundColor Green
        exit 0
    }
    else {
        Write-Host "ERROR: MySQL connection failed" -ForegroundColor Red
        Write-Host "Error: $result" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "ERROR: Failed to test MySQL connection: $_" -ForegroundColor Red
    exit 1
}

