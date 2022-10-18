using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DatabaseWebAPI.Other
{
    public class Functions1
    {
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

        public bool IsLettersSpacesOnly(string text)
        {
            Regex regexLettersSpaces = new Regex(@"/^[a-zA-Z\s]*$/");
            Match match = regexLettersSpaces.Match(text);

            if (match.Success)
                return true;
            else
                return false;
        }
        #endregion
    }
}