using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RazSrtLanguageTranslator
{
    /// <summary>
    /// For Azure Cognitive Services - v3 Translator Text API
    /// </summary>
    class TranslatorHelper
    {
        private readonly string apiKey;
        private readonly string host;
        private const string route_languages = "/languages?api-version=3.0";
        private const string route_translate = "/translate?api-version=3.0";

        private string authToken;
        
        public TranslatorHelper(string host, string apiKey)
        {
            this.host = host;
            this.apiKey = apiKey;

            // For the Authorization Token method as described in https://docs.microsoft.com/en-us/azure/cognitive-services/translator/reference/v3-0-reference#authentication
            // Note that this generated token expires in 10 minutes.
            //InitializeToken(apiKey);
        }

        public void InitializeToken(string apiKey)
        {
            var authTokenSource = new AzureAuthToken(apiKey);
            try
            {
                authToken = authTokenSource.GetAccessToken();
            }
            catch (HttpRequestException)
            {
                if (authTokenSource.RequestStatusCode == HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Request to token service is not authorized (401). Check that the Azure subscription key is valid.");
                    return;
                }
                if (authTokenSource.RequestStatusCode == HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("Request to token service is not authorized (403). For accounts in the free-tier, check that the account quota is not exceeded.");
                    return;
                }
                throw;
            }
        }

        public string GetSupportedLanguages()
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;

                // Construct the full URI
                request.RequestUri = new Uri(host + route_languages);

                // Add the authorization header
                request.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);

                // Send request, get response
                var response = client.SendAsync(request).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        public string Translate(string text, string from, params string[] to)
        {
            var body = new[] { new { Text = text } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Set the method to POST
                request.Method = HttpMethod.Post;

                // Construct the full URI
                var route = route_translate;
                route += "&from=" + from;
                foreach (var l in to)
                    route += string.Concat("&to=", l);
                request.RequestUri = new Uri(host + route);

                // Add the serialized JSON object to your request
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                // Add the authorization header
                request.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);
                //request.Headers.Add("Authorization", authToken);  // in case the authorization token method is preferred.

                // Send request, get response
                var response = client.SendAsync(request).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
