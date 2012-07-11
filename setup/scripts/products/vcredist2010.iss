// requires Windows 7, Windows 7 Service Pack 1, Windows Server 2003 Service Pack 2, Windows Server 2008, Windows Server 2008 R2, Windows Server 2008 R2 SP1, Windows Vista Service Pack 1, Windows XP Service Pack 3
// requires Windows Installer 3.1 or later
// requires Internet Explorer 5.01 or later
// http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992

[CustomMessages]
vcredist2010_title=Visual C++ 2010 Redistributable SP1

en.vcredist2010_size=4.8 MB
en.vcredist2010_size_x64=5.5 MB 
en.vcredist2010_size_ia64=2.2 MB

;http://www.microsoft.com/globaldev/reference/lcid-all.mspx
en.vcredist2010_lcid=''


[Code]
const
	vcredist2010_url = 'http://download.microsoft.com/download/C/6/D/C6D0FD4E-9E53-4897-9B91-836EBA2AACD3/vcredist_x86.exe';
	vcredist2010_url_x64 = 'http://download.microsoft.com/download/C/6/D/C6D0FD4E-9E53-4897-9B91-836EBA2AACD3/vcredist_x86.exe';
	vcredist2010_url_ia64 = 'http://download.microsoft.com/download/3/3/A/33A75193-2CBC-424E-A886-287551FF1EB5/vcredist_IA64.exe';

procedure vcredist2010();
var
	version: cardinal;
begin
	RegQueryDWordValue(HKLM, GetString('SOFTWARE\Microsoft\VisualStudio\10.0\VC\VCRedist\x86', 'SOFTWARE\Wow6432Node\Microsoft\VisualStudio\10.0\VC\VCRedist\x86', 'SOFTWARE\Wow6432Node\Microsoft\VisualStudio\10.0\VC\VCRedist\x86') , 'Installed', version);
                                                                                              
                                                                                              

	if (version <> 1) then
		AddProduct('vcredist2010' + GetArchitectureString() + '.exe',
			CustomMessage('vcredist2010_lcid') + '/passive /norestart',
			CustomMessage('vcredist2010_title'),
			CustomMessage('vcredist2010_size' + GetArchitectureString()),
			vcredist2010_url,
			false, false);
end;