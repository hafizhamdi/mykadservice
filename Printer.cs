using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceConsole
{
    class Printer
    {
        public static string pdfBase64;
        public static string fileid;
        public static int numOfCopies;
        public static string printerName;
        public static string printerType;
        public static string srcPath;

        public Printer() { }

        public Printer(string base64, string id)
        {
            pdfBase64 = base64;
            fileid = id;
        }
        
        public Printer(string base64, string id, int copies, string prtName)
        {
            pdfBase64 = base64;
            fileid = id;
            numOfCopies = copies;
            printerName = prtName;
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
