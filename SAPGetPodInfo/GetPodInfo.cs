using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Text;

namespace SAPGetPodInfo
{
    public static class GetPodInfo
    {

        private static readonly string Username = "username";
        private static readonly string Password = "password";


        [FunctionName("GetPodInfo")]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var options = new RestClientOptions("https://1.1.1.1:1010/src/dev/null")
            {
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            var client = new RestClient(options);

            var request = new RestRequest("POST");
            request.AddHeader("Content-Type", "text/xml");
            request.AddHeader("SOAPAction", "\"#POST\"");
            request.AddHeader("Authorization", $"Basic {Base64Encode($"{Username}:{Password}")}");
            request.AddHeader("Cookie", "sap-usercontext=sap-client=100");

            var body = @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:sap-com:document:sap:soap:functions:mc-style"">" + "\n" +
            @"   <soapenv:Header/>" + "\n" +
            @"   <soapenv:Body>" + "\n" +
            @"      <urn:ZWappGetPodInfo>" + "\n" +
            @"         <Pod>935531959</Pod>" + "\n" +
            @"      </urn:ZWappGetPodInfo>" + "\n" +
            @"   </soapenv:Body>" + "\n" +
            @"</soapenv:Envelope>";

            request.AddParameter("text/xml", body, ParameterType.RequestBody);

            var response = client.Execute(request);

            return response.Content;
        }

        public static string Base64Encode(string textToEncode)
        {
            byte[] textAsBytes = Encoding.UTF8.GetBytes(textToEncode);
            return Convert.ToBase64String(textAsBytes);
        }
    }
}
