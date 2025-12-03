# ============================================================================
# ConfigureMySQL.ps1
# Configures MySQL my.ini file with detected port and correct paths
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$XamppPath,
    
    [Parameter(Mandatory=$true)]
    [string]$PortFile
)

Write-Host "Configuring MySQL..." -ForegroundColor Cyan

# Read port from file
if (-not (Test-Path $PortFile)) {
    Write-Host "ERROR: Port file not found: $PortFile" -ForegroundColor Red
    exit 1
}

$port = Get-Content $PortFile -ErrorAction Stop
Write-Host "Using MySQL port: $port" -ForegroundColor Green

# Paths
$myIniPath = Join-Path $XamppPath "mysql\bin\my.ini"
$mysqlBaseDir = Join-Path $XamppPath "mysql"
$mysqlDataDir = Join-Path $mysqlBaseDir "data"

# Normalize paths (escape backslashes for regex)
$mysqlBaseDirEscaped = [regex]::Escape($mysqlBaseDir)
$mysqlDataDirEscaped = [regex]::Escape($mysqlDataDir)

if (-not (Test-Path $myIniPath)) {
    Write-Host "ERROR: my.ini not found at: $myIniPath" -ForegroundColor Red
    exit 1
}

Write-Host "Reading my.ini from: $myIniPath" -ForegroundColor Gray

# Read file content
$content = Get-Content $myIniPath -Raw -Encoding UTF8

# Update port in [client] section
$content = $content -replace '(?m)^(\[client\]\s*port\s*=\s*)\d+', "`${1}$port"

# Update port in [mysqld] section
$content = $content -replace '(?m)^(\[mysqld\]\s*port\s*=\s*)\d+', "`${1}$port"

# Update basedir if present
if ($content -match 'basedir') {
    $content = $content -replace '(?m)^(\[mysqld\]\s*.*?basedir\s*=\s*)[^\r\n]+', "`${1}$mysqlBaseDir"
}
else {
    # Add basedir after [mysqld] if not present
    $content = $content -replace '(?m)(\[mysqld\])', "`${1}`r`nbasedir=$mysqlBaseDir"
}

# Update datadir if present
if ($content -match 'datadir') {
    $content = $content -replace '(?m)^(\[mysqld\]\s*.*?datadir\s*=\s*)[^\r\n]+', "`${1}$mysqlDataDir"
}
else {
    # Add datadir after [mysqld] if not present
    $content = $content -replace '(?m)(\[mysqld\])', "`${1}`r`ndatadir=$mysqlDataDir"
}

# Ensure port is set in [mysqld] section (add if missing)
if ($content -notmatch '\[mysqld\][\s\S]*?port\s*=') {
    $content = $content -replace '(?m)(\[mysqld\])', "`${1}`r`nport=$port"
}

# Ensure port is set in [client] section (add if missing)
if ($content -notmatch '\[client\][\s\S]*?port\s*=') {
    $content = $content -replace '(?m)(\[client\])', "`${1}`r`nport=$port"
}

# Write updated content back
try {
    # Create backup
    $backupPath = "$myIniPath.backup"
    Copy-Item $myIniPath $backupPath -Force -ErrorAction Stop
    
    # Write updated content
    [System.IO.File]::WriteAllText($myIniPath, $content, [System.Text.Encoding]::UTF8)
    
    Write-Host "MySQL configuration updated successfully!" -ForegroundColor Green
    Write-Host "  Port: $port" -ForegroundColor Gray
    Write-Host "  BaseDir: $mysqlBaseDir" -ForegroundColor Gray
    Write-Host "  DataDir: $mysqlDataDir" -ForegroundColor Gray
    Write-Host "  Backup saved to: $backupPath" -ForegroundColor Gray
    
    exit 0
}
catch {
    Write-Host "ERROR: Failed to update my.ini: $_" -ForegroundColor Red
    # Restore backup if write failed
    if (Test-Path $backupPath) {
        Copy-Item $backupPath $myIniPath -Force
    }
    exit 1
}

