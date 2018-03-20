Installation steps
------------------

1. Open cmd prompt in Administrator mode

2. Goto path C:\Windows\Microsoft.NET\Framework\v4.0.30319

3. Type InstallUtil -i <Service dir>\ServiceConsole.exe

4. Press Enter. Installation will start.

5. Open Services via search under start button.

6. Search for Heitech WinService. Start the service.

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

6. You're done installation.
	
Note: For Internet Explorer, if data didn't display out instead ask to download .json
	Close IE, Run json-ie.reg included in the folder and restart IE.

Upgrade Version:
v1 - handle mykad
v2 - additional mykid support
v2.1 - get hostname


Minimum Requirement:

Supported OS: Windows 7/ 10
.Net Framework: v4.7

Thank you for using this service.

Credits
~hafizh1357
hafizhamdi.10@gmail.com

