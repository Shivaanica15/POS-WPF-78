# ============================================================================
# DetectMySQLPort.ps1
# Detects MySQL port from running MySQL instance
# ============================================================================

param(
    [Parameter(Mandatory=$true)]
    [string]$OutputFile
)

Write-Host "Detecting MySQL port from running instance..." -ForegroundColor Cyan

$detectedPort = $null

# Method 1: Check MySQL service and get port from my.ini
try {
    $mysqlServices = Get-Service -Name "MySQL*" -ErrorAction SilentlyContinue
    if ($mysqlServices) {
        foreach ($service in $mysqlServices) {
            $servicePath = (Get-WmiObject Win32_Service -Filter "Name='$($service.Name)'").PathName
            if ($servicePath) {
                $servicePath = $servicePath.Replace('"', '')
                $mysqlDir = Split-Path (Split-Path $servicePath)
                $myIniPath = Join-Path $mysqlDir "my.ini"
                
                if (Test-Path $myIniPath) {
                    $content = Get-Content $myIniPath -Raw
                    if ($content -match '(?m)\[mysqld\]\s*port\s*=\s*(\d+)') {
                        $detectedPort = $matches[1]
                        Write-Host "Found MySQL port from service: $detectedPort" -ForegroundColor Green
                        break
                    }
                }
            }
        }
    }
}
catch {
    # Ignore errors
}

# Method 2: Check netstat for MySQL connections
if ($null -eq $detectedPort) {
    try {
        $netstat = netstat -an | Select-String "LISTENING" | Select-String ":330"
        foreach ($line in $netstat) {
            if ($line -match ':(\d+)\s') {
                $port = $matches[1]
                $portInt = [int]$port
                if ($portInt -ge 3306 -and $portInt -le 3315) {
                    # Try to connect to verify it's MySQL
                    try {
                        $tcpClient = New-Object System.Net.Sockets.TcpClient
                        $tcpClient.Connect("localhost", $portInt)
                        $tcpClient.Close()
                        $detectedPort = $port
                        Write-Host "Found MySQL port from netstat: $detectedPort" -ForegroundColor Green
                        break
                    }
                    catch {
                        # Not MySQL, continue
                    }
                }
            }
        }
    }
    catch {
        # Ignore errors
    }
}

# Method 3: Try common ports
if ($null -eq $detectedPort) {
    $commonPorts = @(3306, 3307, 3308, 3309, 3310)
    foreach ($port in $commonPorts) {
        try {
            $tcpClient = New-Object System.Net.Sockets.TcpClient
            $tcpClient.Connect("localhost", $port)
            $tcpClient.Close()
            $detectedPort = $port.ToString()
            Write-Host "Found MySQL port by testing: $detectedPort" -ForegroundColor Green
            break
        }
        catch {
            # Port not available, continue
        }
    }
}

# Fallback to default
if ($null -eq $detectedPort) {
    $detectedPort = "3306"
    Write-Host "Using default MySQL port: $detectedPort" -ForegroundColor Yellow
}

# Write port to file
try {
    Set-Content -Path $OutputFile -Value $detectedPort -ErrorAction Stop
    Write-Host "MySQL port detected: $detectedPort" -ForegroundColor Green
    exit 0
}
catch {
    Write-Host "ERROR: Failed to write port file: $_" -ForegroundColor Red
    exit 1
}

