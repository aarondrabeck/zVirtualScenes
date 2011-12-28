ECHO parameter=%1
CD %1
IF EXIST "htdocs" (RMDIR htdocs /s /q)
md "htdocs"
CD %1
xcopy /Y /s "..\..\Plugins\HttpAPI\htdocs\*" "htdocs\*"




