; ========================================
; FinalPOS Installer Script
; Inno Setup - Portable MySQL Integration
; ========================================

#define MyAppName "FinalPOS"
#define MyAppVersion "1.0"
#define MyAppPublisher "FinalPOS"
#define MyAppExeName "FinalPOS.exe"
#define MySQLPort "3310"

[Setup]
; App Information
AppId={{FCF53D94-B95B-45E5-9109-BDAEDA2141B6}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=
OutputDir=Installer
OutputBaseFilename=FinalPOS-Setup
SetupIconFile=
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
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
; Application Files
Source: "FinalPOS\bin\Release\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; MySQL Portable Database
Source: "mysql\*"; DestDir: "{app}\mysql"; Flags: ignoreversion recursesubdirs createallsubdirs
; Configuration Files
Source: "InstallerFiles\config\my.ini"; DestDir: "{app}\mysql"; Flags: ignoreversion
; Scripts
Source: "InstallerFiles\scripts\*"; DestDir: "{app}\scripts"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
; Run MySQL installation script
Filename: "{app}\scripts\Install-MySQL.bat"; Parameters: """{app}"""; StatusMsg: "Installing MySQL Database..."; Flags: runhidden waituntilterminated
; Launch application after installation
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallRun]
; Stop MySQL before uninstall
Filename: "{app}\scripts\Stop-MySQL.bat"; Parameters: """{app}"""; RunOnceId: "StopMySQL"; Flags: runhidden waituntilterminated

[Code]
// Custom code to handle installation directory updates in my.ini
procedure CurStepChanged(CurStep: TSetupStep);
var
  IniFile: string;
  Content: TArrayOfString;
  i: Integer;
begin
  if CurStep = ssPostInstall then
  begin
    IniFile := ExpandConstant('{app}\mysql\my.ini');
    if LoadStringsFromFile(IniFile, Content) then
    begin
      for i := 0 to GetArrayLength(Content) - 1 do
      begin
        StringChangeEx(Content[i], '{INSTALLDIR}', ExpandConstant('{app}'), True);
      end;
      SaveStringsToFile(IniFile, Content, False);
    end;
  end;
end;

function InitializeUninstall(): Boolean;
var
  StopMySQL: string;
  ResultCode: Integer;
begin
  // Stop MySQL before uninstall
  StopMySQL := ExpandConstant('{app}\scripts\Stop-MySQL.bat');
  if FileExists(StopMySQL) then
  begin
    Exec(StopMySQL, ExpandConstant('"""{app}"""'), '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
  end;
  Result := True;
end;

