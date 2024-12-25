using System.Collections.Generic;

namespace MyXorNeuralNetworkApp.Models
{
    public class TrainingData
    {
        public List<double> Inputs { get; set; }
        public List<double> ExpectedOutputs { get; set; }

        public TrainingData(List<double> inputs, List<double> expectedOutputs)
        {
            Inputs = inputs;
            ExpectedOutputs = expectedOutputs;
        }
    }
}