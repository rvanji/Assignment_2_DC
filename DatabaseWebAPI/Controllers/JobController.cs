using Common;
using Common.Models;
using Common.Other;
using DatabaseWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DatabaseWebAPI.Controllers
{
    public class JobController : ApiController
    {
        private static Functions functions = new Functions();
        private static JobPostingDBEntities jobPostingDBEntities = new JobPostingDBEntities();
        private IAuthenticationService auth;

        [HttpPost]
        public IHttpActionResult CreateNewJob([FromBody] JobModel jobData)
        {
            #region -Validate_Job_Create_Data
            if (string.IsNullOrEmpty(jobData.Token))
                return Ok(new JobModel() { Response = new ResponseModel(false, "Authentication error.") });

            if (jobData.CreatedBy == null || jobData.CreatedBy == 0)
                return Ok(new JobModel() { Response = new ResponseModel(false, "User identitfication failed.") });
            #endregion

            return Ok(new JobModel() { Response = new ResponseModel(false, "TE") });
        }

        [HttpPost]
        public IHttpActionResult RetrieveClientJobs([FromBody] ClientModel client)
        {
            #region -Validate_Clients_Jobs_Retrieve
            if (string.IsNullOrEmpty(client.Token))
                return Ok(new ClientsModel() { Response = new ResponseModel(false, "Authentication error.") });

            if (client.ClientID == null || client.ClientID == 0)
                return Ok(new ClientsModel() { Response = new ResponseModel(false, "User identitfication failed.") });
            #endregion

            //bool isValidToken = auth.ValidateToken(client.Token);
            //if (isValidToken)
            //{
            try
            {
                JobsModel jobsModel = new JobsModel();
                jobsModel.JobsList = jobPostingDBEntities.tblJobs.Where(j => j.tblClient.ClientID == client.ClientID && j.RemoteServiceID == null && j.StatusID == (int)Enums.StatusEnums.Active).Select(s => new JobModel()
                {
                    JobID = s.JobID,
                    RemoteServiceID = s.RemoteServiceID,
                    PythonCode = s.PythonCode,
                    Result = s.Result,
                }).ToList();
                jobsModel.Response = new ResponseModel() { IsSuccess = true, Message = "" };
                return Ok(jobsModel);
            }
            catch (Exception)
            {
                return Ok(new JobsModel() { Response = new ResponseModel(false, "An error occurred while fetching jobs.") });
            }
            //}
            //else
            //{
            return Ok(new ClientsModel() { Response = new ResponseModel(false, "Authentication denied.") });
            //}
        }
    }
}