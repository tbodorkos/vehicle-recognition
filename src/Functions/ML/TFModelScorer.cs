﻿using Core.Utils;
using Functions.ML.ImageDataStructures;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Functions.ML
{
    public class TFModelScorer : ITFModelScorer
    {
        private const string COLUMN_NAME = "input";
        private const string IMAGES_FOLDER = "assets/images";
        private const string TENSORFLOW_MODEL = "assets/inception/tensorflow_inception_graph.pb";

        private ITransformer _model;
        private readonly MLContext _mlContext;

        public TFModelScorer(MLContext mlContext)
        {
            _mlContext = mlContext;
        }

        public async Task<string> ClassifySingleImage(string imagePath)
        {
            var imageData = new ImageData { ImagePath = imagePath };

            try
            {
                var predictor = _mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(_model);
                var prediction = predictor.Predict(imageData);

                PredictionWriter.Print(prediction);

                return await Task.FromResult($"Predicted as {prediction.PredictedLabelValue} with probability of {prediction.Score.Max()}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public async Task<bool> Train()
        {
            if (_model != null)
            {
                return await Task.FromResult(true);
            }

            try
            {
                IEstimator<ITransformer> pipeline = _mlContext.Transforms.LoadImages(outputColumnName: COLUMN_NAME, imageFolder: IMAGES_FOLDER, inputColumnName: nameof(ImageData.ImagePath))
                    // The image transforms transform the images into the model's expected format.
                    .Append(_mlContext.Transforms.ResizeImages(outputColumnName: COLUMN_NAME, imageWidth: ImageSettings.imageWidth, imageHeight: ImageSettings.imageHeight, inputColumnName: COLUMN_NAME))
                    .Append(_mlContext.Transforms.ExtractPixels(outputColumnName: COLUMN_NAME, interleavePixelColors: ImageSettings.channelsLast, offsetImage: ImageSettings.mean))
                    .Append(_mlContext.Model.LoadTensorFlowModel(TENSORFLOW_MODEL)
                    .ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { COLUMN_NAME }, addBatchDimensionInput: true))
                    .Append(_mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: "Label"))
                    .Append(_mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
                    .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
                    .AppendCacheCheckpoint(_mlContext);

                var data = new ImageData[]
                {
                    new ImageData { ImagePath = "car1.jpg", Label = "Car"},
                    new ImageData { ImagePath = "car2.jpg", Label = "Car"},
                    new ImageData { ImagePath = "car3.jpg", Label = "Car"},
                    new ImageData { ImagePath = "train1.jpg", Label = "Train"},
                    new ImageData { ImagePath = "train2.jpg", Label = "Train"},
                    new ImageData { ImagePath = "train3.jpg", Label = "Train"},
                    new ImageData { ImagePath = "ship1.jpg", Label = "Ship"},
                    new ImageData { ImagePath = "ship2.jpg", Label = "Ship"},
                    new ImageData { ImagePath = "ship3.jpg", Label = "Ship"}
                };

                IDataView trainingData = _mlContext.Data.LoadFromEnumerable(data);
                _model = pipeline.Fit(trainingData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            return await Task.FromResult(true);
        }

        public Task Test()
        {
            if (_model != null)
            {
                return Task.CompletedTask;
            }

            var data = new ImageData[]
            {
                new ImageData { ImagePath = "car4.jpg", Label = "Car"},
                new ImageData { ImagePath = "train4.jpg", Label = "Train"},
                new ImageData { ImagePath = "ship4.jpg", Label = "Ship"}
            };

            IDataView testData = _mlContext.Data.LoadFromEnumerable(data);
            IDataView predictions = _model.Transform(testData);

            // Create an IEnumerable for the predictions for displaying results
            IEnumerable<ImagePrediction> imagePredictionData = _mlContext.Data.CreateEnumerable<ImagePrediction>(predictions, true);
            PredictionWriter.DisplayResults(imagePredictionData);

            MulticlassClassificationMetrics metrics =
                _mlContext.MulticlassClassification.Evaluate(predictions,
                  labelColumnName: "LabelKey",
                  predictedLabelColumnName: "PredictedLabel");

            Console.WriteLine($"LogLoss is: {metrics.LogLoss}");
            Console.WriteLine($"PerClassLogLoss is: {string.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");

            return Task.CompletedTask;
        }

        public struct ImageSettings
        {
            public const int imageHeight = 224;
            public const int imageWidth = 224;
            public const float mean = 117;
            public const bool channelsLast = true;
        }
    }
}
