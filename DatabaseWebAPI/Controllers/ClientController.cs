using Common;
using Common.Models;
using DatabaseWebAPI.Models;
using Common.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.Http;

namespace DatabaseWebAPI.Controllers
{
    public class ClientController : ApiController
    {
        private static Functions functions = new Functions();
        private static JobPostingDBEntities jobPostingDBEntities = new JobPostingDBEntities();
        private IAuthenticationService auth;

        public ClientController()
        {
            auth = functions.InitAuthentication();
        }

        [HttpGet]
        public IHttpActionResult Login([FromBody] ClientModel client)
        {
            var loginResult = auth.Login(client.Email, client.Password);
            return Ok(loginResult);
        }

        [HttpPost]
        public IHttpActionResult Register([FromBody] ClientModel client)
        {
            var registerResult = auth.Register(client.Username, client.Email, client.Password, client.FirstName, client.LastName);
            return Ok(registerResult);
        }

        [HttpPost]
        public IHttpActionResult RetrieveOnlineClients([FromBody] ClientModel client)
        {
            #region -Validate_Clients_Retrieve_Request
            if (string.IsNullOrEmpty(client.Token))
                return Ok(new ClientsModel() { Response = new ResponseModel(false, "Authentication error.") });

            if (client.ClientID == null || client.ClientID == 0)
                return Ok(new ClientsModel() { Response = new ResponseModel(false, "User identitfication failed.") });
            #endregion

            bool isValidToken = auth.ValidateToken(client.Token);
            if (isValidToken)
            {
                try
                {
                    ClientsModel clientsModel = new ClientsModel();
                    clientsModel.ClientsList = jobPostingDBEntities.tblClients.Where(c => c.IsOnline == true && c.StatusID == (int)Enums.StatusEnums.Active).Select(s => new ClientModel()
                    {
                        ClientID = s.ClientID,
                        Username = s.Username,
                        Email = s.Email,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                    }).ToList();
                    clientsModel.Response = new ResponseModel() { IsSuccess = true, Message = "" };
                    return Ok(clientsModel);
                }
                catch (Exception)
                {
                    return Ok(new ClientsModel() { Response = new ResponseModel(false, "An error occurred while fetching clients.") });
                }
            }
            else
            {
                return Ok(new ClientsModel() { Response = new ResponseModel(false, "Authentication denied.") });
            }
        }
    }
}