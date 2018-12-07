﻿using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using Newtonsoft.Json;

using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration.Install;
using System.IO;
using System.Diagnostics;
using System.Drawing.Printing;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using WIA;
using PdfSharp.Pdf.IO;
using System.Management;


/*  Author: hafizh
 *  Date: 13 Dec 2017
 * 
 *  Windows Service:
 *  i   - ver 1.0 : (13-Dec-18) Handle Mykad
 *  ii  - ver 2.0 : (01-Jan-18) Handle Mykid
 *  iii - ver 2.1 : (13-Mar-18) Add hostname 
 *  iv  - ver 2.2&2.3 : (29-Mar-18) Add printer
 *  v   - ver 2.4 : (07-Nov-18) Add Printer name and set environment
 *  vi  - ver 2.4.5 : Working as UAT on 30-Nov-18
 *  vii - ver 2.4.7 : Add new API for pdf combine  
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
            UriTemplate = "/PostPdfCopies/{id}?numOfCopies={copies}&printerName={prtName}"), CorsEnabled]
        string PostPdfCopies(Pdf data, string id, int copies, string prtName);

        [OperationContract]
        [WebInvoke(Method = "POST",
            BodyStyle = WebMessageBodyStyle.WrappedResponse,
            //RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/PDFtoLabelPrinter/{id}?numOfCopies={copies}&printerName={prtName}&printerType={prtType}"), CorsEnabled]
        string PDFtoLabelPrinter(Pdf data, string id, int copies,string prtName, string prtType);

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
            UriTemplate = "/ScanDocument?fileid={fileid}&path={path}"), CorsEnabled]
        string ScanDocument(String fileid, string path);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            UriTemplate = "/RetrieveScannedDoc?fileid={fileid}&path={path}"), CorsEnabled]
        string RetrieveScannedDoc(string fileid, string path);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            UriTemplate = "/ListPrinter"), CorsEnabled]
        string ListPrinter();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            UriTemplate = "/MergePDFs/{id}?targetPath={targetPath}"), CorsEnabled]
        string MergePDFs(string id, string targetPath);


        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            UriTemplate = "/DeletePDF/{id}?targetPath={targetPath}"), CorsEnabled]
        string DeletePDF(string id, string targetPath);

    }

    
    
    public class Service : IService
    {
        Printer myPrinter;
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
            myPrinter = null; 
            myPrinter = new Printer(base64.ToString(), id);
            return base64.ToString();
        }

        
        public string PostPdfCopies(Pdf data, string id, int copies, string prtName)
        {
            var myPdf = JsonConvert.SerializeObject(data);
            var base64 = data.Base64;
            myPrinter = null;
            myPrinter = new Printer(base64.ToString(), id, copies, prtName, "NORMAL");
            return base64.ToString(); ;
        }

        public string PDFtoLabelPrinter(Pdf data, string id, int copies,string prtName, string prtType)
        {
            var myPdf = JsonConvert.SerializeObject(data);
            var base64 = data.Base64;
            myPrinter = null;
            myPrinter = new Printer(base64.ToString(), id, copies, prtName, prtType); //default copies=1

            return base64.ToString();
        }

        public string ExecPrint(string drive, string path, string progName) {
            string result = "";
            
            if (Printer.pdfBase64 != "")
            {
                Byte[] bytes = Convert.FromBase64String(Printer.pdfBase64);
                string filename = Printer.fileid + ".pdf";
                string srcPath = drive + ":\\" + path;
                string binPath = drive + ":\\" + path + "\\bin";
                File.WriteAllBytes(srcPath + "\\tmp\\"+filename, bytes);

                //PrinterSettings settings = new PrinterSettings();
                //result += "\nDefault Printer Name:[" +settings.PrinterName + "]";

                string param = "";
               
                if (Printer.numOfCopies != 0)
                {
                    param = progName + " " +
                            srcPath + " " +
                            Printer.fileid + " " +
                            Printer.numOfCopies + "x" + " " +
                            Printer.printerType + " " +
                            Printer.printerName;
                }
                else
                {
                    param = progName + " " +
                            srcPath + " 1x";
                }
                result += String.Format("\nSrc Path:[{0}]", srcPath);
                result += String.Format("\nFile ID:[{0}]",Printer.fileid);
                result += String.Format("\nNo. of Copies:[{0}]",Printer.numOfCopies);
                result += String.Format("\nPrinter Name Select:[{0}]", Printer.printerName);
                result += String.Format("\nPrinter Type:[{0}]",Printer.printerType);
                result += "\nStatus:[Success]";
             
                try
                {
                    Process process = new Process();
                    process.StartInfo.WorkingDirectory = @"C:\WINDOWS\system32";
                    process.StartInfo.FileName = @"C:\WINDOWS\system32\cmd.exe";
                    process.StartInfo.Arguments = @"/c " + binPath + "\\" + param;
                    
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = false;
                    process.StartInfo.RedirectStandardError = false;
                    process.Start();

                    process.WaitForExit();
                    
                    process.Close();
                }
                catch (Exception e)
                {
                    return e.Message;
                }
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

        public string ScanDocument(string fileid, string path) {

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
            
            string tmp_path = "";
            if (device != null)
            {
                ImageFile image = new ImageFile();
                string imgExtension = "";

                image = device.ScanJPEG();
                imgExtension = ".jpeg";
                
                tmp_path = path + "\\tmp\\" + fileid + imgExtension;

                // Saving image name
                image.SaveFile(tmp_path);
                
                PdfDocument doc = new PdfDocument();
                PdfPage page = doc.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XImage img = XImage.FromFile(tmp_path);
                gfx.DrawImage(img, 0, 0);
                tmp_path = "";
                tmp_path = path + "\\tmp\\" + fileid + ".pdf";
                doc.Save(tmp_path);
                doc.Close();
                
                //result += Base64Img;

            }
            
            return result += "Scanned document created ["+tmp_path+"]";
        }
        
       
        public string RetrieveScannedDoc(string fileid, string path)
        {
            string tmp = path + "\\tmp\\" + fileid + ".pdf";
            string output = "";

            if (File.Exists(tmp))
            {
                byte[] file = File.ReadAllBytes(tmp);
                string scannedDoc = Convert.ToBase64String(file);


                Product product = new Product();

                product.fileID = fileid;
                product.scannedDoc = scannedDoc;

                //output += "\npro.fileid=[" + product.fileID + "]";
                //output += "\npro.ScannedDoc=[" + product.scannedDoc + "]";
                
                output = JsonConvert.SerializeObject(product);
            }
            return output;
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
        
        public string MergePDFs(string id, string targetPath)
        {
            string param = "";
            param = String.Format("mergePdfs.bat {0} {1}", targetPath, id);

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = @"/c " + targetPath + "\\bin\\" + param;

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.RedirectStandardError = false;
                process.Start();

                process.WaitForExit();

                process.Close();
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return "Combine PDFs Completed.";
        }

        public string DeletePDF(string id, string targetPath)
        {
            string source = "NULL";
            try
            {
                source = String.Format("{0}\\tmp\\{1}.pdf", targetPath, id);
                if (File.Exists(source) == true)
                {
                    string param = "";
                    param = String.Format(@"DEL /F /Q /A {0}", source);


                    Process process = new Process();
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = @"/c " + param;

                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = false;
                    process.StartInfo.RedirectStandardError = false;
                    process.Start();

                    process.WaitForExit();

                    process.Close();
                }
            }
            catch (IOException ioex)
            {
                Console.WriteLine(ioex.Message);
            }

            return String.Format("PDF [{0}] deleted.", source);
        }

        public static void ClearTemp(string targetPath)
        {
            try
            {
                if (Directory.Exists(targetPath) == true)
                {
                    string param = "";
                    param = String.Format(@"DEL /F /Q /A {0}\\*", targetPath);


                    Process process = new Process();
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = @"/c " + param;

                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = false;
                    process.StartInfo.RedirectStandardError = false;
                    process.Start();

                    process.WaitForExit();

                    process.Close();
                }
            }
            catch (IOException ioex)
            {
                Console.WriteLine(ioex.Message);
            }
           
            
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
                        //Test
                        s = channel.Hello();
                        
                        //Clear anything inside tmp folder
                        ClearTemp("C:\\HISHTPService\\tmp");
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
