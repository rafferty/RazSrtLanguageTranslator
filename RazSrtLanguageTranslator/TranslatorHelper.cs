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
        private readonly string host;
        private const string route_languages = "/languages?api-version=3.0";
        private const string route_translate = "/translate?api-version=3.0";

        private string authToken;
        
        public TranslatorHelper(string host, string key)
        {
            this.host = host;
            InitializeToken(key);
        }

        public void InitializeToken(string key)
        {
            var authTokenSource = new AzureAuthToken(key);
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
                request.Headers.Add("Ocp-Apim-Subscription-Key", Properties.Settings.Default.ApiKey);

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
                //request.Headers.Add("Ocp-Apim-Subscription-Key", Properties.Settings.Default.ApiKey);
                request.Headers.Add("Authorization", authToken);

                // Send request, get response
                var response = client.SendAsync(request).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
