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

        private static readonly string Username = "webapp";
        private static readonly string Password = "asd234";


        [FunctionName("GetPodInfo")]
        public static string Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var options = new RestClientOptions("https://172.17.8.30:6443/sap/bc/srt/rfc/sap/zwapp_get_pod_info/100/zwapp_get_pod_info/zwapp_get_pod_info")
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
