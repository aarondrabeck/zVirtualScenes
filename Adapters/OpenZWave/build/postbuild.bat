ECHO parameter=%1
CD %1
IF NOT EXIST "config" (md "config")
CD %1
xcopy /Y /s "..\..\..\Adapters\OpenZWave\config\*" "config\*"




