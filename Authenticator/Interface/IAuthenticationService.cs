using DatabaseWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Authenticator.Interface
{
    [ServiceContract]
    public interface IAuthenticationService
    {
        [OperationContract]
        ClientModel Login(string email, string password);

        [OperationContract]
        ClientModel Register(string username, string email, string password, string firstName, string lastName);

        [OperationContract]
        bool ValidateToken(string token);
    }
}
