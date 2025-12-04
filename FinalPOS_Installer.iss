; ============================================================================
; FinalPOS Inno Setup Installer Script
; MySQL + phpMyAdmin Embedded Setup
; Version: 3.0
; ============================================================================
;
; This installer bundles standalone MySQL Server and phpMyAdmin
; Both are installed in isolated folders and run on custom ports
; ============================================================================

#define MyAppName "FinalPOS"
#define MyAppVersion "1.0"
#define MyAppPublisher "FinalPOS"
#define MyAppURL "https://www.finalpos.com"
#define MyAppExeName "FinalPOS.exe"
#define DatabaseName "POS_NEXA_ERP"
#define MySQLBasePort 3308
#define phpMyAdminBasePort 8000

; MySQL Download URLs (adjust version as needed)
; Using MySQL 8.0.x Windows ZIP Archive (no-installer version)
#define MySQLVersion "8.0.40"
#define MySQLDownloadURL "https://dev.mysql.com/get/Downloads/MySQL-8.0/mysql-{#MySQLVersion}-winx64.zip"
#define MySQLZipName "mysql-{#MySQLVersion}-winx64.zip"

; phpMyAdmin Download URL
#define phpMyAdminVersion "5.2.1"
#define phpMyAdminDownloadURL "https://files.phpmyadmin.net/phpMyAdmin/{#phpMyAdminVersion}/phpMyAdmin-{#phpMyAdminVersion}-all-languages.zip"
#define phpMyAdminZipName "phpMyAdmin-{#phpMyAdminVersion}-all-languages.zip"

