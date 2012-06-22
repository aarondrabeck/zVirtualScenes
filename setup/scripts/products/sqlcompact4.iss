[CustomMessages]
sqlcompact4_title=SQL Server Compact 4

en.sqlcompact4_size=2.5 MB

[Code]
    
const
	sqlcompact4_url = 'http://download.microsoft.com/download/6/9/5/695ED23C-F40F-420C-98D1-0F752B8D4E2B/1033/x86/SSCERuntime_x86-ENU.msi';
	sqlcompact4_url_x64 = 'http://download.microsoft.com/download/6/9/5/695ED23C-F40F-420C-98D1-0F752B8D4E2B/1033/x64/SSCERuntime_x64-ENU.msi';

  const
	sqlcompact4_fn = 'SSCERuntime_x86-ENU.msi';
	sqlcompact4_fn_x64 = 'SSCERuntime_x64-ENU.msi';

procedure sqlcompact4();
begin
	if (not RegKeyExists(HKLM, 'SOFTWARE\Microsoft\Microsoft SQL Server Compact Edition\v4.0\ENU')) then
    AddProduct(GetString(sqlcompact4_fn, sqlcompact4_fn_x64, ''),
			'/qb',
			CustomMessage('sqlcompact4_title'),
			CustomMessage('sqlcompact4_size'),
			GetString(sqlcompact4_url, sqlcompact4_url_x64, ''),
			false, false);
end;
