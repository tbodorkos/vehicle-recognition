using Core.Entities;
using Core.Entities.Prediction;
using Newtonsoft.Json;
using System.Text;

namespace Web.Data
{
    public class RecognizerService : IRecognizerService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public RecognizerService(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }
        public async Task<Status> Predict(string url)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(new { url }), Encoding.UTF8, "application/json");
                var response = await _clientFactory.CreateClient().PostAsync(_configuration["FunctionUrl"], content);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Status>(json)!;
                }

                return new Status { ErrorMessage = $"Something went wrong - {response.ReasonPhrase}" };
            }
            catch (Exception e)
            {
                return new Status { ErrorMessage = e.Message };
            }
        }

        public async Task<PredictionResult> GetPredictionResult(string url)
        {
            try
            {
                var response = await _clientFactory.CreateClient().GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<PredictionResult>(json)!;
                    switch (result.RuntimeStatus)
                    {
                        case "Completed":
                        case "Failed":
                            {
                                return result;
                            }
                        case "Pending":
                        case "Running":
                            {
                                await Task.Delay(2000);
                                return await GetPredictionResult(url);
                            }
                    }
                }

                throw new Exception(response.ReasonPhrase); // TODO handle error status code
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw; // TODO handle error
            }
        }
    }
}
