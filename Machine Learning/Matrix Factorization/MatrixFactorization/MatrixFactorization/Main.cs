using Microsoft.ML;
using MatrixFactorization;

/*  Declaring Required Variables */
Methods.Data _data = new Methods.Data();
Methods.MF _mf = new Methods.MF();
MLContext mlContext = new MLContext(seed: 1); 


/* Dataset */
List<DataSet.MatrixFactorization> dataset = new List<DataSet.MatrixFactorization>();
dataset = await _data.Get();
_data.NormalizeBinning(ref dataset);

// Load Dataset
IDataView dataView = mlContext.Data.LoadFromEnumerable<DataSet.MatrixFactorization>(dataset);

// Split Dataset
var split = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.3);

// Train
var model = _mf.Train(ref mlContext, split);

// Evaluate
var metrics = _mf.Evaluate(mlContext, split, model);

Console.WriteLine("Root Mean Squared Error : " + metrics.RootMeanSquaredError.ToString());
Console.WriteLine("RSquared: " + metrics.RSquared.ToString());


// save
mlContext.Model.Save(model, dataView.Schema, Path.Combine(Environment.CurrentDirectory, "MatrixFactorizationModel", "Model.zip"));


// One sample
_mf.TestSinglePrediction(mlContext,
                         Path.Combine(Environment.CurrentDirectory, "MatrixFactorizationModel", "Model.zip"),
                         new string[] { "d9f441de48234ac0988e81e751bffdea", "c2718533cb2c4fad93ac190bd057052c" });