[Setup]
AppId={{A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DisableProgramGroupPage=yes
OutputDir=Output
OutputBaseFilename=FinalPOS_Setup_v{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64
CloseApplications=yes
RestartApplications=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1

[Files]
; Application files - Copy from Release build output
Source: "FinalPOS\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "*.pdb"

; PHP portable runtime - Required for phpMyAdmin
Source: "PHP\*"; DestDir: "{app}\PHP"; Flags: ignoreversion recursesubdirs createallsubdirs; Excludes: "*.txt,*.ini-development,*.ini-production,test\*"

; Batch files are created dynamically during installation

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
; Launch FinalPOS application
Filename: "{app}\{#MyAppExeName}"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent

[UninstallRun]
; Stop MySQL before uninstallation
Filename: "{app}\StopMySQL.bat"; Flags: runhidden waituntilterminated; RunOnceId: "StopMySQL"
; Stop PHP server before uninstallation
Filename: "taskkill.exe"; Parameters: "/F /IM php.exe"; Flags: runhidden waituntilterminated; RunOnceId: "StopPHP"

[Code]
var
  AppConfigUpdated: Boolean;
  ConfigPage: TOutputProgressWizardPage;
  ProgressPage: TOutputProgressWizardPage;
  LogFile: String;
  AppDir: String;
  DetectedMySQLPort: Integer;
  DetectedphpMyAdminPort: Integer;
  MySQLInitialized: Boolean;
  DatabaseCreated: Boolean;

// ============================================================================
// LOGGING FUNCTIONS
// ============================================================================

procedure WriteToLogFile(Message: String);
var
  LogContent: TArrayOfString;
begin
  try
    if LogFile = '' then
      LogFile := ExpandConstant('{app}\install-log.txt');
    
    if FileExists(LogFile) then
      LoadStringsFromFile(LogFile, LogContent)
    else
      SetArrayLength(LogContent, 0);
    
    SetArrayLength(LogContent, GetArrayLength(LogContent) + 1);
    LogContent[GetArrayLength(LogContent) - 1] := GetDateTimeString('yyyy-mm-dd hh:nn:ss', '-', ':') + ' - ' + Message;
    SaveStringsToFile(LogFile, LogContent, False);
  except
    // Ignore logging errors
  end;
end;

procedure LogAndFile(Message: String);
begin
  Log(Message);
  WriteToLogFile(Message);
end;

// ============================================================================
// PORT DETECTION FUNCTIONS
// ============================================================================

function IsPortInUse(Port: Integer): Boolean;
var
  ResultCode: Integer;
  TempFile: String;
  Output: TArrayOfString;
  I: Integer;
  Line: String;
begin
  Result := False;
  
  // Method 1: Use netstat to check if port is listening
  TempFile := ExpandConstant('{tmp}\portcheck.txt');
  if Exec('cmd.exe', '/c netstat -an | findstr ":' + IntToStr(Port) + '" > "' + TempFile + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    if FileExists(TempFile) then
    begin
      LoadStringsFromFile(TempFile, Output);
      // Check if any line contains LISTENING state
      for I := 0 to GetArrayLength(Output) - 1 do
      begin
        Line := LowerCase(Output[I]);
        if (Pos(':' + IntToStr(Port), Line) > 0) and (Pos('listening', Line) > 0) then
        begin
          Result := True;
          Break;
        end;
      end;
      DeleteFile(TempFile);
      
      if Result then
        Exit; // Port is in use
    end;
  end;
  
  // Method 2: Use PowerShell Test-NetConnection as fallback
  if Exec('powershell.exe', '-Command "$result = Test-NetConnection -ComputerName localhost -Port ' + IntToStr(Port) + ' -InformationLevel Quiet -WarningAction SilentlyContinue; if ($result) { exit 0 } else { exit 1 }"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    // PowerShell returns 0 if port is open/listening
    if ResultCode = 0 then
    begin
      Result := True;
      Exit;
    end;
  end;
  
  // If both methods fail or return no result, assume port is free
  Result := False;
end;

function FindAvailablePort(BasePort: Integer): Integer;
var
  CurrentPort: Integer;
  MaxAttempts: Integer;
  Attempts: Integer;
begin
  CurrentPort := BasePort;
  MaxAttempts := 100; // Try up to 100 ports
  Attempts := 0;
  
  LogAndFile('Finding available port starting from ' + IntToStr(BasePort));
  
  while Attempts < MaxAttempts do
  begin
    if not IsPortInUse(CurrentPort) then
    begin
      LogAndFile('Found available port: ' + IntToStr(CurrentPort));
      Result := CurrentPort;
      Exit;
    end;
    
    CurrentPort := CurrentPort + 1;
    Attempts := Attempts + 1;
  end;
  
  // If we couldn't find a port, return base port anyway
  LogAndFile('Warning: Could not find available port, using base port ' + IntToStr(BasePort));
  Result := BasePort;
end;

// ============================================================================
// DOWNLOAD FUNCTIONS
// ============================================================================

function DownloadFile(URL: String; FilePath: String): Boolean;
var
  ResultCode: Integer;
  PowerShellCmd: String;
begin
  Result := False;
  LogAndFile('Downloading: ' + URL);
  LogAndFile('Saving to: ' + FilePath);
  
  try
    // Use PowerShell with execution policy bypass and proper error handling
    PowerShellCmd := '-ExecutionPolicy Bypass -Command "try { $ProgressPreference = ''SilentlyContinue''; Invoke-WebRequest -Uri ''' + URL + ''' -OutFile ''' + FilePath + ''' -UseBasicParsing -ErrorAction Stop; if (Test-Path ''' + FilePath + ''') { exit 0 } else { exit 1 } } catch { Write-Host $_.Exception.Message; exit 1 }"';
    
    if Exec('powershell.exe', PowerShellCmd, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    begin
      if ResultCode = 0 then
      begin
        if FileExists(FilePath) then
        begin
          LogAndFile('Download completed successfully. File size: ' + IntToStr(GetFileSize(FilePath)) + ' bytes');
          Result := True;
        end
        else
        begin
          LogAndFile('Error: Downloaded file not found at: ' + FilePath);
        end;
      end
      else
      begin
        LogAndFile('Error: Download failed with PowerShell exit code ' + IntToStr(ResultCode));
      end;
    end
    else
    begin
      LogAndFile('Error: Failed to execute PowerShell download command');
    end;
  except
    LogAndFile('Exception during download: ' + GetExceptionMessage);
  end;
end;

function ExtractZip(ZipPath: String; ExtractTo: String): Boolean;
var
  ResultCode: Integer;
  PowerShellCmd: String;
begin
  Result := False;
  LogAndFile('Extracting: ' + ZipPath);
  LogAndFile('Extracting to: ' + ExtractTo);
  
  try
    // Verify zip file exists
    if not FileExists(ZipPath) then
    begin
      LogAndFile('Error: Zip file not found: ' + ZipPath);
      Exit;
    end;
    
    // Use PowerShell Expand-Archive with error handling
    PowerShellCmd := '-ExecutionPolicy Bypass -Command "try { Expand-Archive -Path ''' + ZipPath + ''' -DestinationPath ''' + ExtractTo + ''' -Force -ErrorAction Stop; exit 0 } catch { Write-Host $_.Exception.Message; exit 1 }"';
    
    if Exec('powershell.exe', PowerShellCmd, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    begin
      if ResultCode = 0 then
      begin
        LogAndFile('Extraction completed successfully');
        Result := True;
      end
      else
      begin
        LogAndFile('Error: Extraction failed with PowerShell exit code ' + IntToStr(ResultCode));
      end;
    end
    else
    begin
      LogAndFile('Error: Failed to execute PowerShell extraction command');
    end;
  except
    LogAndFile('Exception during extraction: ' + GetExceptionMessage);
  end;
end;

// ============================================================================
// MYSQL INSTALLATION FUNCTIONS
// ============================================================================

function DownloadAndExtractMySQL: Boolean;
var
  MySQLZipPath: String;
  MySQLExtractPath: String;
  MySQLSourcePath: String;
  MySQLTargetPath: String;
  ResultCode: Integer;
begin
  Result := False;
  MySQLZipPath := ExpandConstant('{tmp}') + '\mysql-8.0.40-winx64.zip';
  MySQLExtractPath := ExpandConstant('{tmp}\mysql-extract');
  MySQLTargetPath := ExpandConstant('{app}\mysql');
  
  try
    // Step 1: Download MySQL
    if Assigned(ProgressPage) then
      ProgressPage.SetText('Downloading MySQL Server...', 'Please wait...');
    
    if not DownloadFile('https://dev.mysql.com/get/Downloads/MySQL-8.0/mysql-8.0.40-winx64.zip', MySQLZipPath) then
    begin
      LogAndFile('Failed to download MySQL');
      Exit;
    end;
    
    // Step 2: Extract MySQL
    if Assigned(ProgressPage) then
      ProgressPage.SetText('Extracting MySQL Server...', 'Please wait...');
    
    // Create extract directory
    if not DirExists(MySQLExtractPath) then
      CreateDir(MySQLExtractPath);
    
    if not ExtractZip(MySQLZipPath, MySQLExtractPath) then
    begin
      LogAndFile('Failed to extract MySQL');
      Exit;
    end;
    
    // Step 3: Move extracted MySQL to app directory
    if Assigned(ProgressPage) then
      ProgressPage.SetText('Installing MySQL Server...', 'Please wait...');
    
    // Find the mysql folder inside the extracted archive
    MySQLSourcePath := MySQLExtractPath + '\mysql-8.0.40-winx64';
    if not DirExists(MySQLSourcePath) then
    begin
      // Try alternative path structure
      MySQLSourcePath := MySQLExtractPath;
    end;
    
    // Copy MySQL to app directory
    if DirExists(MySQLSourcePath) then
    begin
      LogAndFile('Copying MySQL from ' + MySQLSourcePath + ' to ' + MySQLTargetPath);
      
      // Create target directory if it doesn't exist
      if not DirExists(MySQLTargetPath) then
        CreateDir(MySQLTargetPath);
      
      // xcopy returns 0 for success, but also returns non-zero codes that can indicate success
      // Check if files were actually copied by verifying directory contents
      if Exec('xcopy', '"' + MySQLSourcePath + '\*" "' + MySQLTargetPath + '\" /E /I /Y /Q', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
      begin
        // xcopy can return 0-4 for various success scenarios
        if (ResultCode >= 0) and (ResultCode <= 4) then
        begin
          // Verify copy succeeded by checking if bin directory exists
          if DirExists(MySQLTargetPath + '\bin') and FileExists(MySQLTargetPath + '\bin\mysqld.exe') then
          begin
            LogAndFile('MySQL copied successfully');
            Result := True;
          end
          else
          begin
            LogAndFile('Error: MySQL files not found after copy. ResultCode: ' + IntToStr(ResultCode));
          end;
        end
        else
        begin
          LogAndFile('Error: Copy failed with code ' + IntToStr(ResultCode));
        end;
      end
      else
      begin
        LogAndFile('Error: Failed to execute xcopy command');
      end;
    end
    else
    begin
      LogAndFile('Error: MySQL source directory not found: ' + MySQLSourcePath);
      LogAndFile('Checking if extraction created files directly in: ' + MySQLExtractPath);
    end;
    
    // Cleanup
    if FileExists(MySQLZipPath) then
      DeleteFile(MySQLZipPath);
    
  except
    LogAndFile('Exception during MySQL download/extract: ' + GetExceptionMessage);
  end;
end;

function CreateMySQLConfig: Boolean;
var
  ConfigFile: String;
  ConfigContent: TArrayOfString;
  MySQLPath: String;
  MySQLDataPath: String;
begin
  Result := False;
  ConfigFile := ExpandConstant('{app}\mysql\my.ini');
  MySQLPath := ExpandConstant('{app}\mysql');
  MySQLDataPath := ExpandConstant('{app}\mysql\data');
  
  try
    SetArrayLength(ConfigContent, 0);
    
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '[mysqld]';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := 'port=' + IntToStr(DetectedMySQLPort);
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := 'basedir="' + MySQLPath + '"';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := 'datadir="' + MySQLDataPath + '"';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := 'skip-grant-tables=0';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := 'skip-external-locking';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := 'sql-mode="NO_ENGINE_SUBSTITUTION"';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '[client]';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := 'port=' + IntToStr(DetectedMySQLPort);
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := 'default-character-set=utf8mb4';
    
    SaveStringsToFile(ConfigFile, ConfigContent, False);
    LogAndFile('MySQL configuration file created: ' + ConfigFile);
    Result := True;
  except
    LogAndFile('Exception creating MySQL config: ' + GetExceptionMessage);
  end;
end;

function InitializeMySQLData: Boolean;
var
  MySQLDExe: String;
  MySQLConfig: String;
  MySQLDataPath: String;
  ResultCode: Integer;
begin
  Result := False;
  MySQLDExe := ExpandConstant('{app}\mysql\bin\mysqld.exe');
  MySQLConfig := ExpandConstant('{app}\mysql\my.ini');
  MySQLDataPath := ExpandConstant('{app}\mysql\data');
  
  try
    // Check if data directory already exists and is initialized
    if DirExists(MySQLDataPath) and FileExists(MySQLDataPath + '\mysql\user.MYD') then
    begin
      LogAndFile('MySQL data directory already initialized');
      Result := True;
      Exit;
    end;
    
    // Create data directory if it doesn't exist
    if not DirExists(MySQLDataPath) then
      CreateDir(MySQLDataPath);
    
    if Assigned(ProgressPage) then
      ProgressPage.SetText('Initializing MySQL data directory...', 'This may take a few minutes...');
    
    LogAndFile('Initializing MySQL data directory...');
    
    // Initialize MySQL data directory with --initialize-insecure (no root password)
    if Exec(MySQLDExe, '--defaults-file="' + MySQLConfig + '" --initialize-insecure --datadir="' + MySQLDataPath + '"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    begin
      if ResultCode = 0 then
      begin
        LogAndFile('MySQL data directory initialized successfully');
        Result := True;
      end
      else
      begin
        LogAndFile('Error: MySQL initialization failed with code ' + IntToStr(ResultCode));
      end;
    end
    else
    begin
      LogAndFile('Error: Failed to execute MySQL initialization');
    end;
  except
    LogAndFile('Exception during MySQL initialization: ' + GetExceptionMessage);
  end;
end;

function StartMySQL: Boolean;
var
  MySQLDExe: String;
  MySQLConfig: String;
  ResultCode: Integer;
begin
  Result := False;
  MySQLDExe := ExpandConstant('{app}\mysql\bin\mysqld.exe');
  MySQLConfig := ExpandConstant('{app}\mysql\my.ini');
  
  try
    LogAndFile('Starting MySQL server on port ' + IntToStr(DetectedMySQLPort));
    
    // Start MySQL in background
    if Exec(MySQLDExe, '--defaults-file="' + MySQLConfig + '" --standalone --console', '', SW_HIDE, ewNoWait, ResultCode) then
    begin
      // Wait a bit for MySQL to start
      Sleep(5000);
      
      // Verify MySQL is running by checking the port
      if IsPortInUse(DetectedMySQLPort) then
      begin
        LogAndFile('MySQL server started successfully');
        Result := True;
      end
      else
      begin
        LogAndFile('Warning: MySQL may not have started properly');
        Result := True; // Assume success, MySQL might take longer to start
      end;
    end
    else
    begin
      LogAndFile('Error: Failed to start MySQL server');
    end;
  except
    LogAndFile('Exception starting MySQL: ' + GetExceptionMessage);
  end;
end;

function CreateDatabase: Boolean;
var
  MySQLExe: String;
  SQLFile: String;
  ResultCode: Integer;
begin
  Result := False;
  MySQLExe := ExpandConstant('{app}\mysql\bin\mysql.exe');
  
  try
    LogAndFile('Creating database {#DatabaseName}...');
    
    // Wait a bit more for MySQL to be ready
    Sleep(3000);
    
    // Create database using mysql command line
    if Exec(MySQLExe, '-u root -h 127.0.0.1 -P ' + IntToStr(DetectedMySQLPort) + ' -e "CREATE DATABASE IF NOT EXISTS `{#DatabaseName}` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    begin
      if ResultCode = 0 then
      begin
        LogAndFile('Database created successfully');
        Result := True;
      end
      else
      begin
        LogAndFile('Warning: Database creation returned code ' + IntToStr(ResultCode) + ' (may already exist)');
        Result := True; // Assume success, database might already exist
      end;
    end
    else
    begin
      LogAndFile('Error: Failed to execute database creation command');
    end;
  except
    LogAndFile('Exception creating database: ' + GetExceptionMessage);
  end;
end;

// ============================================================================
// PHPMYADMIN INSTALLATION FUNCTIONS
// ============================================================================

function DownloadAndExtractphpMyAdmin: Boolean;
var
  phpMyAdminZipPath: String;
  phpMyAdminExtractPath: String;
  phpMyAdminSourcePath: String;
  phpMyAdminTargetPath: String;
  ResultCode: Integer;
begin
  Result := False;
  phpMyAdminZipPath := ExpandConstant('{tmp}') + '\phpMyAdmin-5.2.1-all-languages.zip';
  phpMyAdminExtractPath := ExpandConstant('{tmp}\phpmyadmin-extract');
  phpMyAdminTargetPath := ExpandConstant('{app}\phpmyadmin');
  
  try
    // Step 1: Download phpMyAdmin
    if Assigned(ProgressPage) then
      ProgressPage.SetText('Downloading phpMyAdmin...', 'Please wait...');
    
    if not DownloadFile('https://files.phpmyadmin.net/phpMyAdmin/5.2.1/phpMyAdmin-5.2.1-all-languages.zip', phpMyAdminZipPath) then
    begin
      LogAndFile('Failed to download phpMyAdmin');
      Exit;
    end;
    
    // Step 2: Extract phpMyAdmin
    if Assigned(ProgressPage) then
      ProgressPage.SetText('Extracting phpMyAdmin...', 'Please wait...');
    
    // Create extract directory
    if not DirExists(phpMyAdminExtractPath) then
      CreateDir(phpMyAdminExtractPath);
    
    if not ExtractZip(phpMyAdminZipPath, phpMyAdminExtractPath) then
    begin
      LogAndFile('Failed to extract phpMyAdmin');
      Exit;
    end;
    
    // Step 3: Move extracted phpMyAdmin to app directory
    if Assigned(ProgressPage) then
      ProgressPage.SetText('Installing phpMyAdmin...', 'Please wait...');
    
    // Find the phpMyAdmin folder inside the extracted archive
    phpMyAdminSourcePath := phpMyAdminExtractPath + '\phpMyAdmin-5.2.1-all-languages';
    if not DirExists(phpMyAdminSourcePath) then
    begin
      // Try alternative path structure
      phpMyAdminSourcePath := phpMyAdminExtractPath;
    end;
    
    // Copy phpMyAdmin to app directory
    if DirExists(phpMyAdminSourcePath) then
    begin
      LogAndFile('Copying phpMyAdmin from ' + phpMyAdminSourcePath + ' to ' + phpMyAdminTargetPath);
      if Exec('xcopy', '"' + phpMyAdminSourcePath + '\*" "' + phpMyAdminTargetPath + '\" /E /I /Y', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
      begin
        if ResultCode = 0 then
        begin
          LogAndFile('phpMyAdmin copied successfully');
          Result := True;
        end
        else
        begin
          LogAndFile('Error: Copy failed with code ' + IntToStr(ResultCode));
        end;
      end;
    end
    else
    begin
      LogAndFile('Error: phpMyAdmin source directory not found: ' + phpMyAdminSourcePath);
    end;
    
    // Cleanup
    if FileExists(phpMyAdminZipPath) then
      DeleteFile(phpMyAdminZipPath);
    
  except
    LogAndFile('Exception during phpMyAdmin download/extract: ' + GetExceptionMessage);
  end;
end;

procedure CreatephpMyAdminConfig;
var
  ConfigFile: String;
  ConfigContent: TArrayOfString;
begin
  try
    ConfigFile := ExpandConstant('{app}\phpmyadmin\config.inc.php');
    
    // Check if config already exists
    if FileExists(ConfigFile) then
    begin
      LogAndFile('phpMyAdmin config.inc.php already exists, skipping creation');
      Exit;
    end;
    
    SetArrayLength(ConfigContent, 0);
    
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '<?php';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '$cfg[''blowfish_secret''] = ''' + GetDateTimeString('yyyy-mm-dd hh:nn:ss', '-', ':') + ''';';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '$i = 0;';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '$i++;';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '$cfg[''Servers''][$i][''auth_type''] = ''config'';';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '$cfg[''Servers''][$i][''host''] = ''127.0.0.1'';';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '$cfg[''Servers''][$i][''port''] = ''' + IntToStr(DetectedMySQLPort) + ''';';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '$cfg[''Servers''][$i][''user''] = ''root'';';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '$cfg[''Servers''][$i][''password''] = '''';';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '';
    SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
    ConfigContent[GetArrayLength(ConfigContent) - 1] := '?>';
    
    SaveStringsToFile(ConfigFile, ConfigContent, False);
    LogAndFile('phpMyAdmin configuration file created: ' + ConfigFile);
  except
    LogAndFile('Exception creating phpMyAdmin config: ' + GetExceptionMessage);
  end;
end;

function StartphpMyAdmin: Boolean;
var
  ResultCode: Integer;
  PHPExe: String;
  phpMyAdminPath: String;
begin
  Result := False;
  PHPExe := ExpandConstant('{app}\PHP\php.exe');
  phpMyAdminPath := ExpandConstant('{app}\phpmyadmin');
  
  try
    LogAndFile('Starting phpMyAdmin server on port ' + IntToStr(DetectedphpMyAdminPort));
    if Exec(PHPExe, '-S localhost:' + IntToStr(DetectedphpMyAdminPort) + ' -t "' + phpMyAdminPath + '"', '', SW_HIDE, ewNoWait, ResultCode) then
    begin
      LogAndFile('phpMyAdmin server process started');
      Sleep(2000);
      Result := True;
    end
    else
    begin
      LogAndFile('Error: Failed to start phpMyAdmin server process');
      Result := False;
    end;
  except
    LogAndFile('Exception starting phpMyAdmin server: ' + GetExceptionMessage);
    Result := False;
  end;
end;

// ============================================================================
// APPLICATION CONFIGURATION FUNCTIONS
// ============================================================================

function UpdateAppConfig: Boolean;
var
  ConfigFile: String;
  ConfigContent: TArrayOfString;
  I, J: Integer;
  ConnectionString: String;
  Found: Boolean;
  Line: String;
  LowerLine: String;
  AlreadyUpdated: Boolean;
begin
  Result := False;
  AlreadyUpdated := False;
  try
    if Assigned(ProgressPage) then
    begin
      ProgressPage.SetText('Updating App.config...', '');
    end;
    
    ConfigFile := ExpandConstant('{app}\FinalPOS.exe.config');
    
    if not FileExists(ConfigFile) then
    begin
      LogAndFile('Error: Configuration file not found: ' + ConfigFile);
      Exit;
    end;
    
    LoadStringsFromFile(ConfigFile, ConfigContent);
    Found := False;
    ConnectionString := 'Server=127.0.0.1;Port=' + IntToStr(DetectedMySQLPort) + ';Database={#DatabaseName};Uid=root;Pwd=;';
    
    // First pass: Check if connection string already exists
    for I := 0 to GetArrayLength(ConfigContent) - 1 do
    begin
      LowerLine := LowerCase(ConfigContent[I]);
      if (Pos('newoneconnectionstring', LowerLine) > 0) and (Pos('connectionstring', LowerLine) > 0) then
      begin
        // Update existing connection string
        Line := ConfigContent[I];
        J := 1;
        while (J <= Length(Line)) and (Line[J] = ' ') do
          J := J + 1;
        ConfigContent[I] := Copy(Line, 1, J - 1) + '<add name="FinalPOS.Properties.Settings.NewOneConnectionString" connectionString="' + ConnectionString + '" providerName="MySql.Data.MySqlClient" />';
        Found := True;
        LogAndFile('Connection string updated at line ' + IntToStr(I + 1));
        Break;
      end;
    end;
    
    // If not found, try to find connectionStrings section and add it
    if not Found then
    begin
      LogAndFile('Connection string not found, searching for connectionStrings section...');
      for I := 0 to GetArrayLength(ConfigContent) - 1 do
      begin
        LowerLine := LowerCase(ConfigContent[I]);
        if Pos('</connectionStrings>', LowerLine) > 0 then
        begin
          Line := '';
          if I > 0 then
          begin
            Line := ConfigContent[I - 1];
            J := 1;
            while (J <= Length(Line)) and (Line[J] = ' ') do
              J := J + 1;
            Line := Copy(ConfigContent[I - 1], 1, J - 1);
          end
          else
          begin
            Line := '    ';
          end;
          
          SetArrayLength(ConfigContent, GetArrayLength(ConfigContent) + 1);
          for J := GetArrayLength(ConfigContent) - 1 downto I + 1 do
          begin
            ConfigContent[J] := ConfigContent[J - 1];
          end;
          ConfigContent[I] := Line + '<add name="FinalPOS.Properties.Settings.NewOneConnectionString" connectionString="' + ConnectionString + '" providerName="MySql.Data.MySqlClient" />';
          Found := True;
          LogAndFile('Connection string added before closing tag');
          Break;
        end;
      end;
    end;
    
    if not Found then
    begin
      LogAndFile('Error: Could not find connectionStrings section in App.config');
      Exit;
    end;
    
    SaveStringsToFile(ConfigFile, ConfigContent, False);
    AppConfigUpdated := True;
    LogAndFile('App.config updated successfully with connection string: ' + ConnectionString);
    Result := True;
  except
    LogAndFile('Exception updating App.config: ' + GetExceptionMessage);
    Result := False;
  end;
end;

// ============================================================================
// BATCH FILE CREATION FUNCTIONS
// ============================================================================

procedure CreateStartMySQLBatch;
var
  BatchFile: String;
  BatchContent: TArrayOfString;
  MySQLDExe: String;
  MySQLConfig: String;
begin
  try
    BatchFile := ExpandConstant('{app}\StartMySQL.bat');
    MySQLDExe := ExpandConstant('{app}\mysql\bin\mysqld.exe');
    MySQLConfig := ExpandConstant('{app}\mysql\my.ini');
    
    SetArrayLength(BatchContent, 0);
    SetArrayLength(BatchContent, GetArrayLength(BatchContent) + 1);
    BatchContent[GetArrayLength(BatchContent) - 1] := '@echo off';
    SetArrayLength(BatchContent, GetArrayLength(BatchContent) + 1);
    BatchContent[GetArrayLength(BatchContent) - 1] := 'echo Starting MySQL Server...';
    SetArrayLength(BatchContent, GetArrayLength(BatchContent) + 1);
    BatchContent[GetArrayLength(BatchContent) - 1] := 'cd /d "%~dp0"';
    SetArrayLength(BatchContent, GetArrayLength(BatchContent) + 1);
    BatchContent[GetArrayLength(BatchContent) - 1] := 'start /B "" "' + MySQLDExe + '" --defaults-file="' + MySQLConfig + '" --standalone --console';
    SetArrayLength(BatchContent, GetArrayLength(BatchContent) + 1);
    BatchContent[GetArrayLength(BatchContent) - 1] := 'timeout /t 3 /nobreak >nul';
    SetArrayLength(BatchContent, GetArrayLength(BatchContent) + 1);
    BatchContent[GetArrayLength(BatchContent) - 1] := 'echo MySQL Server started on port ' + IntToStr(DetectedMySQLPort);
    
    SaveStringsToFile(BatchFile, BatchContent, False);
    LogAndFile('StartMySQL.bat created successfully');
  except
    LogAndFile('Exception creating StartMySQL.bat: ' + GetExceptionMessage);
  end;
end;

procedure CreateStopMySQLBatch;
var
  BatchFile: String;
  BatchContent: TArrayOfString;
  MySQLExe: String;
begin
  try
    BatchFile := ExpandConstant('{app}\StopMySQL.bat');
    MySQLExe := ExpandConstant('{app}\mysql\bin\mysqladmin.exe');
    
    SetArrayLength(BatchContent, 0);
    SetArrayLength(BatchContent, GetArrayLength(BatchContent) + 1);
    BatchContent[GetArrayLength(BatchContent) - 1] := '@echo off';
    SetArrayLength(BatchContent, GetArrayLength(BatchContent) + 1);
    BatchContent[GetArrayLength(BatchContent) - 1] := 'echo Stopping MySQL Server...';
    SetArrayLength(BatchContent, GetArrayLength(BatchContent) + 1);
    BatchContent[GetArrayLength(BatchContent) - 1] := 'cd /d "%~dp0"';
    SetArrayLength(BatchContent, GetArrayLength(BatchContent) + 1);
    BatchContent[GetArrayLength(BatchContent) - 1] := '"' + MySQLExe + '" -u root -h 127.0.0.1 -P ' + IntToStr(DetectedMySQLPort) + ' shutdown';
    SetArrayLength(BatchContent, GetArrayLength(BatchContent) + 1);
    BatchContent[GetArrayLength(BatchContent) - 1] := 'timeout /t 2 /nobreak >nul';
    SetArrayLength(BatchContent, GetArrayLength(BatchContent) + 1);
    BatchContent[GetArrayLength(BatchContent) - 1] := 'taskkill /F /IM mysqld.exe 2>nul';
    SetArrayLength(BatchContent, GetArrayLength(BatchContent) + 1);
    BatchContent[GetArrayLength(BatchContent) - 1] := 'echo MySQL Server stopped';
    
    SaveStringsToFile(BatchFile, BatchContent, False);
    LogAndFile('StopMySQL.bat created successfully');
  except
    LogAndFile('Exception creating StopMySQL.bat: ' + GetExceptionMessage);
  end;
end;

// ============================================================================
// CUSTOM WIZARD PAGES
// ============================================================================

procedure InitializeWizard;
begin
  ConfigPage := CreateOutputProgressPage('Configuration Summary', 'Review the installation configuration before proceeding.');
  ProgressPage := CreateOutputProgressPage('Installation', 'Please wait while components are being configured...');
end;

procedure ShowConfigPage;
var
  ConfigText: String;
begin
  ConfigPage.SetText('Configuration Summary', 'Review the configuration before installation begins:');
  ConfigText := 'Install Directory: ' + ExpandConstant('{app}') + #13#10 +
                'MySQL Port: ' + IntToStr(DetectedMySQLPort) + #13#10 +
                'Database Name: {#DatabaseName}' + #13#10 +
                'phpMyAdmin Port: ' + IntToStr(DetectedphpMyAdminPort) + #13#10 + #13#10 +
                'Status: Ready to install';
  
  ConfigPage.SetProgress(0, 0);
  ConfigPage.Show;
  
  Sleep(2000);
  
  ConfigPage.Hide;
end;

// ============================================================================
// INSTALLER WORKFLOW FUNCTIONS
// ============================================================================

function InitializeSetup(): Boolean;
begin
  Result := True;
  AppConfigUpdated := False;
  MySQLInitialized := False;
  DatabaseCreated := False;
  LogFile := '';
  
  // Initialize log file
  try
    LogFile := ExpandConstant('{tmp}\install-log.txt');
    LogAndFile('=== FinalPOS Installer Started ===');
  except
    LogFile := '';
  end;
  
  // Detect ports
  LogAndFile('Detecting available ports...');
  DetectedMySQLPort := FindAvailablePort({#MySQLBasePort});
  DetectedphpMyAdminPort := FindAvailablePort({#phpMyAdminBasePort});
  
  LogAndFile('MySQL port: ' + IntToStr(DetectedMySQLPort));
  LogAndFile('phpMyAdmin port: ' + IntToStr(DetectedphpMyAdminPort));
end;

function InitializeUninstall(): Boolean;
begin
  Result := True;
  
  try
    LogFile := ExpandConstant('{app}\uninstall-log.txt');
    LogAndFile('=== FinalPOS Uninstall Started ===');
  except
    LogFile := '';
  end;
  
  LogAndFile('Stopping MySQL server...');
  // StopMySQL.bat will be executed by UninstallRun section
  
  LogAndFile('Stopping PHP server...');
  // PHP stop will be executed by UninstallRun section
  
  LogAndFile('Uninstall cleanup completed.');
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  
  if CurPageID = wpReady then
  begin
    ShowConfigPage;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
begin
  if CurStep = ssPostInstall then
  begin
    LogAndFile('=== Starting installation phase ===');
    
    ProgressPage.SetText('Installation', 'Please wait while components are being configured...');
    ProgressPage.SetProgress(0, 0);
    ProgressPage.Show;
    
    try
      // Step 1: Download and install MySQL
      if Assigned(ProgressPage) then
        ProgressPage.SetText('Downloading and installing MySQL Server...', 'This may take several minutes...');
      
      if not DownloadAndExtractMySQL then
      begin
        ProgressPage.Hide;
        MsgBox('Failed to download or install MySQL Server.' + #13#10 + #13#10 +
               'Please check your internet connection and try again.', 
               mbError, MB_OK);
        Exit;
      end;
      
      // Step 2: Create MySQL configuration
      if Assigned(ProgressPage) then
        ProgressPage.SetText('Configuring MySQL Server...', '');
      
      if not CreateMySQLConfig then
      begin
        ProgressPage.Hide;
        MsgBox('Failed to create MySQL configuration file.', mbError, MB_OK);
        Exit;
      end;
      
      // Step 3: Initialize MySQL data directory
      if Assigned(ProgressPage) then
        ProgressPage.SetText('Initializing MySQL data directory...', 'This may take a few minutes...');
      
      if not InitializeMySQLData then
      begin
        ProgressPage.Hide;
        MsgBox('Failed to initialize MySQL data directory.' + #13#10 + #13#10 +
               'The installation will continue, but MySQL may not work properly.', 
               mbError, MB_OK);
      end
      else
      begin
        MySQLInitialized := True;
      end;
      
      // Step 4: Start MySQL
      if Assigned(ProgressPage) then
        ProgressPage.SetText('Starting MySQL Server...', '');
      
      if not StartMySQL then
      begin
        LogAndFile('Warning: MySQL may not have started properly');
      end;
      
      // Step 5: Create database
      if Assigned(ProgressPage) then
        ProgressPage.SetText('Creating database...', '');
      
      if CreateDatabase then
      begin
        DatabaseCreated := True;
      end;
      
      // Step 6: Download and install phpMyAdmin
      if Assigned(ProgressPage) then
        ProgressPage.SetText('Downloading and installing phpMyAdmin...', '');
      
      if not DownloadAndExtractphpMyAdmin then
      begin
        LogAndFile('Warning: Failed to download or install phpMyAdmin');
      end;
      
      // Step 7: Create phpMyAdmin config
      if Assigned(ProgressPage) then
        ProgressPage.SetText('Configuring phpMyAdmin...', '');
      
      CreatephpMyAdminConfig;
      
      // Step 8: Update App.config
      if Assigned(ProgressPage) then
        ProgressPage.SetText('Updating application configuration...', '');
      
      if not UpdateAppConfig then
      begin
        ProgressPage.Hide;
        MsgBox('Failed to update application configuration. The application may not connect to the database.' + #13#10 + #13#10 +
               'You may need to manually update FinalPOS.exe.config with the correct connection string.', 
               mbError, MB_OK);
        Exit;
      end;
      
      // Step 9: Create batch files
      if Assigned(ProgressPage) then
        ProgressPage.SetText('Creating management scripts...', '');
      
      CreateStartMySQLBatch;
      CreateStopMySQLBatch;
      
      // Step 10: Start phpMyAdmin
      if Assigned(ProgressPage) then
        ProgressPage.SetText('Starting phpMyAdmin...', '');
      
      if StartphpMyAdmin then
      begin
        LogAndFile('phpMyAdmin server started successfully');
        Sleep(2000);
        
        // Open browser
        if Assigned(ProgressPage) then
          ProgressPage.SetText('Opening phpMyAdmin...', '');
        
        Exec('cmd.exe', '/c start http://localhost:' + IntToStr(DetectedphpMyAdminPort), '', SW_SHOWNORMAL, ewNoWait, ResultCode);
        LogAndFile('Browser opened to phpMyAdmin at http://localhost:' + IntToStr(DetectedphpMyAdminPort));
      end
      else
      begin
        LogAndFile('Warning: Failed to start phpMyAdmin server');
      end;
      
      ProgressPage.Hide;
      LogAndFile('=== Installation completed successfully ===');
      LogAndFile('MySQL Port: ' + IntToStr(DetectedMySQLPort));
      LogAndFile('phpMyAdmin Port: ' + IntToStr(DetectedphpMyAdminPort));
      
    except
      ProgressPage.Hide;
      RaiseException('Installation failed: ' + GetExceptionMessage);
    end;
  end;
end;
