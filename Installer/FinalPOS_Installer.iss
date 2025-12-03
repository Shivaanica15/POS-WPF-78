; ============================================================================
; FinalPOS Inno Setup Installer Script
; Production-ready installer with XAMPP bundling and automatic MySQL port configuration
; Version: 1.0
; ============================================================================

#define MyAppName "FinalPOS"
#define MyAppVersion "1.0"
#define MyAppPublisher "FinalPOS"
#define MyAppURL "https://www.finalpos.com"
#define MyAppExeName "FinalPOS.exe"
#define BasePath "C:\\Users\\shiva\\Documents\\GitHub\\POS-WPF-78\\Installer\\InstallerFiles"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
AppId={{A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=
OutputDir=Output
OutputBaseFilename=FinalPOS_Setup_v{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin
ArchitecturesAllowed=x64
ArchitecturesInstallIn64BitMode=x64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1

[Files]
; Application files - Copy from FinalPOS\bin\Release\* to InstallerFiles\App\
Source: "{#BasePath}\\App\\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; XAMPP files - Extract XAMPP Portable to InstallerFiles\XAMPP\ (conditional install)
Source: "{#BasePath}\\XAMPP\\*"; DestDir: "C:\\FinalPOS-XAMPP"; Flags: ignoreversion recursesubdirs createallsubdirs; Check: ShouldInstallXAMPP
; Helper scripts - PowerShell scripts in InstallerFiles\Tools\
Source: "{#BasePath}\\Tools\\FindAvailablePort.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "{#BasePath}\\Tools\\ConfigureMySQL.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "{#BasePath}\\Tools\\UpdateAppConfig.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "{#BasePath}\\Tools\\StartMySQL.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "{#BasePath}\\Tools\\DetectXAMPP.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "{#BasePath}\\Tools\\TestMySQLConnection.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "{#BasePath}\\Tools\\ResolvePortConflict.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
; Step 1: Detect available MySQL port (for bundled XAMPP) or check/resolve port conflict (for existing XAMPP)
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\FindAvailablePort.ps1"" -OutputFile ""{{tmp}}\mysql_port.txt"""; StatusMsg: "Detecting available MySQL port..."; Flags: runhidden waituntilterminated; Check: ShouldInstallXAMPP
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\ResolvePortConflict.ps1"" -RequestedPortFile ""{{tmp}}\requested_port.txt"" -OutputFile ""{{tmp}}\mysql_port.txt"""; StatusMsg: "Resolving MySQL port conflicts..."; Flags: runhidden waituntilterminated; Check: ShouldUseExistingXAMPP
; Step 2: Configure MySQL my.ini with detected port (only for bundled XAMPP)
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\ConfigureMySQL.ps1"" -XamppPath ""C:\FinalPOS-XAMPP"" -PortFile ""{{tmp}}\mysql_port.txt"""; StatusMsg: "Configuring MySQL server..."; Flags: runhidden waituntilterminated; Check: ShouldInstallXAMPP
; Step 3: Update App.config connection string with port and password
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\UpdateAppConfig.ps1"" -AppConfigPath ""{{app}}\App.config"" -PortFile ""{{tmp}}\mysql_port.txt"" -PasswordFile ""{{tmp}}\mysql_password.txt"""; StatusMsg: "Updating application configuration..."; Flags: runhidden waituntilterminated
; Step 4: Start MySQL server with configured port (only for bundled XAMPP)
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\StartMySQL.ps1"" -XamppPath ""C:\FinalPOS-XAMPP"" -PortFile ""{{tmp}}\mysql_port.txt"""; StatusMsg: "Starting MySQL server..."; Flags: runhidden waituntilterminated; Check: ShouldInstallXAMPP
; Step 5: Save port to App folder for reference
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -Command ""if (Test-Path '{{tmp}}\mysql_port.txt') {{ $port = Get-Content '{{tmp}}\mysql_port.txt' -ErrorAction SilentlyContinue; if ($port) {{ Set-Content -Path '{{app}}\mysql_port.txt' -Value $port -ErrorAction Stop }} }}"""; StatusMsg: "Saving port configuration..."; Flags: runhidden waituntilterminated
; Step 6: Launch FinalPOS application
Filename: "{app}\{#MyAppExeName}"; Description: "Launch FinalPOS"; Flags: nowait postinstall skipifsilent

[Code]
var
  MySQLPage: TInputQueryWizardPage;
  UseExistingXAMPP: Boolean;
  XAMPPPath: String;
  MySQLPort: String;
  MySQLPassword: String;
  MySQLUsername: String;

function DetectXAMPPInstallation(): String;
var
  CommonPaths: TArrayOfString;
  Path: String;
  MySQLPath: String;
  I: Integer;
begin
  Result := '';
  
  // Common XAMPP installation paths
  SetArrayLength(CommonPaths, 5);
  CommonPaths[0] := 'C:\xampp';
  CommonPaths[1] := ExpandConstant('{pf}\xampp');
  CommonPaths[2] := ExpandConstant('{pf32}\xampp');
  CommonPaths[3] := ExpandConstant('{userdesktop}\xampp');
  CommonPaths[4] := ExpandConstant('{userdocs}\xampp');
  
  // Check each path
  for I := 0 to GetArrayLength(CommonPaths) - 1 do
  begin
    Path := CommonPaths[I];
    if DirExists(Path) then
    begin
      MySQLPath := Path + '\mysql\bin\mysqld.exe';
      if FileExists(MySQLPath) then
      begin
        Result := Path;
        Break;
      end;
    end;
  end;
end;

function InitializeSetup(): Boolean;
begin
  Result := True;
  
  // Detect existing XAMPP installations
  XAMPPPath := DetectXAMPPInstallation();
end;

function InitializeUninstall(): Boolean;
begin
  Result := True;
end;

procedure InitializeWizard;
begin
  // Create custom page for MySQL configuration
  MySQLPage := CreateInputQueryPage(wpSelectTasks,
    'MySQL Database Configuration', 'Configure MySQL database connection',
    'Please specify your MySQL database settings. If you have XAMPP installed, you can use your existing installation.');

  MySQLPage.Add('Use existing XAMPP? (Yes/No):', False);
  MySQLPage.Add('XAMPP Installation Path:', False);
  MySQLPage.Add('MySQL Port (default: 3306):', False);
  MySQLPage.Add('MySQL Username (default: root):', False);
  MySQLPage.Add('MySQL Password (leave empty if no password):', True);
  
  // Set default values
  if XAMPPPath <> '' then
  begin
    MySQLPage.Values[0] := 'Yes';
    MySQLPage.Values[1] := XAMPPPath;
  end
  else
  begin
    MySQLPage.Values[0] := 'No';
    MySQLPage.Values[1] := 'C:\xampp';
  end;
  MySQLPage.Values[2] := '3306';
  MySQLPage.Values[3] := 'root';
  MySQLPage.Values[4] := '';
end;

function ShouldInstallXAMPP(): Boolean;
begin
  Result := not UseExistingXAMPP;
end;

function ShouldUseExistingXAMPP(): Boolean;
begin
  Result := UseExistingXAMPP;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  ResultCode: Integer;
  TestScript: String;
  UseExisting: String;
begin
  Result := True;
  
  if CurPageID = MySQLPage.ID then
  begin
    // Validate input
    UseExisting := Trim(LowerCase(MySQLPage.Values[0]));
    if (UseExisting = 'yes') or (UseExisting = 'y') then
    begin
      UseExistingXAMPP := True;
      XAMPPPath := Trim(MySQLPage.Values[1]);
      MySQLPort := Trim(MySQLPage.Values[2]);
      MySQLUsername := Trim(MySQLPage.Values[3]);
      MySQLPassword := Trim(MySQLPage.Values[4]);
      
      // Validate XAMPP path
      if XAMPPPath = '' then
      begin
        MsgBox('Please specify the XAMPP installation path.', mbError, MB_OK);
        Result := False;
        Exit;
      end;
      
      if not DirExists(XAMPPPath) then
      begin
        if MsgBox('The specified XAMPP path does not exist. Continue anyway?', mbConfirmation, MB_YESNO) = IDNO then
        begin
          Result := False;
          Exit;
        end;
      end;
      
      // Validate port
      if MySQLPort = '' then
      begin
        MySQLPort := '3306';
      end;
      
      // Validate username
      if MySQLUsername = '' then
      begin
        MySQLUsername := 'root';
      end;
      
      // Save port and password to temp files
      SaveStringToFile(ExpandConstant('{tmp}\requested_port.txt'), MySQLPort, False);
      SaveStringToFile(ExpandConstant('{tmp}\mysql_password.txt'), MySQLPassword, False);
    end
    else
    begin
      UseExistingXAMPP := False;
      MySQLPort := '3306'; // Will be detected by FindAvailablePort.ps1
      MySQLUsername := 'root';
      MySQLPassword := ''; // No password for bundled XAMPP
      SaveStringToFile(ExpandConstant('{tmp}\mysql_password.txt'), '', False);
    end;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Configuration is handled by PowerShell scripts in [Run] section
  end;
end;
