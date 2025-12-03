# ============================================================================
# PrepareInstallerFiles.ps1
# Script to prepare InstallerFiles folder for FinalPOS installer
# ============================================================================

param(
    [switch]$CopyReleaseFiles,
    [switch]$CreateFolders
)

$ErrorActionPreference = "Stop"
$basePath = "C:\Users\shiva\Documents\GitHub\POS-WPF-78"
$installerFilesPath = Join-Path $basePath "Installer\InstallerFiles"
$appPath = Join-Path $installerFilesPath "App"
$xamppPath = Join-Path $installerFilesPath "XAMPP"
$toolsPath = Join-Path $installerFilesPath "Tools"
$releasePath = Join-Path $basePath "FinalPOS\bin\Release"

Write-Host "===========================================" -ForegroundColor Cyan
Write-Host "FinalPOS Installer Files Preparation" -ForegroundColor Cyan
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host ""

# Create folder structure
Write-Host "Creating folder structure..." -ForegroundColor Yellow
New-Item -ItemType Directory -Force -Path $appPath | Out-Null
New-Item -ItemType Directory -Force -Path $xamppPath | Out-Null
New-Item -ItemType Directory -Force -Path $toolsPath | Out-Null
Write-Host "✓ Folders created" -ForegroundColor Green
Write-Host ""

# Check if Release build exists
if (Test-Path $releasePath) {
    $releaseFiles = Get-ChildItem -Path $releasePath -File
    if ($releaseFiles.Count -gt 0) {
        Write-Host "Release build found with $($releaseFiles.Count) files" -ForegroundColor Green
        
        if ($CopyReleaseFiles) {
            Write-Host "Copying Release files to App folder..." -ForegroundColor Yellow
            Copy-Item -Path "$releasePath\*" -Destination $appPath -Recurse -Force
            Write-Host "✓ Files copied" -ForegroundColor Green
        } else {
            Write-Host "Release files NOT copied (use -CopyReleaseFiles to copy)" -ForegroundColor Yellow
        }
    } else {
        Write-Host "⚠ Release folder exists but is empty" -ForegroundColor Yellow
        Write-Host "  Build FinalPOS in Release mode first!" -ForegroundColor Yellow
    }
} else {
    Write-Host "⚠ Release build not found at: $releasePath" -ForegroundColor Yellow
    Write-Host "  Build FinalPOS in Release mode first!" -ForegroundColor Yellow
}

Write-Host ""

# Check PowerShell scripts
Write-Host "Checking PowerShell scripts..." -ForegroundColor Yellow
$requiredScripts = @(
    "FindAvailablePort.ps1",
    "ConfigureMySQL.ps1",
    "UpdateAppConfig.ps1",
    "StartMySQL.ps1"
)

$allScriptsExist = $true
foreach ($script in $requiredScripts) {
    $scriptPath = Join-Path $toolsPath $script
    if (Test-Path $scriptPath) {
        Write-Host "  ✓ $script" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $script - MISSING!" -ForegroundColor Red
        $allScriptsExist = $false
    }
}

Write-Host ""

# Summary
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host "SUMMARY" -ForegroundColor Cyan
Write-Host "===========================================" -ForegroundColor Cyan
Write-Host ""

# Check App folder
$appFiles = Get-ChildItem -Path $appPath -File -ErrorAction SilentlyContinue
if ($appFiles.Count -gt 0) {
    Write-Host "✓ App folder: $($appFiles.Count) files" -ForegroundColor Green
} else {
    Write-Host "✗ App folder: EMPTY (copy Release build files here)" -ForegroundColor Red
}

# Check XAMPP folder
$xamppFiles = Get-ChildItem -Path $xamppPath -Recurse -ErrorAction SilentlyContinue
if ($xamppFiles.Count -gt 0) {
    Write-Host "✓ XAMPP folder: $($xamppFiles.Count) files/folders" -ForegroundColor Green
} else {
    Write-Host "✗ XAMPP folder: EMPTY (extract XAMPP Portable here)" -ForegroundColor Red
}

# Check Tools folder
$toolsFiles = Get-ChildItem -Path $toolsPath -File -ErrorAction SilentlyContinue
if ($toolsFiles.Count -eq 4) {
    Write-Host "✓ Tools folder: All scripts present" -ForegroundColor Green
} else {
    Write-Host "⚠ Tools folder: $($toolsFiles.Count)/4 scripts found" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "NEXT STEPS:" -ForegroundColor Cyan
Write-Host "1. Build FinalPOS in Release mode" -ForegroundColor White
Write-Host "2. Run: .\PrepareInstallerFiles.ps1 -CopyReleaseFiles" -ForegroundColor White
Write-Host "3. Extract XAMPP Portable to: $xamppPath" -ForegroundColor White
Write-Host "4. Open FinalPOS_Installer.iss in Inno Setup" -ForegroundColor White
Write-Host "5. Compile installer" -ForegroundColor White
Write-Host ""

