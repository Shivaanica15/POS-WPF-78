; ============================================================================
; FinalPOS Inno Setup Installer Script - PRODUCTION READY
; Complete installation system for FinalPOS with portable MySQL
; ============================================================================
; Version: 1.0.0
; Build Date: 2024
; MySQL Version: 8.0.44
; Database: POS_NEXA_ERP
; Port: 3307
; Root Password: Shivaanica
; ============================================================================
; PATH RESOLUTION:
;   - All paths are relative to where this .iss file is located
;   - Script location: InstallerFiles\
;   - MySQL auto-detection: InstallerFiles\mysql\ (preferred) OR mysql3307\ (fallback)
; ============================================================================

#define MyAppName "FinalPOS"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "FinalPOS"
#define MyAppURL "https://www.finalpos.com"
#define MyAppExeName "FinalPOS.exe"

; ============================================================================
; SETUP SECTION
; ============================================================================

[Setup]
; Application identification
AppId={{A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}

; Installation paths
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes

; Installer metadata
OutputDir=.
OutputBaseFilename=FinalPOS-Setup
SetupIconFile=
LicenseFile=
InfoBeforeFile=
InfoAfterFile=

; Compression and appearance
Compression=lzma
SolidCompression=yes
WizardStyle=modern

; Security and privileges
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64

; Application behavior
CloseApplications=yes
RestartApplications=no

; Uninstaller settings
UninstallDisplayIcon={app}\{#MyAppExeName}
UninstallDisplayName={#MyAppName}
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription=FinalPOS Point of Sale System
VersionInfoCopyright=Copyright (C) 2024

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode

; ============================================================================
; FILES SECTION
; ============================================================================
; All Source paths are relative to InstallerFiles\ directory
; ============================================================================

[Files]
; MySQL portable files - Priority 1: InstallerFiles\mysql\
; This entry is checked first and preferred
Source: "mysql\*"; DestDir: "C:\FinalPOS-MySQL"; Flags: ignoreversion recursesubdirs createallsubdirs; Permissions: users-full; Check: MySQLFolderExists

; MySQL portable files - Priority 2: mysql3307\ (project root, fallback)
; Only used if InstallerFiles\mysql\ doesn't exist
Source: "..\mysql3307\*"; DestDir: "C:\FinalPOS-MySQL"; Flags: ignoreversion recursesubdirs createallsubdirs; Permissions: users-full; Check: MySQL3307FolderExists

; MySQL configuration file
Source: "config\my.ini"; DestDir: "C:\FinalPOS-MySQL"; Flags: ignoreversion; Permissions: users-full; Check: ConfigFileExists

; MySQL management scripts - Batch files
; Includes: Start-MySQL.bat, Stop-MySQL.bat, Check-MySQL.bat, Install-MySQL.bat, Validate-Schema.bat
Source: "scripts\*.bat"; DestDir: "C:\FinalPOS-MySQL\scripts"; Flags: ignoreversion; Permissions: users-full; Check: ScriptsFolderExists

; MySQL database scripts - SQL files
; Includes: init.sql (database initialization), schema.sql (complete schema)
Source: "scripts\*.sql"; DestDir: "C:\FinalPOS-MySQL\scripts"; Flags: ignoreversion; Permissions: users-full; Check: ScriptsFolderExists

; FinalPOS application files
; Includes: FinalPOS.exe, all DLLs, config files, resources
Source: "app\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs; Permissions: users-full; Check: AppFolderExists

; ============================================================================
; ICONS SECTION
; ============================================================================

[Icons]
; Main application shortcut
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"

; MySQL management shortcuts
Name: "{group}\Start MySQL Server"; Filename: "C:\FinalPOS-MySQL\scripts\Start-MySQL.bat"; IconFilename: "{app}\{#MyAppExeName}"
Name: "{group}\Stop MySQL Server"; Filename: "C:\FinalPOS-MySQL\scripts\Stop-MySQL.bat"; IconFilename: "{app}\{#MyAppExeName}"
Name: "{group}\Check MySQL Status"; Filename: "C:\FinalPOS-MySQL\scripts\Check-MySQL.bat"; IconFilename: "{app}\{#MyAppExeName}"
Name: "{group}\Validate Schema"; Filename: "C:\FinalPOS-MySQL\scripts\Validate-Schema.bat"; IconFilename: "{app}\{#MyAppExeName}"

; Uninstaller shortcut
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

; Desktop and Quick Launch shortcuts (optional)
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

; ============================================================================
; RUN SECTION
; ============================================================================

[Run]
; Initialize MySQL database (runs Install-MySQL.bat)
; This script: initializes MySQL, sets password, creates DB, imports schema
Filename: "C:\FinalPOS-MySQL\scripts\Install-MySQL.bat"; Description: "Initialize MySQL Database"; Flags: runhidden waituntilterminated; StatusMsg: "Initializing MySQL database..."; BeforeInstall: StopMySQLIfRunning; AfterInstall: ValidateMySQLConnection

; Launch application after installation (optional, skip if silent)
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

; ============================================================================
; CODE SECTION - Pascal Script
; ============================================================================

[Code]
var
  MySQLInitialized: Boolean;
  LogFile: String;
  DetectedMySQLFolder: String;
  AppLogDir: String;

// ============================================================================
// PATH VALIDATION FUNCTIONS (Runtime Checks)
// ============================================================================

function MySQLFolderExists(): Boolean;
var
  ScriptPath: String;
  MySQLPath: String;
begin
  // Check Priority 1: InstallerFiles\mysql\bin\mysqld.exe
  ScriptPath := ExtractFilePath(ExpandConstant('{srcexe}'));
  MySQLPath := ScriptPath + 'mysql\bin\mysqld.exe';
  Result := FileExists(MySQLPath);
  
  if Result then
  begin
    DetectedMySQLFolder := ScriptPath + 'mysql';
    Log('MySQL folder detected (Priority 1): ' + DetectedMySQLFolder);
  end;
end;

function MySQL3307FolderExists(): Boolean;
var
  ScriptPath: String;
  MySQLPath: String;
begin
  // Check Priority 2: mysql3307\bin\mysqld.exe (project root, one level up)
  // Only use this if mysql folder doesn't exist (fallback)
  ScriptPath := ExtractFilePath(ExpandConstant('{srcexe}'));
  MySQLPath := ExtractFilePath(ScriptPath) + 'mysql3307\bin\mysqld.exe';
  Result := FileExists(MySQLPath) and not MySQLFolderExists();
  
  if Result then
  begin
    DetectedMySQLFolder := ExtractFilePath(ScriptPath) + 'mysql3307';
    Log('MySQL folder detected (Priority 2 - Fallback): ' + DetectedMySQLFolder);
  end;
end;

function ConfigFileExists(): Boolean;
var
  ScriptPath: String;
  ConfigPath: String;
begin
  ScriptPath := ExtractFilePath(ExpandConstant('{srcexe}'));
  ConfigPath := ScriptPath + 'config\my.ini';
  Result := FileExists(ConfigPath);
  
  if not Result then
  begin
    Log('ERROR: config\my.ini not found at: ' + ConfigPath);
  end;
end;

function ScriptsFolderExists(): Boolean;
var
  ScriptPath: String;
  ScriptsPath: String;
begin
  ScriptPath := ExtractFilePath(ExpandConstant('{srcexe}'));
  ScriptsPath := ScriptPath + 'scripts\';
  Result := DirExists(ScriptsPath);
  
  if Result then
  begin
    // Validate critical batch scripts exist
    Result := FileExists(ScriptsPath + 'Install-MySQL.bat') and
              FileExists(ScriptsPath + 'Start-MySQL.bat') and
              FileExists(ScriptsPath + 'Stop-MySQL.bat') and
              FileExists(ScriptsPath + 'Check-MySQL.bat');
    
    // Validate SQL scripts exist
    if Result then
    begin
      Result := FileExists(ScriptsPath + 'init.sql') and
                FileExists(ScriptsPath + 'schema.sql');
    end;
  end;
  
  if not Result then
  begin
    Log('ERROR: Scripts folder incomplete at: ' + ScriptsPath);
  end;
end;

function AppFolderExists(): Boolean;
var
  ScriptPath: String;
  AppPath: String;
begin
  ScriptPath := ExtractFilePath(ExpandConstant('{srcexe}'));
  AppPath := ScriptPath + 'app\' + ExpandConstant('{#MyAppExeName}');
  Result := FileExists(AppPath);
  
  if not Result then
  begin
    Log('ERROR: Application file not found at: ' + AppPath);
  end;
end;

// ============================================================================
// COMPILER PATH VALIDATION FUNCTION
// ============================================================================
// This function validates ALL required paths before installation begins
// Shows clear error messages if anything is missing
// ============================================================================

function ValidateCompilerPaths(): Boolean;
var
  ScriptPath: String;
  MySQLPath1: String;
  MySQLPath2: String;
  MySQLFound: Boolean;
  ErrorMsg: String;
begin
  Result := True;
  ScriptPath := ExtractFilePath(ExpandConstant('{srcexe}'));
  ErrorMsg := '';
  
  Log('=== Path Validation Started ===');
  Log('Script location: ' + ScriptPath);
  
  // Validate MySQL folder (Priority 1: InstallerFiles\mysql\, Priority 2: mysql3307\)
  MySQLPath1 := ScriptPath + 'mysql\bin\mysqld.exe';
  MySQLPath2 := ExtractFilePath(ScriptPath) + 'mysql3307\bin\mysqld.exe';
  MySQLFound := FileExists(MySQLPath1) or FileExists(MySQLPath2);
  
  if not MySQLFound then
  begin
    ErrorMsg := ErrorMsg + 'MySQL folder not found!' + #13#10;
    ErrorMsg := ErrorMsg + 'Checked locations:' + #13#10;
    ErrorMsg := ErrorMsg + '  1. ' + MySQLPath1 + #13#10;
    ErrorMsg := ErrorMsg + '  2. ' + MySQLPath2 + #13#10;
    ErrorMsg := ErrorMsg + #13#10;
    ErrorMsg := ErrorMsg + 'Please extract MySQL 8.0.44 portable ZIP to:' + #13#10;
    ErrorMsg := ErrorMsg + '  InstallerFiles\mysql\  (preferred)' + #13#10;
    ErrorMsg := ErrorMsg + '  OR' + #13#10;
    ErrorMsg := ErrorMsg + '  mysql3307\  (project root)' + #13#10;
    Result := False;
  end
  else
  begin
    if FileExists(MySQLPath1) then
      Log('MySQL found: ' + MySQLPath1)
    else
      Log('MySQL found (fallback): ' + MySQLPath2);
  end;
  
  // Validate config file
  if not FileExists(ScriptPath + 'config\my.ini') then
  begin
    ErrorMsg := ErrorMsg + #13#10 + 'Configuration file not found!' + #13#10;
    ErrorMsg := ErrorMsg + 'Expected: ' + ScriptPath + 'config\my.ini' + #13#10;
    Result := False;
  end
  else
  begin
    Log('Config file found: ' + ScriptPath + 'config\my.ini');
  end;
  
  // Validate scripts folder
  if not DirExists(ScriptPath + 'scripts\') then
  begin
    ErrorMsg := ErrorMsg + #13#10 + 'Scripts folder not found!' + #13#10;
    ErrorMsg := ErrorMsg + 'Expected: ' + ScriptPath + 'scripts\' + #13#10;
    Result := False;
  end
  else
  begin
    // Check critical scripts
    if not FileExists(ScriptPath + 'scripts\Install-MySQL.bat') then
    begin
      ErrorMsg := ErrorMsg + #13#10 + 'Install-MySQL.bat not found!' + #13#10;
      Result := False;
    end;
    if not FileExists(ScriptPath + 'scripts\init.sql') then
    begin
      ErrorMsg := ErrorMsg + #13#10 + 'init.sql not found!' + #13#10;
      Result := False;
    end;
    if not FileExists(ScriptPath + 'scripts\schema.sql') then
    begin
      ErrorMsg := ErrorMsg + #13#10 + 'schema.sql not found!' + #13#10;
      Result := False;
    end;
    
    if Result then
    begin
      Log('All scripts found in: ' + ScriptPath + 'scripts\');
    end;
  end;
  
  // Validate app folder
  if not FileExists(ScriptPath + 'app\' + ExpandConstant('{#MyAppExeName}')) then
  begin
    ErrorMsg := ErrorMsg + #13#10 + 'Application files not found!' + #13#10;
    ErrorMsg := ErrorMsg + 'Expected: ' + ScriptPath + 'app\' + ExpandConstant('{#MyAppExeName}') + #13#10;
    ErrorMsg := ErrorMsg + 'Please copy FinalPOS Release build to app\ folder.' + #13#10;
    Result := False;
  end
  else
  begin
    Log('Application found: ' + ScriptPath + 'app\' + ExpandConstant('{#MyAppExeName}'));
  end;
  
  // Show error if validation failed
  if not Result then
  begin
    ErrorMsg := 'INSTALLATION CANNOT PROCEED' + #13#10 + #13#10 + ErrorMsg;
    MsgBox(ErrorMsg, mbError, MB_OK);
    Log('Path validation FAILED');
  end
  else
  begin
    Log('Path validation PASSED');
  end;
end;

// ============================================================================
// INITIALIZATION FUNCTIONS
// ============================================================================

procedure InitializeWizard;
begin
  // Initialize log file
  LogFile := ExpandConstant('{tmp}\FinalPOS-Install.log');
  AppLogDir := ExpandConstant('{localappdata}\FinalPOS\Logs');
  
  Log('============================================================================');
  Log('FinalPOS Installation Started');
  Log('============================================================================');
  Log('Script location: ' + ExtractFilePath(ExpandConstant('{srcexe}')));
  Log('Installation directory: ' + ExpandConstant('{app}'));
  Log('MySQL directory: C:\FinalPOS-MySQL');
  Log('Log file: ' + LogFile);
  Log('Application log directory: ' + AppLogDir);
  Log('============================================================================');
end;

function InitializeSetup(): Boolean;
var
  ResultCode: Integer;
begin
  Result := True;
  
  // Validate all paths before proceeding
  if not ValidateCompilerPaths() then
  begin
    Result := False;
    Exit;
  end;
  
  // Check if MySQL is already running (will be stopped during installation)
  if Exec('tasklist', '/FI "IMAGENAME eq mysqld.exe"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    if ResultCode = 0 then
    begin
      Log('MySQL server is currently running. Will stop it during installation.');
    end;
  end;
  
  Log('Initialization complete. Ready to proceed with installation.');
end;

// ============================================================================
// MYSQL MANAGEMENT FUNCTIONS
// ============================================================================

procedure StopMySQLIfRunning;
var
  ResultCode: Integer;
  MaxWait: Integer;
  WaitCount: Integer;
begin
  Log('Stopping MySQL server if running...');
  
  // Try graceful shutdown first using mysqladmin
  if FileExists('C:\FinalPOS-MySQL\bin\mysqladmin.exe') then
  begin
    Log('Attempting graceful MySQL shutdown...');
    Exec('C:\FinalPOS-MySQL\bin\mysqladmin.exe', '-u root -pShivaanica --port=3307 shutdown', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    Sleep(3000); // Wait for graceful shutdown
    
    // Check if MySQL is still running
    WaitCount := 0;
    MaxWait := 10; // Maximum 10 seconds wait
    while WaitCount < MaxWait do
    begin
      if Exec('tasklist', '/FI "IMAGENAME eq mysqld.exe"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
      begin
        if ResultCode <> 0 then
        begin
          Log('MySQL stopped gracefully.');
          Exit;
        end;
      end;
      Sleep(1000);
      WaitCount := WaitCount + 1;
    end;
  end;
  
  // Force kill if graceful shutdown failed or mysqladmin not available
  Log('Forcing MySQL shutdown...');
  Exec('taskkill', '/F /IM mysqld.exe /T', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Sleep(2000);
  
  // Verify MySQL is stopped
  if Exec('tasklist', '/FI "IMAGENAME eq mysqld.exe"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    if ResultCode = 0 then
    begin
      Log('WARNING: MySQL may still be running after shutdown attempt.');
    end
    else
    begin
      Log('MySQL server stopped successfully.');
    end;
  end;
end;

procedure ValidateMySQLConnection;
var
  ResultCode: Integer;
  MySQLClient: String;
  RetryCount: Integer;
  MaxRetries: Integer;
begin
  Log('Validating MySQL connection...');
  MySQLClient := 'C:\FinalPOS-MySQL\bin\mysql.exe';
  RetryCount := 0;
  MaxRetries := 5;
  
  if not FileExists(MySQLClient) then
  begin
    Log('WARNING: MySQL client not found. Cannot validate connection.');
    Exit;
  end;
  
  // Retry connection validation (MySQL may need a moment to fully start)
  while RetryCount < MaxRetries do
  begin
    Exec(MySQLClient, '-u root -pShivaanica --port=3307 -e "USE POS_NEXA_ERP; SELECT ''Connection OK'' AS Status;"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    
    if ResultCode = 0 then
    begin
      Log('MySQL connection validated successfully.');
      Log('Database: POS_NEXA_ERP');
      Log('Port: 3307');
      Log('User: root');
      Exit;
    end;
    
    RetryCount := RetryCount + 1;
    if RetryCount < MaxRetries then
    begin
      Log('Connection validation attempt ' + IntToStr(RetryCount) + ' failed. Retrying...');
      Sleep(2000);
    end;
  end;
  
  Log('WARNING: MySQL connection validation failed after ' + IntToStr(MaxRetries) + ' attempts.');
  Log('Error code: ' + IntToStr(ResultCode));
  MsgBox('Warning: MySQL connection validation failed.' + #13#10 + #13#10 +
         'The database may not be properly configured.' + #13#10 +
         'Please check:' + #13#10 +
         '1. MySQL server is running (use Start-MySQL.bat)' + #13#10 +
         '2. Port 3307 is not blocked' + #13#10 +
         '3. Password is correct: Shivaanica' + #13#10 +
         '4. Check installation logs for details.', 
         mbError, MB_OK);
end;

// ============================================================================
// INSTALLATION STEP HANDLERS
// ============================================================================

procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
begin
  if CurStep = ssPostInstall then
  begin
    Log('Post-install step: Verifying installation...');
    
    // Wait for files to be written
    Sleep(2000);
    
    // Verify MySQL files were installed
    if not DirExists('C:\FinalPOS-MySQL\bin') then
    begin
      Log('ERROR: MySQL bin directory not found after installation!');
      MsgBox('ERROR: MySQL files were not installed correctly.' + #13#10 +
             'Installation may be incomplete. Please check the logs.', 
             mbError, MB_OK);
      Exit;
    end;
    
    // Verify scripts were installed
    if not FileExists('C:\FinalPOS-MySQL\scripts\Install-MySQL.bat') then
    begin
      Log('ERROR: MySQL scripts were not installed correctly!');
      MsgBox('ERROR: MySQL scripts were not installed correctly.' + #13#10 +
             'Installation may be incomplete. Please check the logs.', 
             mbError, MB_OK);
      Exit;
    end;
    
    // Verify application was installed
    if not FileExists(ExpandConstant('{app}\{#MyAppExeName}')) then
    begin
      Log('ERROR: Application was not installed correctly!');
      MsgBox('ERROR: Application files were not installed correctly.' + #13#10 +
             'Installation may be incomplete. Please check the logs.', 
             mbError, MB_OK);
      Exit;
    end;
    
    Log('Installation verification passed.');
    Log('MySQL directory: C:\FinalPOS-MySQL');
    Log('Application directory: ' + ExpandConstant('{app}'));
  end;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  
  if CurPageID = wpReady then
  begin
    Log('Ready to install. All files will be copied.');
    Log('Installation will:');
    Log('  1. Copy MySQL files to C:\FinalPOS-MySQL');
    Log('  2. Copy application files to ' + ExpandConstant('{app}'));
    Log('  3. Initialize MySQL database');
    Log('  4. Import schema.sql');
    Log('  5. Create shortcuts');
  end;
end;

// ============================================================================
// UNINSTALLATION FUNCTIONS
// ============================================================================

function InitializeUninstall(): Boolean;
var
  ResultCode: Integer;
  MaxWait: Integer;
  WaitCount: Integer;
begin
  Result := True;
  Log('============================================================================');
  Log('FinalPOS Uninstallation Started');
  Log('============================================================================');
  
  // Stop FinalPOS application if running
  Log('Stopping FinalPOS application...');
  Exec('taskkill', '/F /IM FinalPOS.exe /T', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Sleep(1000);
  
  // Stop MySQL server before uninstallation
  Log('Stopping MySQL server...');
  if FileExists('C:\FinalPOS-MySQL\bin\mysqladmin.exe') then
  begin
    Log('Attempting graceful MySQL shutdown...');
    Exec('C:\FinalPOS-MySQL\bin\mysqladmin.exe', '-u root -pShivaanica --port=3307 shutdown', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
    Sleep(3000);
    
    // Wait for MySQL to stop
    WaitCount := 0;
    MaxWait := 10;
    while WaitCount < MaxWait do
    begin
      if Exec('tasklist', '/FI "IMAGENAME eq mysqld.exe"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
      begin
        if ResultCode <> 0 then
        begin
          Log('MySQL stopped gracefully.');
          Break;
        end;
      end;
      Sleep(1000);
      WaitCount := WaitCount + 1;
    end;
  end;
  
  // Force kill MySQL if still running
  Log('Ensuring MySQL is stopped...');
  Exec('taskkill', '/F /IM mysqld.exe /T', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  Sleep(2000);
  
  Log('MySQL server stopped.');
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  ResultCode: Integer;
begin
  if CurUninstallStep = usUninstall then
  begin
    Log('Removing installation directories...');
    
    // Remove MySQL directory
    if DirExists('C:\FinalPOS-MySQL') then
    begin
      Log('Removing C:\FinalPOS-MySQL...');
      DelTree('C:\FinalPOS-MySQL', True, True, True);
      Log('MySQL directory removed.');
    end;
    
    // Remove application log directory (if exists)
    if DirExists(ExpandConstant('{localappdata}\FinalPOS\Logs')) then
    begin
      Log('Removing application logs...');
      DelTree(ExpandConstant('{localappdata}\FinalPOS\Logs'), True, True, True);
    end;
    
    Log('============================================================================');
    Log('FinalPOS Uninstallation Complete');
    Log('============================================================================');
  end;
end;

// ============================================================================
// UNINSTALL DELETE SECTION
// ============================================================================

[UninstallDelete]
; Remove MySQL directory completely
Type: filesandordirs; Name: "C:\FinalPOS-MySQL"

; Remove application log files
Type: files; Name: "{app}\*.log"
Type: files; Name: "{app}\*.tmp"
Type: filesandordirs; Name: "{localappdata}\FinalPOS\Logs"

// ============================================================================
// UNINSTALL RUN SECTION
// ============================================================================

[UninstallRun]
; Stop FinalPOS application before uninstall
Filename: "taskkill"; Parameters: "/F /IM FinalPOS.exe /T"; Flags: runhidden; RunOnceId: "StopApp"

; Stop MySQL server before uninstall
Filename: "taskkill"; Parameters: "/F /IM mysqld.exe /T"; Flags: runhidden; RunOnceId: "StopMySQL"

// ============================================================================
// END OF SCRIPT
// ============================================================================
