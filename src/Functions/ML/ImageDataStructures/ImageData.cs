using Microsoft.ML.Data;

namespace Functions.ML.ImageDataStructures
{
    public class ImageData
    {
        [LoadColumn(0)]
        public string ImagePath;

        [LoadColumn(1)]
        public string Label;
    }
}
