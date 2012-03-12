ECHO parameter=%1
CD %1
IF EXIST "htdocs" (RMDIR htdocs /s /q)
md "htdocs"
CD htdocs
IF EXIST "sencha" (RMDIR htdocs /s /q)
md "sencha"
CD %1
xcopy /Y /s "..\..\Plugins\HttpAPI\htdocs\*" "htdocs\*"
xcopy /Y "..\..\Plugins\HttpAPI\sencha\index.html" "htdocs\sencha\"
xcopy /Y "..\..\Plugins\HttpAPI\sencha\all-classes.js" "htdocs\sencha\"
xcopy /Y "..\..\Plugins\HttpAPI\sencha\app.js" "htdocs\sencha\"
xcopy /Y /s "..\..\Plugins\HttpAPI\sencha\touch\sencha-touch.js" "htdocs\sencha\touch\"
xcopy /Y /s "..\..\Plugins\HttpAPI\sencha\resources\*" "htdocs\sencha\resources\*"



