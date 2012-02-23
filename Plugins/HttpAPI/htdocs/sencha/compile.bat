@echo off
pause
sencha create jsb -a index.html -p app.jsb3
pause
sencha build -p app.jsb3 -d ./
pause