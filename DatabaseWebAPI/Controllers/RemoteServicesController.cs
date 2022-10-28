using Common;
using Common.Models;
using Common.Other;
using DatabaseWebAPI.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace DatabaseWebAPI.Controllers
{
    public class RemoteServicesController : ApiController
    {
        private static Functions functions = new Functions();
        private static JobPostingDBEntities jobPostingDBEntities = new JobPostingDBEntities();
        private IAuthenticationService auth;

        public RemoteServicesController()
        {
            auth = functions.InitAuthentication();
        }

        [HttpPost]
        public IHttpActionResult PublishRemoteService([FromBody] RemoteServiceModel remoteService)
        {
            #region -Validate_Remote_Service_Publish_Data
            if (string.IsNullOrEmpty(remoteService.Token))
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "Authentication error.") });

            if (remoteService.Client == null || remoteService.Client.ClientID == null || remoteService.Client.ClientID == 0)
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "User identitfication failed.") });

            string mIPAddress = functions.DecodeString(remoteService.IPAddress);
            if (string.IsNullOrEmpty(mIPAddress) || string.Equals(mIPAddress = mIPAddress.Trim(), ""))
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "IP Address is required.") });
            else if (!functions.IsValidIPAddress(mIPAddress))
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "IP Address is invalid.") });

            if (remoteService.Port == null)
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "Port is required.") });
            else if (remoteService.Port < 0)
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "Port is invalid.") });
            #endregion

            bool isValidToken = auth.ValidateToken(remoteService.Token);
            if (isValidToken)
            {
                try
                {
                    var existingService = jobPostingDBEntities.tblRemoteServices.Where(r => r.IPAddress.Equals(mIPAddress) && r.Port == remoteService.Port && r.StatusID == (int)Enums.StatusEnums.Active).FirstOrDefault();
                    if (existingService == null)
                    {
                        jobPostingDBEntities.tblRemoteServices.Add(new tblRemoteService()
                        {
                            IPAddress = mIPAddress,
                            Port = remoteService.Port,
                            IsAllocated = false,
                            CreatedBy = remoteService.Client.ClientID,
                            CreatedDate = DateTime.Now,
                            StatusID = (int)Enums.StatusEnums.Active,
                        });;
                        jobPostingDBEntities.SaveChanges();
                        return Ok(new RemoteServiceModel() { Response = new ResponseModel(true, "IP Address & Port hosted.") });
                    }
                    else
                    {
                        return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "IP Address & Port already exists.") });
                    }
                }
                catch (Exception)
                {
                    return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "An error occurred while saving to db.") });
                }
            }
            else
            {
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "Authentication denied.") });
            }
        }

        [HttpPost]
        public IHttpActionResult UnpublishRemoteService([FromBody] RemoteServiceModel remoteService)
        {
            #region -Validate_Remote_Service_Unpublish_Data
            if (string.IsNullOrEmpty(remoteService.Token))
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "Authentication error.") });

            if (remoteService.Client == null || remoteService.Client.ClientID == null || remoteService.Client.ClientID == 0)
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "User identitfication failed.") });

            string mIPAddress = functions.DecodeString(remoteService.IPAddress);
            if (string.IsNullOrEmpty(mIPAddress) || string.Equals(mIPAddress = mIPAddress.Trim(), ""))
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "IP Address is required.") });
            else if (!functions.IsValidIPAddress(mIPAddress))
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "IP Address is invalid.") });

            if (remoteService.Port == null)
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "Port is required.") });
            else if (remoteService.Port < 0)
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "Port is invalid.") });
            #endregion

            bool isValidToken = auth.ValidateToken(remoteService.Token);
            if (isValidToken)
            {
                try
                {
                    var existingService = jobPostingDBEntities.tblRemoteServices.Where(r => r.IPAddress.Equals(mIPAddress) && r.Port == remoteService.Port && r.StatusID == (int)Enums.StatusEnums.Active).FirstOrDefault();
                    if (existingService != null)
                    {
                        if (existingService.IsAllocated != true)
                        {
                            existingService.StatusID = (int)Enums.StatusEnums.Deleted;
                            existingService.UpdatedBy = remoteService.Client.ClientID;
                            existingService.UpdatedDate = DateTime.Now;
                            jobPostingDBEntities.SaveChanges();

                            return Ok(new RemoteServiceModel() { Response = new ResponseModel(true, "IP Address & Port removed.") });
                        }
                        else
                        {
                            return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "Remote Service is being consumed.") });
                        }
                    }
                    else
                    {
                        return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "Remote Service does not exist.") });
                    }
                }
                catch (Exception)
                {
                    return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "An error occurred while saving to db.") });
                }
            }
            else
            {
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "Authentication denied.") });
            }
        }

        [HttpPost]
        public IHttpActionResult ListRemoteServices([FromBody] ClientModel client)
        {
            #region -Validate_Remote_Services_Request
            if (string.IsNullOrEmpty(client.Token))
                return Ok(new RemoteServiceModel() { Response = new ResponseModel(false, "Authentication error.") });
            #endregion

            bool isValidToken = auth.ValidateToken(client.Token);
            if (isValidToken)
            {
                try
                {
                    RemoteServicesModel remoteServicesModel = new RemoteServicesModel();
                    remoteServicesModel.RemoteServices = jobPostingDBEntities.tblRemoteServices.Where(r => (client.ClientID != null ? r.CreatedBy == client.ClientID : true) && r.tblClient.IsOnline == true && r.StatusID == (int)Enums.StatusEnums.Active).Select(s => new RemoteServiceModel()
                    {
                        Client = new ClientModel() { ClientID = s.tblClient.ClientID, FirstName = s.tblClient.FirstName, LastName = s.tblClient.LastName },
                        IPAddress = s.IPAddress,
                        Port = s.Port,
                        NoOfJobs = s.tblJobs.Count(),
                        IsAllocated = s.IsAllocated,
                    }).ToList();
                    remoteServicesModel.Response = new ResponseModel() { IsSuccess = true, Message = "" };
                    return Ok(remoteServicesModel);
                }
                catch (Exception)
                {
                    return Ok(new RemoteServicesModel() { Response = new ResponseModel(false, "An error occurred while fetching remote services.") });
                }
            }
            else
            {
                return Ok(new RemoteServicesModel() { Response = new ResponseModel(false, "Authentication denied.") });
            }
        }
    }
}