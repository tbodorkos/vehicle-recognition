namespace Core.Entities.Prediction
{
    public class PredictionResult
    {
        public string Name { get; set; } = default!;
        public string InstanceId { get; set; } = default!;
        public string RuntimeStatus { get; set; } = default!;
        public object Input { get; set; } = default!;
        public string CustomStatus { get; set; } = default!;
        public PredictionOutput Output { get; set; } = default!;
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}
