using System.Net;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FunctionChaining
{
    public class HttpTriggerFunction
    {
        private readonly ILogger _logger;

        public HttpTriggerFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpTriggerFunction>();
        }

        [Function("Function1")]
        public async Task <HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP triggered - starting the chained function.");

            string data = " Oh Hello there";

            string result = await CallActivityFunction(data);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            return response;
        }

        public static async Task<string> CallActivityFunction(string data)
        {
            // Call another Azure Function from here
            var functionName = "ChainedFunction"; 
            
            var client = new HttpClient();
            var apiUrl = $"http://localhost:7095/api/{functionName}"; // Replace with the actual URL
            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            else
            {
                return $"Error calling {functionName}: {response.StatusCode}";
            }
        }
    }
}
