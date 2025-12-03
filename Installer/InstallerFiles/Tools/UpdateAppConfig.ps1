# ============================================================================
# UpdateAppConfig.ps1
# Updates App.config connection string with detected MySQL port
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$AppConfigPath,
    
    [Parameter(Mandatory=$true)]
    [string]$PortFile,
    
    [string]$PasswordFile = ""
)

Write-Host "Updating App.config..." -ForegroundColor Cyan

# Read port from file
if (-not (Test-Path $PortFile)) {
    Write-Host "ERROR: Port file not found: $PortFile" -ForegroundColor Red
    exit 1
}

$port = Get-Content $PortFile -ErrorAction Stop
Write-Host "Updating connection string with port: $port" -ForegroundColor Green

# Read password if provided
$password = ""
if (-not [string]::IsNullOrWhiteSpace($PasswordFile) -and (Test-Path $PasswordFile)) {
    $password = Get-Content $PasswordFile -ErrorAction SilentlyContinue
    if ($password) {
        Write-Host "Updating connection string with password" -ForegroundColor Green
    }
}

if (-not (Test-Path $AppConfigPath)) {
    Write-Host "ERROR: App.config not found at: $AppConfigPath" -ForegroundColor Red
    exit 1
}

# Load XML document
try {
    [xml]$xml = Get-Content $AppConfigPath -Encoding UTF8
    
    # Find connection string
    $connectionString = $xml.SelectSingleNode("//connectionStrings/add[@name='FinalPOS.Properties.Settings.NewOneConnectionString']")
    
    if ($null -eq $connectionString) {
        Write-Host "ERROR: Connection string not found in App.config" -ForegroundColor Red
        exit 1
    }
    
    # Parse existing connection string
    $oldConnString = $connectionString.connectionString
    Write-Host "Old connection string: $oldConnString" -ForegroundColor Gray
    
    # Parse connection string parts
    $parts = $oldConnString -split ';'
    $newParts = @()
    $hasPort = $false
    
    $hasPassword = $false
    foreach ($part in $parts) {
        $part = $part.Trim()
        if ([string]::IsNullOrWhiteSpace($part)) {
            continue
        }
        
        if ($part -match '^Port\s*=', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase) {
            # Replace existing port
            $newParts += "Port=$port"
            $hasPort = $true
        }
        elseif ($part -match '^Pwd\s*=', [System.Text.RegularExpressions.RegexOptions]::IgnoreCase) {
            # Always replace password (even if empty)
            $newParts += "Pwd=$password"
            $hasPassword = $true
        }
        elseif ($part -match '^Server\s*=') {
            # Keep server, add port after it
            $newParts += $part
            if (-not $hasPort) {
                $newParts += "Port=$port"
                $hasPort = $true
            }
        }
        else {
            $newParts += $part
        }
    }
    
    # Add password if not found (even if empty)
    if (-not $hasPassword) {
        $newParts += "Pwd=$password"
    }
    
    # Add port if not found
    if (-not $hasPort) {
        # Insert port after Server
        $serverIndex = -1
        for ($i = 0; $i -lt $newParts.Length; $i++) {
            if ($newParts[$i] -match '^Server\s*=') {
                $serverIndex = $i
                break
            }
        }
        
        if ($serverIndex -ge 0) {
            $newParts = $newParts[0..$serverIndex] + "Port=$port" + $newParts[($serverIndex+1)..($newParts.Length-1)]
        }
        else {
            # Add at beginning if server not found
            $newParts = @("Port=$port") + $newParts
        }
    }
    
    # Build new connection string
    $newConnString = $newParts -join ';'
    Write-Host "New connection string: $newConnString" -ForegroundColor Gray
    
    # Update XML
    $connectionString.connectionString = $newConnString
    
    # Create backup
    $backupPath = "$AppConfigPath.backup"
    Copy-Item $AppConfigPath $backupPath -Force -ErrorAction Stop
    
    # Save XML
    $xml.Save($AppConfigPath)
    
    Write-Host "App.config updated successfully!" -ForegroundColor Green
    Write-Host "  Backup saved to: $backupPath" -ForegroundColor Gray
    
    exit 0
}
catch {
    Write-Host "ERROR: Failed to update App.config: $_" -ForegroundColor Red
    Write-Host "Stack trace: $($_.Exception.StackTrace)" -ForegroundColor Red
    
    # Restore backup if exists
    $backupPath = "$AppConfigPath.backup"
    if (Test-Path $backupPath) {
        Copy-Item $backupPath $AppConfigPath -Force
        Write-Host "Restored backup." -ForegroundColor Yellow
    }
    
    exit 1
}

