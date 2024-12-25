using System.Collections.Generic;

namespace MyXorNeuralNetworkApp.Models
{
    public class Layer
    {
        public List<Neuron> Neurons { get; private set; }

        public Layer(int neuronCount, int inputCount)
        {
            Neurons = new List<Neuron>();
            for (int i = 0; i < neuronCount; i++)
            {
                Neurons.Add(new Neuron(inputCount));
            }
        }

        // Получение выходов слоя
        public List<double> GetOutputs(List<double> inputs)
        {
            List<double> outputs = new List<double>();
            foreach (var neuron in Neurons)
            {
                outputs.Add(neuron.ComputeOutput(inputs));
            }
            return outputs;
        }
    }
}