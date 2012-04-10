ECHO parameter=%1
CD %1
IF EXIST "htdocs" (RMDIR htdocs /s /q)
md "htdocs"
CD htdocs
IF EXIST "sencha" (RMDIR htdocs /s /q)
md "sencha"
CD %1
xcopy /Y /s "..\..\Plugins\HttpAPI\htdocs\*" "htdocs\*"
xcopy /Y "..\..\Plugins\HttpAPI\SenchaApp\index.html" "htdocs\sencha\"
xcopy /Y "..\..\Plugins\HttpAPI\SenchaApp\all-classes.js" "htdocs\sencha\"
xcopy /Y "..\..\Plugins\HttpAPI\SenchaApp\app.js" "htdocs\sencha\"
xcopy /Y /s "..\..\Plugins\HttpAPI\SenchaApp\touch\sencha-touch.js" "htdocs\sencha\touch\"
xcopy /Y /s "..\..\Plugins\HttpAPI\SenchaApp\resources\*" "htdocs\sencha\resources\*"



