@echo off

set path=C:\Windows\Microsoft.NET\Framework\v4.0.30319
echo %path%
if not exist %path% ( 
	echo msgbox "Microsoft .NET Framework v4.7 is not installed in this system. Update or Install to continue" > "%temp%\msgbox.vbs" 
	wscript.exe "%temp%\msgbox.vbs"
) else (
	
	REM Install service
	%path%\InstallUtil.exe -i %cd%\ServiceConsole.exe
	echo OK. 
)
