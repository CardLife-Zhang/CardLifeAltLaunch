using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace CardLifeAltLaunch
{
    /// <summary>
    /// Authenticates Usernames and Passwords
    /// </summary>
    public class CAuthenticator
    {

        public CAuthenticator(Action<string, string, EMessageType> aMessageHandler)
        {
            MessageHandler = aMessageHandler;
        }

        public Action<string, string, EMessageType> MessageHandler {
            get;
        }

        public bool Authenticate(string anEmail, SecureString aPassword, out CAuthentificationData anAuthData)
        {
            var aClient = new RestClient("https://live-auth.cardlifegame.com/");

            IRestResponse<CAuthentificationData> aResponse = null;
            aPassword.UseSecureString((pass) =>
            {
                // Try to create a request.  Abort if not possible
                var aRequest = CreateRequest(anEmail, pass);
                if (aRequest == null)
                {
                    return;
                }

                aResponse = aClient.Execute<CAuthentificationData>(aRequest);
            });

            if(aResponse == null)
            {
                anAuthData = null;
                return false;
            }

            // Could we talk to the server?
            if (CheckResponse(aResponse))
            {
                anAuthData = ParseReturnedData(aResponse);

                if (anAuthData != null)
                {
                    Properties.Settings.Default.emailAddress = anAuthData.EmailAddress;
                    Properties.Settings.Default.Save();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                anAuthData = null;
                return false;
            }
        }

        private void SendMessage(string aTitleResource, string aTextResource, EMessageType aMessageType)
        {
            MessageHandler(
                Properties.Resources.ResourceManager.GetString(aTitleResource),
                Properties.Resources.ResourceManager.GetString(aTextResource),
                aMessageType
            );
        }

        private void SendMessage(string aTitleResource, string aTextResource, string[] aTextResourceParams, EMessageType aMessageType)
        {
            MessageHandler(
                Properties.Resources.ResourceManager.GetString(aTitleResource),
                string.Format(Properties.Resources.ResourceManager.GetString(aTextResource), aTextResourceParams),
                aMessageType
            );
        }

        /// <summary>
        /// Creates a Request for talking to the server
        /// </summary>
        /// <param name="anEmail"></param>
        /// <param name="aPassword"></param>
        /// <returns></returns>
        private RestRequest CreateRequest(string anEmail, string aPassword)
        {
            // Check to see if they entered creds and abort if not
            if ((anEmail.Length == 0) && (aPassword.Length == 0))
            {
                return null;
            }

            var aRequest = new RestRequest("api/auth/authenticate/", Method.POST);
            aRequest.RequestFormat = DataFormat.Json;

            var body = new
            {
                emailAddress = anEmail,
                Password = aPassword
            };
            aRequest.AddJsonBody(body);

            return aRequest;
        }

        /// <summary>
        /// Checks the response from the server
        /// </summary>
        /// <param name="aResponse"></param>
        /// <returns></returns>
        private bool CheckResponse(IRestResponse<CAuthentificationData> aResponse)
        {
            if (!aResponse.IsSuccessful)
            { 
                if (aResponse.Content.Equals("User not found"))  // Username enumeration... :(
                {
                    // Did the server accept our username
                    SendMessage("AUTH_COULD_NOT_LOGIN", "AUTH_EMAIL_FAILED", new string[] { "\n" }, EMessageType.Warning);
                }
                else if (aResponse.Content.Equals("Failed to authenticate"))
                {
                    // Did the server accept our password
                    SendMessage("AUTH_COULD_NOT_LOGIN", "AUTH_PASSWORD_FAILED", new string[] { "\n" }, EMessageType.Warning);
                } 
                else if (aResponse.StatusCode == 0)
                {
                    // Couldn't contact server
                    SendMessage("AUTH_COULD_NOT_LOGIN", "AUTH_SERVER_TALK_FAILED", new string[] { "\n", aResponse.ErrorMessage }, EMessageType.Warning);
                    return false;
                }
                else
                {
                    // All other failures!
                    SendMessage("AUTH_COULD_NOT_LOGIN", "AUTH_GENERAL_FAILURE", new string[] { "\n", aResponse.ErrorMessage }, EMessageType.Warning);
                }

                return false;
            } else
            {
                return true;
            }
        }

        /// <summary>
        /// Parses the returned data from the server
        /// </summary>
        /// <param name="aResponse"></param>
        /// <returns></returns>
        private CAuthentificationData ParseReturnedData(IRestResponse<CAuthentificationData> aResponse)
        {
            try
            {
                CAuthentificationData anAuthdata = aResponse.Data;
                return anAuthdata;

            }
            catch (Exception e)
            {
                SendMessage("AUTH_COULD_NOT_LOGIN", "AUTH_SERVER_RETURNED_BAD_DATA", EMessageType.Warning);
                Console.WriteLine("Exception " + e.Message);

                return null;
            }
        }
    }
}
