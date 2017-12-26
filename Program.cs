using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using Newtonsoft.Json;

using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration;
using System.Configuration.Install;


/*  Author: hafizh
 *  Date: 13 Dec 2017
 * 
 *  Windows Service:
 *  i  - ver 1.0 : Handle Mykad
 *  ii - ver 2.0 : Handle Mykid
 *  
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
    }

    public class Service : IService
    {
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

        public class WindowsService : ServiceBase
        {
            public ServiceHost serviceHost = null;
            public WindowsService()
            {
                // Name the Windows Service
                ServiceName = "WindowsService";
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

    class Program
    {
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
            service.ServiceName = "MykadWinService";
            service.DisplayName = "MyKad Windows Service";
            service.Description = "Mykad service use to trigger smartcard reader";

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
