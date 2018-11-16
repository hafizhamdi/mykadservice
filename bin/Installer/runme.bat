@echo off

rem runme.bat
rem usage : runme.bat <dir> <noOfCopies> <printerType> <printerName>
rem 	dir         - current directory
rem 	noOfCopies  - print copies
rem     printerType - LBL or NORMAL
rem     printerName - printer name  

rem executable path
set currentPath=%1
echo %currentPath%,"Done" >> %currentPath%"\log.txt"

rem Set default to 1
set noOfCopies=%2
if %noOfCopies%=="" set noOfCopies="1x"
echo %noOfCopies%,"Done" >> %currentPath%"\log.txt"

rem type can be "LBL" or "NORMAL"
set printerType=%3
echo %printerType%,"Done" >> %currentPath%"\log.txt"

rem Get Printer name
set printerName=%~4
echo %printerName%,"Done" >> %currentPath%"\log.txt"

rem %currentPath%\PDFtoPrinter.exe %currentPath%\0.pdf
if %printerType%==LBL (
	%currentPath%\PDFtoPrinter.exe %currentPath%\0.pdf %printerName% 
) else (%currentPath%\SumatraPDF.exe -print-to %printerName% -print-settings %noOfCopies% %currentPath%"\0.pdf")


echo %noOfCopies% >> %currentPath%"\log.txt"
rem Start printing on default printer
rem C:\htecwinsvc\PDFtoPrinter.exe "C:\htecwinsvc\pg101.pdf"
rem %currentPath%\SumatraPDF.exe -print-to "SHARP MX-M264NV PCL6" -print-settings %noOfCopies% %currentPath%"\0.pdf"

echo "-----------------" >> %currentPath%"\log.txt"