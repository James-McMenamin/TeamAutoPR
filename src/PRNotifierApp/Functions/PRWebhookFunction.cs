using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PRNotifierApp.Services;

namespace PRNotifierApp.Functions
{
    public class PRWebhookFunction
    {
        private readonly IPRNotifierService _prNotifierService;
        private readonly ILogger _logger;

        public PRWebhookFunction(IPRNotifierService prNotifierService, ILoggerFactory loggerFactory)
        {
            _prNotifierService = prNotifierService;
            _logger = loggerFactory.CreateLogger<PRWebhookFunction>();
        }

        [Function("PRWebhook")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic? data = JsonConvert.DeserializeObject(requestBody);

            if (data?.eventType == "git.pullrequest.created" ||
                data?.eventType == "git.pullrequest.updated")
            {
                await _prNotifierService.ProcessPullRequestEventAsync(data);
                var response = req.CreateResponse(HttpStatusCode.OK);
                return response;
            }

            var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badRequestResponse.WriteStringAsync(JsonConvert.SerializeObject(new { error = "Unsupported event type" }));
            return badRequestResponse;
        }
    }
}