using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common.Other
{
    public class Functions
    {
        #region Authentication
        public IAuthenticationService InitAuthentication()
        {
            ChannelFactory<IAuthenticationService> AuthenticationFactory;
            NetTcpBinding tcp = new NetTcpBinding();
            AuthenticationFactory = new ChannelFactory<IAuthenticationService>(tcp, "net.tcp://localhost:10400/AuthenticatiorService");
            return AuthenticationFactory.CreateChannel();
        }
        #endregion

        #region Encoding/Decoding
        public string EncodeString(string text)
        {
            if (String.IsNullOrEmpty(text) || String.Equals(text.Trim(), ""))
                return text;

            byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(strBytes);
        }

        public string DecodeString(string base64Text)
        {
            if (String.IsNullOrEmpty(base64Text) || String.Equals(base64Text.Trim(), ""))
                return base64Text;

            byte[] encodedBytes = Convert.FromBase64String(base64Text);
            return System.Text.Encoding.UTF8.GetString(encodedBytes);
        }
        #endregion

        #region Validations
        public bool IsValidEmail(string email)
        {
            Regex regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regexEmail.Match(email);

            if (match.Success)
                return true;
            else
                return false;
        }
        public bool IsValidIPAddress(string ipAddress)
        {
            try
            {
                if (ipAddress.Length >= 8)
                {
                    IPAddress.Parse(ipAddress);
                    return true;
                }  
                else
                {
                    return false;
                }
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public bool IsLettersSpacesOnly(string text)
        {
            Regex regexLettersSpaces = new Regex(@"^[A-Za-z ]+$");
            Match match = regexLettersSpaces.Match(text);

            if (match.Success)
                return true;
            else
                return false;
        }
        #endregion
    }
}
