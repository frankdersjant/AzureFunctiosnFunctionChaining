using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace FunctionChaining
{
    public class ChainedFunction
    {
        private readonly ILogger _logger;

        public ChainedFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ChainedFunction>();
        }

        [Function("ChainedFunction")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req, FunctionContext context)
        {
            var logger = context.GetLogger<ChainedFunction>();

            // Perform an activity using the received data
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            string data = JsonSerializer.Deserialize<string>(requestBody);

            logger.LogInformation($"Performing chained activity with data: {data}");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            return response;
        }
    }
}
