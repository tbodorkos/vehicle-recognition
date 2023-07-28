using Core.Entities;
using Core.Entities.Prediction;

namespace Web.Data
{
    public interface IRecognizerService
    {
        Task<Status> Predict(string url);
        Task<PredictionResult> GetPredictionResult(string url);
    }
}
