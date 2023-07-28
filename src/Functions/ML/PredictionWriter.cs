using Functions.ML.ImageDataStructures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Core.Utils
{
    public static class PredictionWriter
    {
        public static void DisplayResults(IEnumerable<ImagePrediction> predictions)
        {
            foreach (var prediction in predictions)
            {
                Print(prediction);
            }
        }

        public static void Print(ImagePrediction prediction)
        {
            var defaultForeground = Console.ForegroundColor;
            var labelColor = ConsoleColor.Magenta;
            var probColor = ConsoleColor.Blue;
            var exactLabel = ConsoleColor.Green;

            Console.Write("Image: ");
            Console.ForegroundColor = labelColor;
            Console.Write($"{Path.GetFileName(prediction.ImagePath)}");
            Console.ForegroundColor = defaultForeground;
            Console.Write(" labeled as ");
            Console.ForegroundColor = labelColor;
            Console.Write(prediction.Label);
            Console.ForegroundColor = defaultForeground;
            Console.Write(" predicted as ");
            Console.ForegroundColor = exactLabel;
            Console.Write($"{prediction.PredictedLabelValue}");
            Console.ForegroundColor = defaultForeground;
            Console.Write(" with probability ");
            Console.ForegroundColor = probColor;
            Console.Write(prediction.Score.Max());
            Console.ForegroundColor = defaultForeground;
            Console.WriteLine("");
        }
    }
}
