using System;
using System.Collections.Generic;
using MyXorNeuralNetworkApp.Models;

namespace MyXorNeuralNetworkApp.Services
{
    public class NeuralNetworkService
    {
        // Можно хранить одну сеть на все приложение (Singleton),
        // либо можно создавать новый экземпляр под каждого клиента/запрос – зависит от задачи.
        
        private NeuralNetwork _network;
        private List<TrainingData> _trainingSet;
        
        public NeuralNetworkService()
        {
            // Инициализация: по умолчанию зададим 3 нейрона в скрытом слое, 1 выход, 8 входов
            // Но оставим возможность переинициализировать (InitializeNetwork).
            InitializeNetwork(8, 3, 1, 0.1);
        }

        public void InitializeNetwork(int inputSize, int hiddenNeurons, int outputNeurons, double learningRate)
        {
            _network = new NeuralNetwork(inputSize, hiddenNeurons, outputNeurons, learningRate);
            _trainingSet = GenerateTrainingData(inputSize);
        }

        // Генерация обучающего набора
        private List<TrainingData> GenerateTrainingData(int numberOfInputs)
        {
            var dataSet = new List<TrainingData>();
            int totalCombinations = (int)Math.Pow(2, numberOfInputs);

            for (int i = 0; i < totalCombinations; i++)
            {
                List<double> inputs = new List<double>();
                int temp = i;
                int countOnes = 0;

                for (int bit = 0; bit < numberOfInputs; bit++)
                {
                    double input = (temp & 1) == 1 ? 1.0 : 0.0;
                    inputs.Add(input);
                    if (input == 1.0) countOnes++;
                    temp >>= 1;
                }

                // 1, если нечётное количество единиц, иначе 0
                double expectedOutput = (countOnes % 2 == 1) ? 1.0 : 0.0;

                dataSet.Add(new TrainingData(inputs, new List<double> { expectedOutput }));
            }
            return dataSet;
        }

        public string TrainNetwork(int epochs, double threshold)
        {
            if (_network == null || _trainingSet == null)
                return "Сеть не инициализирована. Сначала вызовите InitializeNetwork.";

            for (int epoch = 1; epoch <= epochs; epoch++)
            {
                foreach (var data in _trainingSet)
                {
                    _network.Train(data.Inputs, data.ExpectedOutputs);
                }

                // Каждые N эпох можно проверять ошибку
                if (epoch % 1000 == 0)
                {
                    double error = _network.CalculateError(_trainingSet);
                    if (error < threshold)
                    {
                        return $"Порог ошибки достигнут на эпохе {epoch}. Ошибка: {error}";
                    }
                }
            }

            double finalError = _network.CalculateError(_trainingSet);
            return $"Обучение завершено после {epochs} эпох. Итоговая ошибка: {finalError}";
        }

        public List<object> TestNetwork()
        {
            if (_network == null || _trainingSet == null)
                return null;

            var results = new List<object>();

            foreach (var data in _trainingSet)
            {
                var output = _network.ForwardPass(data.Inputs);
                double predicted = output[0] >= 0.5 ? 1 : 0;

                results.Add(new 
                {
                    Inputs = data.Inputs,
                    Expected = data.ExpectedOutputs[0],
                    Predicted = predicted
                });
            }

            return results;
        }


        public object GetNetworkParameters()
        {
            if (_network == null) return null;

            var layersInfo = new List<object>();
            for (int layerIndex = 0; layerIndex < _network.Layers.Count; layerIndex++)
            {
                var layer = _network.Layers[layerIndex];
                var neuronInfos = new List<object>();

                for (int neuronIndex = 0; neuronIndex < layer.Neurons.Count; neuronIndex++)
                {
                    var neuron = layer.Neurons[neuronIndex];
                    neuronInfos.Add(new 
                    {
                        NeuronIndex = neuronIndex + 1,
                        Weights = neuron.Weights,
                        Bias = neuron.Bias
                    });
                }

                layersInfo.Add(new 
                {
                    LayerIndex = layerIndex + 1,
                    Neurons = neuronInfos
                });
            }

            return layersInfo;
        }
    }
}
