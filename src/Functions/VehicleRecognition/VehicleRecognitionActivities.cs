using Core.Utils;
using Functions.ML;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Functions.VehicleRecognition
{
    public class RecognizeVehicleActivities
    {
        private readonly ITFModelScorer _tfModelScorer;

        public RecognizeVehicleActivities(ITFModelScorer tfModelScorer)
        {
            _tfModelScorer = tfModelScorer;
        }

        [FunctionName("A_TrainModel")]
        public async Task TrainModel([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("Training model");
            await _tfModelScorer.Train();
        }

        [FunctionName("A_TestModel")]
        public async Task TestModel([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("Testing model");
            await _tfModelScorer.Test();
        }

        [FunctionName("A_PredictImage")]
        public async Task<string> PredictImage([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("Predicting image");
            return await _tfModelScorer.ClassifySingleImage(input);
        }

        [FunctionName("A_SaveImage")]
        public static async Task<string> SaveImage([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("Saving image");
            
            var saveLocation = @Path.Combine(Environment.CurrentDirectory, "assets", "images", "prediction.jpg");

            using var imageResponse = await WebRequest.Create(input).GetResponseAsync();
            using var responseStream = imageResponse.GetResponseStream();
            using var binaryReader = new BinaryReader(responseStream);

            FileWriter.Save(binaryReader.ReadBytes(500000), saveLocation);

            return saveLocation;
        }

        [FunctionName("A_DeleteImage")]
        public static void DeleteImage([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("Deleting image");
            FileWriter.Delete(input);
        }
    }
}
