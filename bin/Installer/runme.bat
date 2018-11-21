@echo off

rem runme.bat
rem usage : runme.bat <dir> <fileid> <noOfCopies> <printerType> <printerName>
rem 	dir         - current directory
rem 	fileid 		- file id
rem 	noOfCopies  - print copies
rem     printerType - LBL or NORMAL
rem     printerName - printer name  

rem executable path
set currentPath=%1
echo %currentPath%,"Done" >> %currentPath%\\tmp\\log.txt

set fileid=%2
echo %fileid%,"Done" >> %fileid%\\tmp\\log.txt

rem Set default to 1
set noOfCopies=%3
if %noOfCopies%=="" set noOfCopies="1x"
echo %noOfCopies%,"Done" >> %currentPath%\\tmp\\log.txt

rem type can be "LBL" or "NORMAL"
set printerType=%4
echo %printerType%,"Done" >> %currentPath%\\tmp\\log.txt

rem Get Printer name
set printerName=%5
echo %printerName%,"Done" >> %currentPath%\\tmp\\log.txt

rem %currentPath%\PDFtoPrinter.exe %currentPath%\0.pdf
if %printerType%==LBL (
	%currentPath%\PDFtoPrinter.exe %currentPath%\\tmp\\%fileid%.pdf %printerName% 
) else (%currentPath%\SumatraPDF.exe -print-to %printerName% -print-settings %noOfCopies% %currentPath%\\tmp\\0.pdf)


echo %noOfCopies% >> %currentPath%\\tmp\\log.txt
rem Start printing on default printer
rem C:\htecwinsvc\PDFtoPrinter.exe "C:\htecwinsvc\pg101.pdf"
rem %currentPath%\SumatraPDF.exe -print-to "SHARP MX-M264NV PCL6" -print-settings %noOfCopies% %currentPath%"\0.pdf"

echo "-----------------" >> %currentPath%\\tmp\\log.txt