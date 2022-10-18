using DatabaseWebAPI.Models;
using DatabaseWebAPI.Other;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Authenticator.Service
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class AuthenticationService : Interface.IAuthenticationService
    {
        private static Functions1 functions = new Functions1();
        private static Random randomGenerator = new Random();
        private static JobPostingDBEntities jobPostingDBEntities = new JobPostingDBEntities();

        public ClientModel Login(string email, string password)
        {
            #region Validate_Login_Data
            string mEmail = functions.DecodeString(email);
            if (string.IsNullOrEmpty(mEmail) || string.Equals(mEmail = mEmail.Trim(), ""))
                return new ClientModel() { Response = new ResponseModel(false, "Email Address is required.") };
            else if (functions.IsValidEmail(mEmail) == false)
                return new ClientModel() { Response = new ResponseModel(false, "Email Address is invalid.") };

            string mPassword = functions.DecodeString(password);
            if (string.IsNullOrEmpty(mPassword) || string.Equals(mPassword = mPassword.Trim(), ""))
                return new ClientModel() { Response = new ResponseModel(false, "Password is required.") };
            else if (mPassword.Length < 10)
                return new ClientModel() { Response = new ResponseModel(false, "Atleast 10 characters required for password.") };
            #endregion

            try
            {
                var existingClient = jobPostingDBEntities.tblClients.Where(c => c.Email.Equals(mEmail) && c.Password.Equals(password)).FirstOrDefault();
                if (existingClient != null)
                {
                    if (existingClient.StatusID == (int)Enums.StatusEnums.Active)
                    {
                        string tokenCode = functions.EncodeString(existingClient.ClientID + "." + randomGenerator.Next(0, int.MaxValue));
                        jobPostingDBEntities.tblTokens.Add(new tblToken() { ClientID = existingClient.ClientID, TokenCode = tokenCode, CreatedDate = DateTime.Now });
                        jobPostingDBEntities.tblLoginHistories.Add(new tblLoginHistory { ClientID = existingClient.ClientID, Email = mEmail, ActionDate = DateTime.Now, StatusID = (int)Enums.StatusEnums.Passed });
                        jobPostingDBEntities.SaveChanges();
                        return new ClientModel() { Response = new ResponseModel(true, "Client login successful.") };
                    }
                    else
                    {
                        jobPostingDBEntities.tblLoginHistories.Add(new tblLoginHistory { ClientID = existingClient.ClientID, Email = mEmail, ActionDate = DateTime.Now, StatusID = (int)Enums.StatusEnums.Failed });
                        jobPostingDBEntities.SaveChanges();
                        return new ClientModel() { Response = new ResponseModel(false, "Client is not active at the moment.") };
                    }
                }
                else
                {
                    jobPostingDBEntities.tblLoginHistories.Add(new tblLoginHistory { Email = mEmail, ActionDate = DateTime.Now, StatusID = (int)Enums.StatusEnums.Failed });
                    jobPostingDBEntities.SaveChanges();
                    return new ClientModel() { Response = new ResponseModel(false, "Invalid email address or password.") };
                }
            }
            catch(Exception ex)
            {
                return new ClientModel() { Response = new ResponseModel(false, "An error occurred while logging in.") };
            }
        }

        public ClientModel Register(string username, string email, string password, string firstName, string lastName)
        {
            #region Validate_Client_Data
            string mUsername = functions.DecodeString(username);
            if (string.IsNullOrEmpty(mUsername) || string.Equals(mUsername = mUsername.Trim(), ""))
                return new ClientModel() { Response = new ResponseModel(false, "Username is required.") };
            else if (mUsername.Length < 5)
                return new ClientModel() { Response = new ResponseModel(false, "Atleast 5 characters required for username.") };

            string mEmail = functions.DecodeString(email);
            if (string.IsNullOrEmpty(mEmail) || string.Equals(mEmail = mEmail.Trim(), ""))
                return new ClientModel() { Response = new ResponseModel(false, "Email Address is required.") };
            else if (functions.IsValidEmail(mEmail) == false)
                return new ClientModel() { Response = new ResponseModel(false, "Email Address is invalid.") };

            string mPassword = functions.DecodeString(password);
            if (string.IsNullOrEmpty(mPassword) || string.Equals(mPassword = mPassword.Trim(), ""))
                return new ClientModel() { Response = new ResponseModel(false, "Password is required.") };
            else if (mPassword.Length < 10)
                return new ClientModel() { Response = new ResponseModel(false, "Atleast 10 characters required for password.") };

            string mFirstName = functions.DecodeString(firstName);
            if (string.IsNullOrEmpty(mFirstName) || string.Equals(mFirstName = mFirstName.Trim(), ""))
                return new ClientModel() { Response = new ResponseModel(false, "First Name is required.") };
            else if (mFirstName.Length <= 1 || functions.IsLettersSpacesOnly(mFirstName) == false)
                return new ClientModel() { Response = new ResponseModel(false, "First Name is invalid.") };

            string mLastName = functions.DecodeString(lastName);
            if (string.IsNullOrEmpty(mLastName) || string.Equals(mLastName = mLastName.Trim(), ""))
                return new ClientModel() { Response = new ResponseModel(false, "Last Name is required.") };
            else if (mLastName.Length <= 1 || functions.IsLettersSpacesOnly(mLastName) == false)
                return new ClientModel() { Response = new ResponseModel(false, "Last Name is invalid.") };
            #endregion

            var existingClient = jobPostingDBEntities.tblClients.Where(c => (c.Username.Equals(mUsername) || c.Email.Equals(mEmail)) && c.StatusID == (int)Enums.StatusEnums.Active).FirstOrDefault();
            if (existingClient == null)
            {
                try
                {
                    jobPostingDBEntities.tblClients.Add(new tblClient()
                    {
                        Username = mUsername,
                        Email = mEmail,
                        Password = functions.EncodeString(mPassword),
                        FirstName = mFirstName,
                        LastName = mLastName,
                        CreatedDate = DateTime.Now,
                        StatusID = (int)Enums.StatusEnums.Active,
                    });
                    var result = jobPostingDBEntities.SaveChanges();

                    if (result > 0)
                    {
                        return new ClientModel() { Response = new ResponseModel(true, "Client registration successful.") };
                    }
                    else
                    {
                        return new ClientModel() { Response = new ResponseModel(false, "Failed to complete the client registration.") };
                    }
                }
                catch (Exception ex)
                {
                    return new ClientModel() { Response = new ResponseModel(false, "Database error occurred while saving.") };
                }
            }
            else
            {
                return new ClientModel() { Response = new ResponseModel(false, "Client with the same Username or Email Address already exists.") };
            }
        }
    
        public bool ValidateToken(string token)
        {
            try
            {
                return jobPostingDBEntities.tblTokens.Any(c => c.TokenID.Equals(token));
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
