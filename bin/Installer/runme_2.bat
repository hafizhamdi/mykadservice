@echo off

rem executable path
set currentPath=%1

rem Set default to 1
set noOfCopies=%2
if %noOfCopies%=="" set noOfCopies="1x"

rem type can be "LBL" or "NORMAL"
set printerType=%3

rem Get Printer name
set printerName=%4

if %printerType%=="LBL" (
	%currentPath%\PDFtoPrinter.exe %currentPath%\0.pdf %printerName% 
) else (%currentPath%\SumatraPDF.exe -print-to %printerName% -print-settings %noOfCopies% %currentPath%"\0.pdf")

echo %noOfCopies% >> %currentPath%"\log.txt"
rem Start printing on default printer
rem C:\htecwinsvc\PDFtoPrinter.exe "C:\htecwinsvc\pg101.pdf"
rem %currentPath%\SumatraPDF.exe -print-to "SHARP MX-M264NV PCL6" -print-settings %noOfCopies% %currentPath%"\0.pdf"

echo "Done" >> %currentPath%"\log.txt"