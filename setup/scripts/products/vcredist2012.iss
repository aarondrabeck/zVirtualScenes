// requires Windows 7, Windows 7 Service Pack 1, Windows Server 2003 Service Pack 2, Windows Server 2008, Windows Server 2008 R2, Windows Server 2008 R2 SP1, Windows Vista Service Pack 1, Windows XP Service Pack 3
// requires Windows Installer 3.1 or later
// requires Internet Explorer 5.01 or later
// http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992

[CustomMessages]
vcredist2012_title=Visual C++ Redistributable for Visual Studio 2012 Update 1

en.vcredist2012_size=6.3 MB
en.vcredist2012_size_x64=6.9 MB 
en.vcredist2012_size_ia64=6.9 MB

;http://www.microsoft.com/globaldev/reference/lcid-all.mspx
en.vcredist2012_lcid=''


[Code]
const
	vcredist2012_url = 'http://download.microsoft.com/download/1/6/B/16B06F60-3B20-4FF2-B699-5E9B7962F9AE/VSU1/vcredist_x86.exe';                                          
                                       
                                                      
procedure vcredist2012();
var
	version: cardinal;
begin
	RegQueryDWordValue(HKLM, GetString('SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{E824E81C-80A4-3DFF-B5F9-4842A9FF5F7F}', 
  'SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{8e70e4e1-06d7-470b-9f74-a51bef21088e}', 
  'SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\{8e70e4e1-06d7-470b-9f74-a51bef21088e}') , 'Installed', version);

               

	if (version <> 1) then
		AddProduct('vcredist2012' + GetArchitectureString() + '.exe',
			CustomMessage('vcredist2012_lcid') + '/passive /norestart',
			CustomMessage('vcredist2012_title'),
			CustomMessage('vcredist2012_size' + GetArchitectureString()),
			vcredist2012_url,
			false, false);
end;