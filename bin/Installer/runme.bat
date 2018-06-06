@echo off

rem executable path
set currentPath=%1

rem Set default to 1
set noOfCopies=%2
if %noOfCopies%=="" set noOfCopies="1x"

echo %noOfCopies% >> %currentPath%"\log.txt"
rem Start printing on default printer
rem C:\htecwinsvc\PDFtoPrinter.exe "C:\htecwinsvc\pg101.pdf"
%currentPath%\SumatraPDF.exe -print-to-default -print-settings %noOfCopies% %currentPath%"\0.pdf"

echo "Done" >> %currentPath%"\log.txt"