using Common;
using Common.Models;
using DatabaseWebAPI.Models;
using Common.Other;
using System;
using System.Linq;
using System.ServiceModel;

namespace Authenticator.Service
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class AuthenticationService : IAuthenticationService
    {
        private static Functions functions = new Functions();
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
                        existingClient.IsOnline = false;
                        if (existingClient.IsOnline != true)
                        {
                            string tokenCode = functions.EncodeString(existingClient.ClientID + "." + randomGenerator.Next(0, int.MaxValue));
                            jobPostingDBEntities.tblTokens.Add(new tblToken() { ClientID = existingClient.ClientID, TokenCode = tokenCode, CreatedDate = DateTime.Now });
                            jobPostingDBEntities.tblLoginHistories.Add(new tblLoginHistory { ClientID = existingClient.ClientID, Email = mEmail, ActionDate = DateTime.Now, StatusID = (int)Enums.StatusEnums.Passed });
                            existingClient.IsOnline = true;
                            jobPostingDBEntities.SaveChanges();
                            return new ClientModel() { ClientID = existingClient.ClientID, Email = existingClient.Email, FirstName = existingClient.FirstName, LastName = existingClient.LastName, Token = tokenCode, Response = new ResponseModel(true, "Client login successful.") };
                        }
                        else
                        {
                            return new ClientModel() { Response = new ResponseModel(false, "Client is already logged in.") };
                        }
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

        public ClientModel Logout(int? clientID, string token)
        {
            #region Validate_Logout_Data
            if (clientID == null || clientID == 0)
                return new ClientModel() { Response = new ResponseModel(false, "User identitfication failed.") };

            if (string.IsNullOrEmpty(token))
                return new ClientModel() { Response = new ResponseModel(false, "Authentication error.") };
            #endregion

            try
            {
                var existingClient = jobPostingDBEntities.tblClients.Where(c => c.ClientID == clientID).FirstOrDefault();
                if (existingClient != null)
                {
                    existingClient.IsOnline = false;
                    jobPostingDBEntities.SaveChanges();
                    return new ClientModel() { Email = existingClient.Email, Response = new ResponseModel(true, "Client logout successful.") };
                }
                else
                {
                    return new ClientModel() { Response = new ResponseModel(false, "Client does not exists.") };
                }
            }
            catch (Exception ex)
            {
                return new ClientModel() { Response = new ResponseModel(false, "An error occurred while logging out.") };
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
                        return new ClientModel() { Email = mEmail, Response = new ResponseModel(true, "Client registration successful.") };
                    }
                    else
                    {
                        return new ClientModel() { Response = new ResponseModel(false, "Failed to complete the client registration.") };
                    }
                }
                catch (Exception)
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
                return jobPostingDBEntities.tblTokens.Any(c => c.TokenCode.Equals(token));
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
