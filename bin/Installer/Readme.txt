Use Installer
-------------
Run htecwinsvc2.2 Installer. Finished.


Manual - Installation steps
---------------------------

1. Open cmd prompt in Administrator mode

2. Goto path C:\Windows\Microsoft.NET\Framework\v4.0.30319

3. Type InstallUtil -i <Service dir>\ServiceConsole.exe

4. Press Enter. Installation will start.

5. Open Services via search under start button.

6. Search for htecwinsvc service. Start the service.

7. If error exception occurs during installation, uninstall service.
	Type: InstallUtil /u <Service dir>\ServiceConsole.exe
	Uninstall success.

8. Reinstall service.


Testing installed service
-------------------------

1. Open Browser Chrome.

2. Type url : http://localhost:8000/Hello
	Output: "Hello Im In"

3. Type url : http://localhost:8000/readmykad
	Output: Mykad data in json format

4. Type url : http://localhost:8000/readmykid
	Output: Mykid data in json format

5. Type url : http://localhost:8000/getHostName
	Output: PC hostname 

%%%%%%%%%%%%%%%%%%%%
%% htecwinsvcv2.2 %%
%%%%%%%%%%%%%%%%%%%%

# User can print pdf file in base64 from web browser
1. Use example index.html provided in installation folder as reference.
2. Modify with the application.

3. Call http://localhost:8000/PostPdf/{id}
	Ex: http://localhost:8000/PostPdf/pg101
	to post generated base64 from apps to service
  
4. Call http://localhost:8000/ExecPrint/<Installation-folder>/runme.bat
	Ex: http://localhost:8000/ExecPrint/htecwinsvc/runme.bat
	to generate base64 to pdf file in installation folder and direct print.

6. You're done installation.
	
Note: For Internet Explorer, if data didn't display out instead ask to download .json
	Close IE, Run json-ie.reg included in the folder and restart IE.

Upgrade Version:
v1 - handle mykad
v2 - additional mykid support
v2.1 - get hostname
v2.2 - allow direct print from web


Minimum Requirement:

Supported OS: Windows 7/ 10
.Net Framework: v4.7.1

Thank you for using this service.

Credits
~hafizh1357
hafizhamdi.10@gmail.com

