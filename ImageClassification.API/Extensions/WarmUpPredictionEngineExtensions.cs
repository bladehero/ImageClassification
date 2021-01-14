using ImageClassification.Shared.DataModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ImageClassification.API.Extensions
{
    public static class WarmUpPredictionEngineExtensions
    {
        public static void WarmUpPredictionEnginePool(this IServiceCollection services, string warmupImage, byte additionalTestCount = 5)
        {
            var predictionEnginePool =
                services.BuildServiceProvider()
                        .GetRequiredService<PredictionEnginePool<InMemoryImageData, ImagePrediction>>();
            var predictionEngine = predictionEnginePool.GetPredictionEngine();
            predictionEnginePool.ReturnPredictionEngine(predictionEngine);

            // Get a sample image
            var img = Image.FromFile(warmupImage);
            byte[] imageData;
            using (MemoryStream ms = new MemoryStream())
            {
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                imageData = ms.ToArray();
            }

            var imageInputData = new InMemoryImageData(image: imageData, label: null, imageFileName: null);

            // Measure execution time.
            var watch = System.Diagnostics.Stopwatch.StartNew();

            ImagePrediction prediction = predictionEnginePool.Predict(imageInputData);
            for (int i = 1; i < additionalTestCount; i++)
            {
                predictionEnginePool.Predict(imageInputData);
            }

            // Stop measuring time.
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            Console.WriteLine();
            Console.WriteLine(new string('*', 25));
            Console.WriteLine();
            Console.WriteLine("Warmup prediction: {0}, {1}% - {2} sec.",
                              prediction.PredictedLabel,
                              prediction.Score.Max() * 100,
                              elapsedMs / 1000.0);
            Console.WriteLine();
            Console.WriteLine(new string('*', 25));
            Console.WriteLine();
        }
    }
}
