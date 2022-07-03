using Microsoft.ML;
using Microsoft.ML.Trainers;
using Microsoft.ML.Data;
using Newtonsoft.Json;

namespace MatrixFactorization
{
    internal class Methods
    {
        public class Data
        {
            // Get Linear Regression Dataset from api
            public async Task<List<DataSet.MatrixFactorization>> Get()
            {
                // API uri
                string uri = "http://api.dalilak.pro/Query/MF_DataSet_";
                using (var client = new HttpClient())
                {
                    // initialize Get Request
                    var response = await client.GetAsync(uri);
                    var content = response.Content.ReadAsStringAsync().Result;

                    // Convert the content of the request from string to DataSet.LinearRegression
                    return JsonConvert.DeserializeObject<List<DataSet.MatrixFactorization>>(content);
                }
            }

            public void NormalizeBinning(ref List<DataSet.MatrixFactorization> ds)
            {

                // Compress the numbers between 0 and 1
                foreach (var item in ds)
                {
                    item.rate = (item.rate - 1)/(5 - 1);
                }
            }

        }

        public class MF
        {
            public ITransformer Train(ref MLContext ml, DataOperationsCatalog.TrainTestData d)
            {
                IEstimator<ITransformer> estimator = ml.Transforms.Conversion.MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: "user_id")
                .Append(ml.Transforms.Conversion.MapValueToKey(outputColumnName: "placeIdEncoded", inputColumnName: "place_id"));

                var options = new MatrixFactorizationTrainer.Options
                {
                    MatrixColumnIndexColumnName = "userIdEncoded",
                    MatrixRowIndexColumnName = "placeIdEncoded",
                    LabelColumnName = "rate",
                    NumberOfIterations = 250,
                    ApproximationRank = 100
                };

                var trainerEstimator = estimator.Append(ml.Recommendation().Trainers.MatrixFactorization(options));

                Console.WriteLine("=============== Training Matrix Factorization model ===============");
                var model = trainerEstimator.Fit(d.TrainSet);

                return model;
            }
            public RegressionMetrics Evaluate(MLContext ml, DataOperationsCatalog.TrainTestData d, ITransformer model)
            {
                var prediction = model.Transform(d.TestSet);

                return ml.Regression.Evaluate(prediction, labelColumnName: "rate", scoreColumnName: "Score");
            }

            public void TestSinglePrediction(MLContext ml, string modelPath, string[] sample)
            {
                // Loading model
                DataViewSchema modelSchema;
                ITransformer model = ml.Model.Load(modelPath, out modelSchema);

                var predictionEngine = ml.Model.CreatePredictionEngine<DataSet.MatrixFactorization, Recommender_Predictions>(model);

                var testInput = new DataSet.MatrixFactorization { user_id = sample[0], place_id = sample[1] };

                var RatingPrediction = predictionEngine.Predict(testInput);

                Console.WriteLine("Recommendation of place (" + testInput.place_id + ") for User (" + testInput.user_id + ") has score: " + RatingPrediction.Score);

            }

        }
    }
}
