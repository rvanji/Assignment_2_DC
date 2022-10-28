using Authenticator.Service;
using Common;
using DatabaseWebAPI.Models;
using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Timers;

namespace Authenticator
{
    internal class Program
    {
        private static int tokenExpiration = 30; // no. of minutes
        private static System.Timers.Timer timer;
        private static JobPostingDBEntities jobPostingDBEntities = new JobPostingDBEntities();

        static void Main(string[] args)
        {
            Console.WriteLine("\n\n");
            Console.WriteLine("\t\t# Authentication Server");
            Console.WriteLine("\n\n");

            var tcpBinding = new NetTcpBinding();
            var host = new ServiceHost(typeof(AuthenticationService));
            host.AddServiceEndpoint(typeof(IAuthenticationService), tcpBinding, "net.tcp://localhost:10400/AuthenticatiorService");
            host.Open();
            Console.WriteLine("\t\t# Server Started On");
            Console.WriteLine("\t\t--> Port : 10400");
            Console.WriteLine("\t\t--> Binding : Net Tcp");
            Console.WriteLine("\n\n");

            if(timer == null)
            {
                timer = new System.Timers.Timer(10000);
                timer.Elapsed += new ElapsedEventHandler(TokenExpire);
                timer.Enabled = true;
            }

            Console.WriteLine("\t\t# Press any key to exit.");
            Console.ReadLine();
            host.Close();
        }

        private static void TokenExpire(object source, ElapsedEventArgs e)
        {
            try
            {
                DateTime dateTimeToClear = DateTime.Now.AddMinutes(-tokenExpiration);
                var expiredTokens = jobPostingDBEntities.tblTokens.Where(t => t.CreatedDate < dateTimeToClear).ToList();
                foreach(var token in expiredTokens)
                {
                    jobPostingDBEntities.tblTokens.Remove(token);
                }
            }
            catch (Exception)
            {                
            }
        }
    }
}
