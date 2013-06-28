using RestSharp;
using RestSharp.Contrib;

namespace AppHarbor
{
    public static class AccessTokenHelper
    {
        /// <summary>
        /// Get access token.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string GetAccessToken(string username, string password)
        {
            //NOTE: Remove when merged into AppHarbor.NET library
            var restClient = new RestClient("https://appharbor-token-client.apphb.com");
            var request = new RestRequest("/token", Method.POST);

            request.AddParameter("username", username);
            request.AddParameter("password", password);

            var response = restClient.Execute(request);
            var accessToken = HttpUtility.ParseQueryString(response.Content)["access_token"];

            if (accessToken == null)
            {
                throw new CommandException("Couldn't log in. Try again");
            }

            return accessToken;
        }
    }
}
