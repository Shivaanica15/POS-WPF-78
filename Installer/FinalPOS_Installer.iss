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
; XAMPP files - Extract XAMPP Portable to InstallerFiles\XAMPP\
Source: "{#BasePath}\\XAMPP\\*"; DestDir: "C:\\FinalPOS-XAMPP"; Flags: ignoreversion recursesubdirs createallsubdirs
; Helper scripts - PowerShell scripts in InstallerFiles\Tools\
Source: "{#BasePath}\\Tools\\FindAvailablePort.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "{#BasePath}\\Tools\\ConfigureMySQL.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "{#BasePath}\\Tools\\UpdateAppConfig.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "{#BasePath}\\Tools\\StartMySQL.ps1"; DestDir: "{tmp}"; Flags: deleteafterinstall

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
; Step 1: Detect available MySQL port (3306-3315)
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\FindAvailablePort.ps1"" -OutputFile ""{{tmp}}\mysql_port.txt"""; StatusMsg: "Detecting available MySQL port..."; Flags: runhidden waituntilterminated
; Step 2: Configure MySQL my.ini with detected port
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\ConfigureMySQL.ps1"" -XamppPath ""C:\FinalPOS-XAMPP"" -PortFile ""{{tmp}}\mysql_port.txt"""; StatusMsg: "Configuring MySQL server..."; Flags: runhidden waituntilterminated
; Step 3: Update App.config connection string with port
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\UpdateAppConfig.ps1"" -AppConfigPath ""{{app}}\App.config"" -PortFile ""{{tmp}}\mysql_port.txt"""; StatusMsg: "Updating application configuration..."; Flags: runhidden waituntilterminated
; Step 4: Start MySQL server with configured port
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -File ""{{tmp}}\StartMySQL.ps1"" -XamppPath ""C:\FinalPOS-XAMPP"" -PortFile ""{{tmp}}\mysql_port.txt"""; StatusMsg: "Starting MySQL server..."; Flags: runhidden waituntilterminated
; Step 5: Save port to App folder for reference
Filename: "powershell.exe"; Parameters: "-ExecutionPolicy Bypass -Command ""if (Test-Path '{{tmp}}\mysql_port.txt') {{ $port = Get-Content '{{tmp}}\mysql_port.txt' -ErrorAction SilentlyContinue; if ($port) {{ Set-Content -Path '{{app}}\mysql_port.txt' -Value $port -ErrorAction Stop }} }}"""; StatusMsg: "Saving port configuration..."; Flags: runhidden waituntilterminated
; Step 6: Launch FinalPOS application
Filename: "{app}\{#MyAppExeName}"; Description: "Launch FinalPOS"; Flags: nowait postinstall skipifsilent

[Code]
var
  PortNumber: Integer;

function InitializeSetup(): Boolean;
begin
  Result := True;
end;

function InitializeUninstall(): Boolean;
begin
  Result := True;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
  PortFile: string;
begin
  if CurStep = ssPostInstall then
  begin
    // Port detection is handled by PowerShell scripts in [Run] section
  end;
end;
