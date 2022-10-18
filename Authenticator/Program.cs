using Authenticator.Interface;
using Authenticator.Service;
using DatabaseWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Authenticator
{
    internal class Program
    {
        private static int tokenExpiration = 30; // no. of minutes
        private static JobPostingDBEntities jobPostingDBEntities = new JobPostingDBEntities();

        static void Main(string[] args)
        {
            Console.WriteLine("\n\n");
            Console.WriteLine("\t\t# Authentication Server");
            Console.WriteLine("\n\n");

            var tcpBinding = new NetTcpBinding();
            var host = new ServiceHost(typeof(AuthenticationService));
            host.AddServiceEndpoint(typeof(IAuthenticationService), tcpBinding, "net.tcp://localhost:8085/AuthenticatiorService");
            host.Open();
            Console.WriteLine("\t\t# Server Started On");
            Console.WriteLine("\t\t- Port : 8085");
            Console.WriteLine("\t\t- Binding : Net Tcp");
            Console.WriteLine("\n\n");

            Timer timer = new Timer(new TimerCallback(TokenExpire), null, 1000, Timeout.Infinite);

            Console.WriteLine("\t\t# Press any key to exit.");
            Console.ReadLine();
            host.Close();
        }

        private static void TokenExpire(object state)
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
            catch (Exception ex)
            {
                
            }
        }
    }
}
