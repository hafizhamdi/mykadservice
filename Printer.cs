using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WIA;

namespace ServiceConsole
{
    class Printer
    {
        public string pdfBase64;
        public string fileid;
        public int numOfCopies;
        public string printerName;
        public string printerType;

        public Printer() { }

        public Printer(string base64, string id)
        {
            pdfBase64 = base64;
            fileid = id;
            numOfCopies = 1;
            printerName = "";
            printerType = "NORMAL";
        }
        
        public Printer(string base64, string id, int copies, string prtName)
        {
            pdfBase64 = base64;
            fileid = id;
            numOfCopies = copies;
            printerName = prtName;
            printerType = "NORMAL";
        }

        public Printer(string base64, string id, int copies, string prtName, string prtType)
        {
            pdfBase64 = base64;
            fileid = id;
            numOfCopies = copies;
            printerName = prtName;
            printerType = prtType;
        }

        public static void RunExecutable(string executable, string arguments)
        {
            ProcessStartInfo starter = new ProcessStartInfo(executable, arguments);
            starter.CreateNoWindow = true;
            starter.RedirectStandardOutput = true;
            starter.UseShellExecute = false;
            Process process = new Process();
            process.StartInfo = starter;
            process.Start();
            StringBuilder buffer = new StringBuilder();
            using (StreamReader reader = process.StandardOutput)
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    buffer.Append(line);
                    buffer.Append(Environment.NewLine);
                    line = reader.ReadLine();
                    Thread.Sleep(100);
                }
            }
            if (process.ExitCode != 0)
            {
                throw new Exception(string.Format(@"""{0}"" exited with ExitCode {1}. Output: {2}",
                 executable, process.ExitCode, buffer.ToString()));
            }
        }
    }
}
