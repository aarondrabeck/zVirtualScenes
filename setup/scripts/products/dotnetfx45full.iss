// requires Windows 7, Windows 7 Service Pack 1, Windows Server 2003 Service Pack 2, Windows Server 2008, Windows Server 2008 R2, Windows Server 2008 R2 SP1, Windows Vista Service Pack 1, Windows XP Service Pack 3
// requires Windows Installer 3.1
// requires Internet Explorer 5.01
// WARNING: express setup (downloads and installs the components depending on your OS) if you want to deploy it on cd or network download the full bootsrapper on website below
// http://www.microsoft.com/downloads/en/details.aspx?FamilyID=9cfb2d51-5ff4-4491-b0e5-b386f32c0992

[CustomMessages]
dotnetfx45full_title=.NET Framework 4.5 Full

dotnetfx45full_size=3 MB - 197 MB

;http://www.microsoft.com/globaldev/reference/lcid-all.mspx
en.dotnetfx45full_lcid=''


[Code]
const
	dotnetfx45full_url = 'http://download.microsoft.com/download/D/0/F/D0F564A3-6734-470B-9772-AC38B3B6D8C2/dotNetFx45_Full_setup.exe';

procedure dotnetfx45full();
begin
	if (not netfxinstalled(NetFx45Full, '')) then
		AddProduct('dotNetFx45_Full_setup.exe',
			CustomMessage('dotnetfx45full_lcid') + '/q /passive /norestart',
			CustomMessage('dotnetfx45full_title'),
			CustomMessage('dotnetfx45full_size'),
			dotnetfx45full_url,
			false, false);
end;