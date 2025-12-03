# ============================================================================
# SetMySQLPassword.ps1
# Sets MySQL root password for bundled XAMPP installation
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$XamppPath,
    
    [Parameter(Mandatory=$true)]
    [string]$Password
)

Write-Host "Setting MySQL root password..." -ForegroundColor Cyan

$mysqlPath = Join-Path $XamppPath "mysql\bin\mysql.exe"
$mysqldPath = Join-Path $XamppPath "mysql\bin\mysqld.exe"

if (-not (Test-Path $mysqlPath)) {
    Write-Host "ERROR: MySQL client not found at: $mysqlPath" -ForegroundColor Red
    exit 1
}

try {
    # Wait a moment for MySQL to be ready
    Start-Sleep -Seconds 3
    
    # Set password using mysqladmin or mysql
    $mysqladminPath = Join-Path $XamppPath "mysql\bin\mysqladmin.exe"
    
    if (Test-Path $mysqladminPath) {
        # Try mysqladmin first
        $result = & $mysqladminPath -u root password "$Password" 2>&1
        if ($LASTEXITCODE -eq 0) {
            Write-Host "MySQL root password set successfully!" -ForegroundColor Green
            exit 0
        }
    }
    
    # Fallback: Use mysql client
    $passwordEscaped = $Password -replace '"', '\"'
    $setPasswordQuery = "ALTER USER 'root'@'localhost' IDENTIFIED BY '$passwordEscaped'; FLUSH PRIVILEGES;"
    
    $result = & $mysqlPath -u root -e "$setPasswordQuery" 2>&1
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "MySQL root password set successfully!" -ForegroundColor Green
        exit 0
    }
    else {
        Write-Host "WARNING: Could not set password automatically. Password may need to be set manually." -ForegroundColor Yellow
        Write-Host "Error: $result" -ForegroundColor Gray
        exit 0  # Don't fail installation
    }
}
catch {
    Write-Host "WARNING: Failed to set MySQL password: $_" -ForegroundColor Yellow
    Write-Host "Password may need to be set manually." -ForegroundColor Yellow
    exit 0  # Don't fail installation
}
