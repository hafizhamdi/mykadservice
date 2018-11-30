@echo off

rem runme.bat
rem usage : runme.bat <dir> <fileid> <noOfCopies> <printerType> <printerName>
rem 	dir         - current directory
rem 	fileid 		- file id
rem 	noOfCopies  - print copies
rem     printerType - LBL or NORMAL
rem     printerName - printer name  
setlocal EnableDelayedExpansion
set DONE=0

rem executable path
set currentPath=%1
echo [%date%,%time%] Looking at %currentPath% >> %currentPath%\\tmp\\log.txt

set fileid=%2
echo [%date%,%time%] File ID:%fileid% >> %currentPath%\\tmp\\log.txt

rem Set default to 1
set noOfCopies=%3
if %noOfCopies%=="" set noOfCopies="1x"
echo [%date%,%time%] COPIES:%noOfCopies% >> %currentPath%\\tmp\\log.txt

rem type can be "LBL" or "NORMAL"
set printerType=%4
echo [%date%,%time%] Printer Type:%printerType% >> %currentPath%\\tmp\\log.txt

rem Get Printer name
set printerName=%5
echo [%date%,%time%] Printer Name:%printerName% >> %currentPath%\\tmp\\log.txt

if %printerType%==LBL (
	for /f "usebackq skip=1 tokens=*" %%a in (`wmic printer where "name like '%%LABEL%%'" get name ^| findstr /r /v "^$"`
	) do set myprinter=%%a
	
	FOR /L %%b IN (1,1,%noOfCopies%) DO (
		%currentPath%\\pdfcmd.exe command=printpdf input="%currentPath%\\tmp\\%fileid%.pdf" printer="%myprinter%"
	)
	echo [%date%,%time%] Shared Printer Name:!myprinter! >> %currentPath%\\tmp\\log.txt

	set DONE=1
) else (

	FOR /L %%b IN (1,1,%noOfCopies%) DO (
		%currentPath%\\pdfcmd.exe command=printpdf input="%currentPath%\\tmp\\%fileid%.pdf" printer="%printerName%"
	)
	set DONE=1)

if %DONE%==1 (
	echo [%date%,%time%] Printing started. >> %currentPath%\\tmp\\log.txt
)
echo ----------------------------------------------------------- >> %currentPath%\\tmp\\log.txt
