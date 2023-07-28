using Core.Entities.Prediction;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace Functions.VehicleRecognition
{
    public class VehicleRecognitionStarter
    {
        [FunctionName("VehicleRecognitionStarter")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, [DurableClient] IDurableOrchestrationClient starter, ILogger log)
        {
            log.LogInformation("VehicleRecognition function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var vehicleUrl = JsonConvert.DeserializeObject<PredictionInput>(requestBody);

            if (vehicleUrl == null)
            {
                return new BadRequestObjectResult("Please pass the vehicle url in the request body");
            }

            log.LogInformation($"About to start orchestration for {vehicleUrl}");

            var instanceId = await starter.StartNewAsync("O_RecognizeVehicle", vehicleUrl);
            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
