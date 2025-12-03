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
Source: "{#BasePath}\\Tools\\DetectMySQLPort.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "{#BasePath}\\Tools\\ValidateMySQLPassword.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "{#BasePath}\\Tools\\SetMySQLPassword.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
; Step 1: Detect available MySQL port (for bundled XAMPP) or auto-detect port (for existing XAMPP)
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\FindAvailablePort.ps1"" -OutputFile ""{{tmp}}\mysql_port.txt"""; StatusMsg: "Detecting available MySQL port..."; Flags: runhidden waituntilterminated; Check: ShouldInstallXAMPP
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\DetectMySQLPort.ps1"" -OutputFile ""{{tmp}}\mysql_port.txt"""; StatusMsg: "Auto-detecting MySQL port..."; Flags: runhidden waituntilterminated; Check: ShouldUseExistingXAMPP
; Step 1b: Validate MySQL password (for existing XAMPP only, skip if empty password)
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\ValidateMySQLPassword.ps1"" -PortFile ""{{tmp}}\mysql_port.txt"" -PasswordFile ""{{tmp}}\mysql_password.txt"""; StatusMsg: "Validating MySQL password..."; Flags: runhidden waituntilterminated; Check: ShouldValidatePassword
; Step 2: Configure MySQL my.ini with detected port and set password to 'admin' (only for bundled XAMPP)
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\ConfigureMySQL.ps1"" -XamppPath ""C:\FinalPOS-XAMPP"" -PortFile ""{{tmp}}\mysql_port.txt"""; StatusMsg: "Configuring MySQL server..."; Flags: runhidden waituntilterminated; Check: ShouldInstallXAMPP
; Step 3: Start MySQL server with configured port (only for bundled XAMPP)
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\StartMySQL.ps1"" -XamppPath ""C:\FinalPOS-XAMPP"" -PortFile ""{{tmp}}\mysql_port.txt"""; StatusMsg: "Starting MySQL server..."; Flags: runhidden waituntilterminated; Check: ShouldInstallXAMPP
; Step 4: Set MySQL root password to 'admin' (only for bundled XAMPP)
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\SetMySQLPassword.ps1"" -XamppPath ""C:\FinalPOS-XAMPP"" -Password ""admin"""; StatusMsg: "Setting MySQL root password..."; Flags: runhidden waituntilterminated; Check: ShouldInstallXAMPP
; Step 5: Update App.config connection string with port and password
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\UpdateAppConfig.ps1"" -AppConfigPath ""{{app}}\App.config"" -PortFile ""{{tmp}}\mysql_port.txt"" -PasswordFile ""{{tmp}}\mysql_password.txt"""; StatusMsg: "Updating application configuration..."; Flags: runhidden waituntilterminated
; Step 6: Save port to App folder for reference
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -Command ""if (Test-Path '{{tmp}}\mysql_port.txt') {{ $port = Get-Content '{{tmp}}\mysql_port.txt' -ErrorAction SilentlyContinue; if ($port) {{ Set-Content -Path '{{app}}\mysql_port.txt' -Value $port -ErrorAction Stop }} }}"""; StatusMsg: "Saving port configuration..."; Flags: runhidden waituntilterminated
; Step 7: Launch FinalPOS application
Filename: "{app}\{#MyAppExeName}"; Description: "Launch FinalPOS"; Flags: nowait postinstall skipifsilent

[Code]
var
  MySQLPage: TInputQueryWizardPage;
  UseExistingXAMPP: Boolean;
  MySQLPassword: String;

function InitializeSetup(): Boolean;
begin
  Result := True;
end;

function InitializeUninstall(): Boolean;
begin
  Result := True;
end;

procedure InitializeWizard;
begin
  // Create custom page for MySQL configuration
  MySQLPage := CreateInputQueryPage(wpSelectTasks,
    'MySQL Database Configuration', 'Choose your MySQL setup option',
    'Select whether you have existing XAMPP/MySQL installed or want to install bundled MySQL.');
  
  MySQLPage.Add('I have existing XAMPP/MySQL installed (Yes/No):', False);
  MySQLPage.Add('MySQL Root Password (leave empty if no password):', True);
  
  // Set default to "No" (install bundled)
  MySQLPage.Values[0] := 'No';
  MySQLPage.Values[1] := '';
end;

function ShouldInstallXAMPP(): Boolean;
begin
  Result := not UseExistingXAMPP;
end;

function ShouldUseExistingXAMPP(): Boolean;
begin
  Result := UseExistingXAMPP;
end;

function ShouldValidatePassword(): Boolean;
begin
  // Only validate password if using existing XAMPP and password is not empty
  Result := UseExistingXAMPP and (MySQLPassword <> '');
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  ResultCode: Integer;
  UseExisting: String;
  DetectedPort: String;
begin
  Result := True;
  
  if CurPageID = MySQLPage.ID then
  begin
    // Validate input
    UseExisting := Trim(LowerCase(MySQLPage.Values[0]));
    if (UseExisting = 'yes') or (UseExisting = 'y') then
    begin
      UseExistingXAMPP := True;
      MySQLPassword := Trim(MySQLPage.Values[1]);
      
      // Password is optional - empty password is allowed
      
      // Auto-detect MySQL port using inline PowerShell
      DetectedPort := '3306';
      if Exec('powershell.exe', '-ExecutionPolicy Bypass -Command "$port = 3306; $services = Get-Service -Name ''MySQL*'' -ErrorAction SilentlyContinue; if ($services) { foreach ($svc in $services) { $path = (Get-WmiObject Win32_Service -Filter \"Name=''$($svc.Name)''\").PathName; if ($path) { $mysqlDir = Split-Path (Split-Path $path.Replace(''\"'', ''''))); $ini = Join-Path $mysqlDir ''my.ini''; if (Test-Path $ini) { $content = Get-Content $ini -Raw; if ($content -match ''\[mysqld\]\s*port\s*=\s*(\d+)'') { $port = $matches[1]; break } } } } }; $port"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
      begin
        // Port detection attempted, use default 3306
      end;
      
      // Try to validate password using inline PowerShell (simplified check)
      // Note: Full validation will happen during installation phase
      if MsgBox('Password will be validated during installation. Continue?', mbConfirmation, MB_YESNO) = IDNO then
      begin
        Result := False;
        Exit;
      end;
      
      // Save password to temp file
      SaveStringToFile(ExpandConstant('{tmp}\mysql_password.txt'), MySQLPassword, False);
    end
    else
    begin
      UseExistingXAMPP := False;
      // For bundled XAMPP, password will be set to 'admin' automatically
      MySQLPassword := 'admin';
      SaveStringToFile(ExpandConstant('{tmp}\mysql_password.txt'), MySQLPassword, False);
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
