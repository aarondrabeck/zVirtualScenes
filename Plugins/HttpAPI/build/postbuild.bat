ECHO parameter=%1
CD %1
IF EXIST "htdocs" (RMDIR htdocs /s /q)
md "htdocs"
CD htdocs
IF EXIST "sencha" (RMDIR htdocs /s /q)
md "sencha"
CD %1
xcopy /Y /s "..\..\Plugins\HttpAPI\htdocs\*" "htdocs\*"
xcopy /Y /s "..\..\Plugins\HttpAPI\zvsMobile\build\zvsMobile\production\*" "htdocs\sencha\*"



