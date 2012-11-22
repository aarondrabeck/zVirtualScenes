; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!
#define use_dotnetfx45
#define use_sqlcompact4
#define use_vc2010

#define MyAppExeName "zVirtualScenes.exe"
#define MyAppName "zVirtualScenes"
#define MyAppVersion GetFileVersion(AddBackslash("..\bin\release\") + MyAppExeName)
#define MyAppPublisher "Nonce Labs"

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

[Types]
Name: "full"; Description: "Full installation"
Name: "compact"; Description: "Compact installation"
Name: "custom"; Description: "Custom installation"; Flags: iscustom

[Components]
Name: "core"; Description: "zVirtualScenes Core"; Types: full compact custom; Flags: fixed
Name: "jabber"; Description: "Jabber Plug-in"; Types: full
Name: "growl"; Description: "Growl Plug-in"; Types: full
Name: "http"; Description: "HTTP API Plug-in"; Types: full
Name: "webapi"; Description: "Web API Plug-in"; Types: full
Name: "lightswitch"; Description: "LightSwitch Server Plug-in"; Types: full
Name: "noaa"; Description: "NOAA Plug-in"; Types: full
Name: "openzwave"; Description: "OpenZWave Plug-in"; Types: full compact
Name: "controlthink"; Description: "ControlThink Plug-in"; Types: full
Name: "speech"; Description: "Speech Plug-in"; Types: full

[Files]
Source: "..\bin\release\EntityFramework.dll"; DestDir: "{app}"; Flags: ignoreversion           
Source: "..\bin\release\Jint.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\release\SciLexer.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\release\SciLexer64.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\release\ScintillaNET.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\release\ScintillaNET.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\release\EntityFramework.xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\release\WPFToolkit.Extended.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\release\zvs.zVirtualScenes.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\release\zVirtualScenes.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\release\zVirtualScenes.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\bin\release\zvs.Entities.dll"; DestDir: "{app}"; Flags: ignoreversion 
Source: "..\bin\release\log4net.dll"; DestDir: "{app}"; Flags: ignoreversion 
Source: "..\bin\release\log4net.config"; DestDir: "{app}"; Flags: ignoreversion
 

Source: "..\bin\release\plugins\dnssd.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\EntityFramework.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion           
Source: "..\bin\release\EntityFramework.xml"; DestDir: "{app}\plugins"; Flags: ignoreversion   
;Source: "..\bin\release\plugins\GlobalHotKeyPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
;Source: "..\bin\release\plugins\GlobalHotKeyPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\Growl.Connector.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\Growl.CoreLibrary.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\GrowlPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\GrowlPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\HttpAPI.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\HttpAPI.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion 
Source: "..\bin\release\plugins\LightSwitchPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\LightSwitchPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\NOAAPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\NOAAPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\OpenZWaveDotNet.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\OpenZWavePlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\OpenZWavePlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\SpeechPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\SpeechPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\ZeroconfService.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\zvs.zVirtualScenes.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\plugins\zvs.Entities.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion
Source: "..\bin\release\scripts\*"; DestDir: "{app}\scripts"; Flags: ignoreversion recursesubdirs 
    
Source: "..\bin\release\plugins\jabber-net.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion; Components: jabber  
Source: "..\bin\release\plugins\JabberPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion; Components: jabber  
Source: "..\bin\release\plugins\JabberPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion; Components: jabber

Source: "..\bin\release\plugins\Growl.Connector.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion; Components: growl
Source: "..\bin\release\plugins\Growl.CoreLibrary.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion; Components: growl
Source: "..\bin\release\plugins\GrowlPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion; Components: growl
Source: "..\bin\release\plugins\GrowlPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion; Components: growl

Source: "..\bin\release\plugins\HttpAPI.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion ; Components: http
Source: "..\bin\release\plugins\HttpAPI.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion; Components: http 

Source: "..\bin\release\plugins\LightSwitchPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion ; Components: lightswitch
Source: "..\bin\release\plugins\LightSwitchPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion  ; Components: lightswitch
Source: "..\bin\release\plugins\dnssd.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion   ; Components: lightswitch

Source: "..\bin\release\plugins\NOAAPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion     ; Components: noaa
Source: "..\bin\release\plugins\NOAAPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion ; Components: noaa

Source: "..\bin\release\plugins\OpenZWaveDotNet.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion  ; Components: openzwave
Source: "..\bin\release\plugins\OpenZWavePlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion   ; Components: openzwave
Source: "..\bin\release\plugins\OpenZWavePlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion ; Components: openzwave
Source: "..\bin\release\plugins\ZeroconfService.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion     ; Components: openzwave
Source: "..\bin\release\plugins\config\*"; DestDir: "{app}\plugins\config"; Flags: ignoreversion recursesubdirs    ; Components: openzwave

Source: "..\bin\release\plugins\SpeechPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion      ; Components: speech
Source: "..\bin\release\plugins\SpeechPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion   ; Components: speech
                                                                            
Source: "..\bin\release\plugins\ControlThink.ZWave.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion      ; Components: controlthink
Source: "..\bin\release\plugins\ThinkStickHIDPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion     ; Components: controlthink
Source: "..\bin\release\plugins\ThinkStickHIDPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion  ; Components: controlthink


Source: "..\bin\release\plugins\HttpAPI.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion      ; Components: webapi
Source: "..\bin\release\plugins\HttpAPI.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion     ; Components: webapi
Source: "..\bin\release\plugins\Microsoft.Data.Entity.Design.Extensibility.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion  ; Components: webapi
Source: "..\bin\release\plugins\Microsoft.Data.OData.Contrib.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion  ; Components: webapi
Source: "..\bin\release\plugins\Microsoft.Data.OData.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion  ; Components: webapi
Source: "..\bin\release\plugins\Newtonsoft.Json.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion  ; Components: webapi
Source: "..\bin\release\plugins\System.Net.Http.Formatting.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion  ; Components: webapi
Source: "..\bin\release\plugins\System.Web.Http.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion  ; Components: webapi
Source: "..\bin\release\plugins\System.Web.Http.OData.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion  ; Components: webapi
Source: "..\bin\release\plugins\System.Web.Http.SelfHost.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion  ; Components: webapi
Source: "..\bin\release\plugins\WebAPIPlugin.dll.config"; DestDir: "{app}\plugins"; Flags: ignoreversion  ; Components: webapi
Source: "..\bin\release\plugins\WebAPIPlugin.dll"; DestDir: "{app}\plugins"; Flags: ignoreversion  ; Components: webapi


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