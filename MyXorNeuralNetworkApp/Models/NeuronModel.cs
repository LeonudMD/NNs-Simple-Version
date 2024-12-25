using System;
using System.Collections.Generic;

namespace MyXorNeuralNetworkApp.Models
{
    public class Neuron
    {
        private static Random rnd = new Random();

        public List<double> Weights { get; private set; }
        public double Bias { get; set; }
        public double Output { get; private set; }
        public double Delta { get; set; }

        public Neuron(int inputCount)
        {
            Weights = new List<double>();
            for (int i = 0; i < inputCount; i++)
            {
                // Инициализация весов случайными значениями от -1 до 1
                Weights.Add(rnd.NextDouble() * 2 - 1);
            }
            Bias = rnd.NextDouble() * 2 - 1;
        }

        // Сигмоидальная функция активации
        public static double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        // Производная сигмоидальной функции
        public static double SigmoidDerivative(double output)
        {
            return output * (1.0 - output);
        }

        // Вычисление выхода нейрона
        public double ComputeOutput(List<double> inputs)
        {
            if (inputs.Count != Weights.Count)
                throw new ArgumentException("Количество входов не совпадает с количеством весов.");

            double sum = Bias;
            for (int i = 0; i < inputs.Count; i++)
            {
                sum += inputs[i] * Weights[i];
            }
            Output = Sigmoid(sum);
            return Output;
        }
    }
}