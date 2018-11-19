﻿using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using Newtonsoft.Json;

using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Runtime.Serialization;
using System.IO;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Windows.Forms;
using WIA;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;


/*  Author: hafizh
 *  Date: 13 Dec 2017
 * 
 *  Windows Service:
 *  i   - ver 1.0 : (13-Dec-18) Handle Mykad
 *  ii  - ver 2.0 : (01-Jan-18) Handle Mykid
 *  iii - ver 2.1 : (13-Mar-18) Add hostname 
 *  iv  - ver 2.2&2.3 : (29-Mar-18) Add printer
 *  v   - ver 2.4 : (07-Nov-18) Add Printer name and set environment
 *  
     */

namespace ServiceConsole
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json), CorsEnabled]
        string Hello();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
           RequestFormat = WebMessageFormat.Json), CorsEnabled]
        string readmykad();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
           RequestFormat = WebMessageFormat.Json), CorsEnabled]
        string readmykid();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
           RequestFormat = WebMessageFormat.Json), CorsEnabled]
        string getHostName();
        
        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedResponse,
            //RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/PostPdf/{id}"), CorsEnabled]
        string PostPdf(Pdf data, string id);

        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedResponse,
            //RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/PostPdfCopies?numOfCopies={copies}&prtName={prtName}"), CorsEnabled]
        string PostPdfCopies(Pdf data, int copies, string prtName);

        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedResponse,
            //RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/PDFtoLabelPrinter?printerName={prtName}&printerType={prtType}"), CorsEnabled]
        string PDFtoLabelPrinter(Pdf data, string prtName, string prtType);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
           RequestFormat = WebMessageFormat.Json,
           UriTemplate = "/ExecPrint/{drive}/{path}/{progName}"), CorsEnabled]
        string ExecPrint(string drive, string path, string progName);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
           RequestFormat = WebMessageFormat.Json,
           UriTemplate = "/GetEnvironment?envName={envName}"), CorsEnabled]
        string GetEnvironment(string envName);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            UriTemplate = "/SetEnvironment?envName={envName}&envValue={envValue}"), CorsEnabled]
        string SetEnvironment(string envName, string envValue);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            UriTemplate = "/ScanDocument?fileid={fileid}"), CorsEnabled]
        string ScanDocument(String fileid);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            UriTemplate = "/RetrieveScannedDoc?fileid={fileid}"), CorsEnabled]
        string RetrieveScannedDoc(string fileid);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            UriTemplate = "/ListPrinter"), CorsEnabled]
        string ListPrinter();

    }

    
    
    public class Service : IService
    {
        Printer p;
        Scanner device;

        public string Hello()
        {
            return "Hello Im In!";
        }

        public string readmykad()
        {
            MyKad.Init();
            MyKad.SelectJPN();
            MyKad.ReadInfoMyKad();
            MyKad.Flush();

            Person person = new Person();

            person.name = MyKad.name.Trim(' ');
            person.origname = MyKad.origname.Trim(' ');
            person.gpcname = MyKad.gpcname.Trim(' ');
            person.icno = MyKad.icno.Trim(' ');
            person.dob = MyKad.dob.Trim(' ');
            person.pob = MyKad.pob.Trim(' ');
            person.gender = MyKad.gender.Trim(' ');
            person.religion = MyKad.religion.Trim(' ');
            person.race = MyKad.race.Trim(' ');
            person.citizenship = MyKad.citizenship.Trim(' ');
            person.addr1 = MyKad.addr1.Trim(' ');
            person.addr2 = MyKad.addr2.Trim(' ');
            person.addr3 = MyKad.addr3.Trim(' ');
            person.city = MyKad.city.Trim(' ');
            person.poscode = MyKad.poscode.Trim(' ');
            person.state = MyKad.state.Trim(' ');
            person.photo = MyKad.photo;

            string sPerson = JsonConvert.SerializeObject(person);

            return sPerson;
        }

        public string readmykid()
        {
            MyKad.Init();
            MyKad.SelectJPN();
            MyKad.ReadInfoMyKid();
            MyKad.Flush();

            Person person = new Person();

            person.name = MyKad.name.Trim(' ');
            person.icno = MyKad.icno.Trim(' ');
            person.regno = MyKad.regno.Trim(' ');
            person.dob = MyKad.dob.Trim(' ');
            person.tob = MyKad.tob.Trim(' ');
            person.pob = MyKad.pob.Trim(' ');
            person.gender = MyKad.gender.Trim(' ');
            person.religion = MyKad.religion.Trim(' ');
            person.citizenship = MyKad.citizenship.Trim(' ');
            person.addr1 = MyKad.addr1.Trim(' ');
            person.addr2 = MyKad.addr2.Trim(' ');
            person.addr3 = MyKad.addr3.Trim(' ');
            person.city = MyKad.city.Trim(' ');
            person.poscode = MyKad.poscode.Trim(' ');
            person.state = MyKad.state.Trim(' ');

            string sPerson = JsonConvert.SerializeObject(person);

            return sPerson;
        }

        public string getHostName()
        {
            Attributes attr = new Attributes();
            attr.hostname = Environment.MachineName;
           
            string result = JsonConvert.SerializeObject(attr);
            return result;
        }

        public string PostPdf(Pdf data, string id)
        {
            var myPdf = JsonConvert.SerializeObject(data);
            var base64 = data.Base64;
          
            p = new Printer(base64.ToString(), id);
            return base64.ToString();
        }

        
        public string PostPdfCopies(Pdf data, int copies, string prtName)
        {
            var myPdf = JsonConvert.SerializeObject(data);
            var base64 = data.Base64;

            p = new Printer(base64.ToString(), "0", copies, prtName);
            return base64.ToString();
        }

        public string PDFtoLabelPrinter(Pdf data, string prtName, string prtType)
        {
            var myPdf = JsonConvert.SerializeObject(data);
            var base64 = data.Base64;

            p = new Printer(base64.ToString(), "0", 1, prtName, prtType); //default copies=1
            return base64.ToString() + "\n" +
                    "printerName=["+ Printer.printerName + "]\n"+
                    "printerType=["+ Printer.printerType + "]\n";
        }

        public string ExecPrint(string drive, string path, string progName) {
            string result = "";
           
            if (Printer.pdfBase64 != "")
            {
                Byte[] bytes = Convert.FromBase64String(Printer.pdfBase64);
                string filename = Printer.fileid + ".pdf";
                string src_path = drive + ":\\" + path;
                Printer.srcPath = src_path;
                File.WriteAllBytes(src_path + "\\"+filename, bytes);

                PrinterSettings settings = new PrinterSettings();
                result += "\nPrinter Name:[" +settings.PrinterName + "]";

                string param = "";
               
                if (Printer.numOfCopies != 0)
                {
                    param = progName + " " +
                            src_path + " " +
                            Printer.numOfCopies + "x" + " " +
                            Printer.printerType + " " + 
                            Printer.printerName;
                }
                else
                {
                    param = progName + " " +
                            src_path + " 1x";
                }
                result += "\nSrc Path:[" + src_path + "]";
                result += "\nNo. of Copies:[" +Printer.numOfCopies + "]";
                result += "\nPrinter Name:[" + Printer.printerName + "]";
                result += "\nPrinter Type:[" + Printer.printerType + "]";
                result += "\nStatus:[Success]";

                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = @"/c " + src_path + "\\" + param;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardError = false;
                process.Start();
                process.WaitForExit();

                process.Close();

            }

            return result;          
        }

        public string GetEnvironment(string envName)
        {
            string envValue;

            envValue = Environment.GetEnvironmentVariable(envName);
            
            return "EnvName:"+ envName + " " + "EnvValue:" + envValue + ".getted";
        }

        public string SetEnvironment(string envName, string envValue)
        {
            string result="";
            // Determine whether the environment variable exists.
            if (Environment.GetEnvironmentVariable(envName) != null)
            { 
                // If it doesn't exist, create it.
                Environment.SetEnvironmentVariable(envName, envValue, EnvironmentVariableTarget.Machine);
                result += "EnvName:" + envName + " " + "EnvValue:" + envValue + ".setted";
            }
            return result; 
        }

        public string ScanDocument(string fileid) {

            device = null;
            string result = "";
            // Create a DeviceManager instance
            var deviceManager = new DeviceManager();
            

            // Loop through the list of devices and add the name to the listbox
            for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
            {
                // Add the device only if it's a scanner
                if (deviceManager.DeviceInfos[i].Type != WiaDeviceType.ScannerDeviceType)
                {
                    continue;
                }

                // Add the Scanner device to the listbox (the entire DeviceInfos object)
                // Important: we store an object of type scanner (which ToString method returns the name of the scanner)
                device = new Scanner(deviceManager.DeviceInfos[i]);
               
            }

            string filename = "";
            if(device != null)
            {
                ImageFile image = new ImageFile();
                string imgExtension = "";

                image = device.ScanJPEG();
                imgExtension = ".jpeg";

                filename = fileid + imgExtension;
                string src_path = "";
                src_path = Printer.srcPath;
                // Saving image name
                image.SaveFile(filename);

                //byte[] byteImg = (byte[])image.FileData.get_BinaryData();      

                //string Base64Img = Convert.ToBase64String(byteImg);

                PdfDocument doc = new PdfDocument();
                PdfPage page = doc.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XImage img = XImage.FromFile(filename);
                gfx.DrawImage(img, 0, 0);
                Scanner.fileID = fileid;
                doc.Save(fileid + ".pdf");
                doc.Close();
                
                //result += Base64Img;

            }
            
            return result += "Scanned document created ["+filename+"]";
        }
        
        public string RetrieveScannedDoc(string fileid)
        {
            string src_path = Printer.srcPath;
            byte[] file = File.ReadAllBytes(fileid+".pdf");
            string scannedDoc = Convert.ToBase64String(file);
            //device.setImgBase64(scannedDoc);

            string result = "";
            if (scannedDoc != "")
            {
                result = "{'Fileid': " + "'" + fileid + "',";
                result += "'Base64':'";

                result += scannedDoc + "'}";
            }
            return result;
        }

        public string ListPrinter()
        {
            string result ="{ 'printers': [";
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                if(printer != "")
                {
                    result += "'" + printer + "',";
                }
            }

            if (result.EndsWith(",") == true)
            {
                string tmp = "";
                tmp = result.Substring(0, result.Length - 1); 
                result = tmp + "]}";
            }
            return result;
        }

        public class WindowsService : ServiceBase
        {
            public ServiceHost serviceHost = null;
            public WindowsService()
            {
                // Name the Windows Service
                ServiceName = "HISHTPService";
            }

            public static void Main()
            {
                ServiceBase.Run(new WindowsService());
            }

            // Start the Windows service.
            protected override void OnStart(string[] args)
            {
                if (serviceHost != null)
                {
                    serviceHost.Close();
                }
                
                serviceHost = new CorsEnabledServiceHost(typeof(Service), new Uri("http://localhost:8000/"));
                try
                {
                    serviceHost.Open();
                    using (ChannelFactory<IService> cf = new ChannelFactory<IService>(new WebHttpBinding(), "http://localhost:8000/"))
                    {
                        cf.Endpoint.Behaviors.Add(new WebHttpBehavior());

                        IService channel = cf.CreateChannel();


                        string s;

                        Console.WriteLine("Calling Hello via HTTP GET: ");
                        s = channel.Hello();
                        Console.WriteLine("   Output: {0}", s);

                        Console.WriteLine("");
                        Console.WriteLine("This can also be accomplished by navigating to");
                        Console.WriteLine("http://localhost:8000/Hello");
                        Console.WriteLine("in a web browser while this service is running.");
                        Console.WriteLine("");
                    }
                    
                }
                catch (CommunicationException cex)
                {
                    Console.WriteLine("An exception occurred: {0}", cex.Message);
                    serviceHost.Abort();
                }
                
            }

            protected override void OnStop()
            {
                if (serviceHost != null)
                {
                    serviceHost.Close();
                    serviceHost = null;
                }
            }
        }
    }
    

    public class Person
    {
        public string name;
        public string origname;
        public string gpcname;
        public string icno;
        public string regno;
        public string dob; //date
        public string tob; //time
        public string pob; //place
        public string gender;
        public string citizenship;
        public string religion;
        public string race;
        public string addr1;
        public string addr2;
        public string addr3;
        public string poscode;
        public string city;
        public string state;
        public string photo;
    }

    public class Attributes
    {
        public string hostname;
    }

    public class Pdf
    {
        public string Id;
        public string Base64;
    }

    class Program
    {
        public Program(string filename, string base64)
        {
            Byte[] bytes = Convert.FromBase64String(base64);
            ByteArrayToFile(filename, bytes);
        }

        public bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }
    }

    // Provide the ProjectInstaller class which allows 
    // the service to be installed by the Installutil.exe tool
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.StartType = ServiceStartMode.Automatic;
            service.ServiceName = "HISHTPService";
            service.DisplayName = "HISHTP Service";
            service.Description = "Self-hosted Service for Web browser interact with card reader and printer";

            Installers.Add(process);
            Installers.Add(service);

            //service.AfterInstall += Service_AfterInstall;
        }

        /*private void Service_AfterInstall(object sender, InstallEventArgs e)
        {
            ServiceController sc = new ServiceController("MyKad Windows Service");
            try
            {
                //sc.Start(); old
                sc.Start()

            }
            finally
            {
                sc.Close();
            }
        }*/
    }
}
