using Common.Models;
using System.ServiceModel;

namespace Common
{
    [ServiceContract]
    public interface IAuthenticationService
    {
        [OperationContract]
        ClientModel Login(string email, string password);

        [OperationContract]
        ClientModel Logout(int? clientID, string token);

        [OperationContract]
        ClientModel Register(string username, string email, string password, string firstName, string lastName);

        [OperationContract]
        bool ValidateToken(string token);
    }
}
