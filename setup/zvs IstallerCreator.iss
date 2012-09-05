; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!
#define use_dotnetfx45
#define use_sqlcompact4
#define use_vc2010

#define MyAppName "zVirtualScenes"
;#define MyAppVersion "{#VERSION_NAME}"
#define MyAppPublisher "Nonce Labs"
#define MyAppExeName "zVirtualScenes.exe"

[Setup]                            
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
PrivilegesRequired=admin
AppId={{09FA2C8A-D475-4DC3-A895-107587BF4B90}
AppMutex=zVirtualScenesGUIMutex
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppCopyright=Copyright � {#MyAppPublisher} 2012
VersionInfoVersion={#MyAppVersion}
VersionInfoCompany={#MyAppPublisher}
AppPublisher={#MyAppPublisher}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
UninstallDisplayIcon={app}\zVirtualScenesGUI.exe
AllowNoIcons=yes
;OutputDir=;
OutputBaseFilename={#MyAppName} Setup {#MyAppVersion}
Compression=lzma
SolidCompression=yes
OutputDir=output

ArchitecturesAllowed=x86 x64 ia64
ArchitecturesInstallIn64BitMode=x64 ia64

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "..\output\EntityFramework.dll"; DestDir: "{app}"; Flags: ignoreversion           
Source: "..\output\EntityFramework.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\output\WPFToolkit.Extended.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\output\zvs.zVirtualScenes.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\output\zVirtualScenes.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\output\zVirtualScenes.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\output\zvs.Entities.dll"; DestDir: "{app}"; Flags: ignoreversion 
 

Source: "..\output\plugins\dnssd.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\EntityFramework.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion           
Source: "..\output\EntityFramework.xml"; DestDir: "{app}\plugins"; Flags: ignoreversion   
;Source: "..\output\plugins\GlobalHotKeyPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
;Source: "..\output\plugins\GlobalHotKeyPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\Growl.Connector.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\Growl.CoreLibrary.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\GrowlPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\GrowlPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\HttpAPI.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\HttpAPI.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion 
Source: "..\output\plugins\LightSwitchPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\LightSwitchPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\NOAAPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\NOAAPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\OpenZWaveDotNet.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\OpenZWavePlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\OpenZWavePlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\SpeechPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\SpeechPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\ZeroconfService.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\zvs.zVirtualScenes.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\zvs.Entities.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\htdocs\*"; DestDir: "{app}\plugins\htdocs"; Flags: ignoreversion recursesubdirs
Source: "..\output\plugins\config\*"; DestDir: "{app}\plugins\config"; Flags: ignoreversion recursesubdirs  
Source: "..\output\plugins\ControlThink.ZWave.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\ThinkStickHIDPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\output\plugins\ThinkStickHIDPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall runascurrentuser 

#include "scripts\products.iss"
#include "scripts\products\stringversion.iss"
#include "scripts\products\winversion.iss"
#include "scripts\products\fileversion.iss"
#include "scripts\products\dotnetfxversion.iss"

#ifdef use_sqlcompact4
#include "scripts\products\sqlcompact4.iss"
#endif

#ifdef use_dotnetfx45
#include "scripts\products\dotnetfx45full.iss"
#endif

#ifdef use_vc2010
#include "scripts\products\vcredist2010.iss"
#endif


[Code]
function InitializeSetup(): boolean;
begin
	//init windows version
	initwinversion();

  	// if no .netfx 4.5 is found, install the full (smallest)
#ifdef use_dotnetfx45
if (not netfxinstalled(NetFx45Full, '')) then
		dotnetfx45full();
#endif

#ifdef use_sqlcompact4
sqlcompact4();
#endif

#ifdef use_vc2010
	vcredist2010();
#endif


	Result := true;
end;