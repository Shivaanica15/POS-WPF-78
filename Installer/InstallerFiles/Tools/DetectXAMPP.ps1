# ============================================================================
# DetectXAMPP.ps1
# Detects existing XAMPP installations on the system
# ============================================================================

param(
    [string]$OutputFile = "$env:TEMP\xampp_info.txt"
)

Write-Host "Detecting XAMPP installations..." -ForegroundColor Cyan

$xamppPaths = @()

# Common XAMPP installation paths
$commonPaths = @(
    "C:\xampp",
    "C:\Program Files\xampp",
    "C:\Program Files (x86)\xampp",
    "$env:ProgramFiles\xampp",
    "${env:ProgramFiles(x86)}\xampp"
)

# Also check for XAMPP in common portable locations
$portablePaths = @(
    "$env:USERPROFILE\Desktop\xampp",
    "$env:USERPROFILE\Documents\xampp",
    "D:\xampp",
    "E:\xampp"
)

$allPaths = $commonPaths + $portablePaths

foreach ($path in $allPaths) {
    if (Test-Path $path) {
        $mysqlPath = Join-Path $path "mysql\bin\mysqld.exe"
        if (Test-Path $mysqlPath) {
            Write-Host "Found XAMPP at: $path" -ForegroundColor Green
            $xamppPaths += $path
        }
    }
}

# Check for MySQL service
try {
    $mysqlService = Get-Service -Name "MySQL*" -ErrorAction SilentlyContinue
    if ($mysqlService) {
        Write-Host "Found MySQL service: $($mysqlService.Name)" -ForegroundColor Green
        # Try to find MySQL installation path from service
        $servicePath = (Get-WmiObject Win32_Service -Filter "Name='$($mysqlService.Name)'").PathName
        if ($servicePath) {
            $mysqlDir = Split-Path (Split-Path $servicePath.Replace('"', ''))
            $xamppDir = Split-Path $mysqlDir
            if (Test-Path $xamppDir) {
                if ($xamppPaths -notcontains $xamppDir) {
                    Write-Host "Found XAMPP via MySQL service at: $xamppDir" -ForegroundColor Green
                    $xamppPaths += $xamppDir
                }
            }
        }
    }
}
catch {
    # Ignore errors
}

# Write results to file
if ($xamppPaths.Count -gt 0) {
    $xamppPaths -join "|" | Set-Content -Path $OutputFile -ErrorAction Stop
    Write-Host "Found $($xamppPaths.Count) XAMPP installation(s)" -ForegroundColor Green
    exit 0
}
else {
    "" | Set-Content -Path $OutputFile -ErrorAction Stop
    Write-Host "No XAMPP installations found" -ForegroundColor Yellow
    exit 0
}

