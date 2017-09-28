using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;

namespace TokenApp
{
    class Program
    {
        private static string token = string.Empty;

        static void Main(string[] args)
        {
            //Get an authentication access token
            token = GetToken().Result;
        }

        #region Get an authentication access token
        private static async Task<string> GetToken()
        {
            //The client id that Azure AD created when you registered your client app.
            string clientID = "a6973ab8-3845-4702-8f17-d3bd0d4b0071";

            //RedirectUri you used when you register your app.
            //For a client app, a redirect uri gives Azure AD more details on the application that it will authenticate.
            // You can use this redirect uri for your client app
            string redirectUri = "https://insights.local-dev.com";

            //Resource Uri for Power BI API
            string resourceUri = "https://analysis.windows.net/powerbi/api";

            //OAuth2 authority Uri
            string authorityUri = "https://login.windows.net/common/oauth2/authorize";

            string apiUrl = "https://api.powerbi.com/";

            string groupId = "0d35bcec-57d3-47f8-98db-9cf8ccd66bfa";

            //Get access token:
            // To call a Power BI REST operation, create an instance of AuthenticationContext and call AcquireToken
            // AuthenticationContext is part of the Active Directory Authentication Library NuGet package
            // To install the Active Directory Authentication Library NuGet package in Visual Studio,
            //  run "Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory" from the nuget Package Manager Console.

            // AcquireToken will acquire an Azure access token
            // Call AcquireToken to get an Azure token from Azure Active Directory token issuance endpoint
            AuthenticationContext authContext = new AuthenticationContext(authorityUri);
            string token = authContext.AcquireToken(resourceUri, clientID, new Uri(redirectUri)).AccessToken;

            var tokenCredentials = new TokenCredentials(token, "Bearer");
            using (var client = new PowerBIClient(new Uri(apiUrl), tokenCredentials))
            {
                // Get a list of reports.
                var reports = await client.Reports.GetReportsInGroupAsync(groupId);

                // Get the first report in the group.
                var report = reports.Value.FirstOrDefault();

                // Generate Embed Token.
                var generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                var tokenResponse = await client.Reports.GenerateTokenInGroupAsync(groupId, report.Id, generateTokenRequestParameters);


                // Generate Embed Configuration.
                Console.WriteLine("Embed Token:");
                Console.WriteLine(tokenResponse.Token);

                Console.WriteLine("EmbedUrl:");
                Console.WriteLine(report.EmbedUrl);

                Console.WriteLine("Report Id:");
                Console.WriteLine(report.Id);

                Console.ReadLine();
            }

            return token;
        }

        #endregion
    }
}
