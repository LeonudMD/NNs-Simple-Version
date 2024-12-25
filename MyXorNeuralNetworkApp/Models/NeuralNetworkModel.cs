using System;
using System.Collections.Generic;
using System.Linq;

namespace MyXorNeuralNetworkApp.Models
{
    public class NeuralNetwork
    {
        public List<Layer> Layers { get; private set; }
        private double learningRate;

        public NeuralNetwork(int inputSize, int hiddenNeurons, int outputNeurons, double learningRate = 0.1)
        {
            Layers = new List<Layer>();
            Layers.Add(new Layer(hiddenNeurons, inputSize)); // Скрытый слой
            Layers.Add(new Layer(outputNeurons, hiddenNeurons)); // Выходной слой
            this.learningRate = learningRate;
        }

        // Прямой проход
        public List<double> ForwardPass(List<double> inputs)
        {
            List<double> outputs = inputs;
            foreach (var layer in Layers)
            {
                outputs = layer.GetOutputs(outputs);
            }
            return outputs;
        }

        // Обратное распространение ошибки
        public void Backpropagate(List<double> inputs, List<double> expectedOutputs)
        {
            // 1. Прямой проход
            List<double> actualOutputs = ForwardPass(inputs);

            // 2. Ошибка выходного слоя
            var outputLayer = Layers.Last();
            for (int i = 0; i < outputLayer.Neurons.Count; i++)
            {
                var neuron = outputLayer.Neurons[i];
                double error = expectedOutputs[i] - neuron.Output;
                neuron.Delta = error * Neuron.SigmoidDerivative(neuron.Output);
            }

            // 3. Ошибка скрытых слоёв
            for (int i = Layers.Count - 2; i >= 0; i--)
            {
                var currentLayer = Layers[i];
                var nextLayer = Layers[i + 1];
                for (int j = 0; j < currentLayer.Neurons.Count; j++)
                {
                    double error = 0.0;
                    for (int k = 0; k < nextLayer.Neurons.Count; k++)
                    {
                        error += nextLayer.Neurons[k].Weights[j] * nextLayer.Neurons[k].Delta;
                    }
                    currentLayer.Neurons[j].Delta = error * Neuron.SigmoidDerivative(currentLayer.Neurons[j].Output);
                }
            }

            // 4. Обновление весов и смещений
            for (int i = 0; i < Layers.Count; i++)
            {
                var layer = Layers[i];
                List<double> layerInputs = (i == 0) 
                    ? inputs 
                    : Layers[i - 1].Neurons.Select(n => n.Output).ToList();

                foreach (var neuron in layer.Neurons)
                {
                    for (int j = 0; j < neuron.Weights.Count; j++)
                    {
                        neuron.Weights[j] += learningRate * neuron.Delta * layerInputs[j];
                    }
                    neuron.Bias += learningRate * neuron.Delta;
                }
            }
        }

        // Обучение сети на одном примере
        public void Train(List<double> inputs, List<double> expectedOutputs)
        {
            Backpropagate(inputs, expectedOutputs);
        }

        // Расчёт ошибки
        public double CalculateError(List<TrainingData> trainingSet)
        {
            double totalError = 0.0;
            foreach (var data in trainingSet)
            {
                var output = ForwardPass(data.Inputs);
                for (int i = 0; i < output.Count; i++)
                {
                    double error = data.ExpectedOutputs[i] - output[i];
                    totalError += error * error;
                }
            }
            return totalError / trainingSet.Count;
        }
    }
}
