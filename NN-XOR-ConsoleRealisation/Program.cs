using System;
using System.Collections.Generic;
using System.Linq;

namespace TripleXORNeuralNetwork
{
    // Класс, представляющий отдельный нейрон
    public class Neuron
    {
        public List<double> Weights { get; private set; }
        public double Bias { get; set; }
        public double Output { get; private set; }
        public double Delta { get; set; }

        private static Random rnd = new Random();

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

    // Класс, представляющий слой нейронов
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

    // Класс, представляющий нейронную сеть
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

        // Метод прямого прохода (Forward Pass)
        public List<double> ForwardPass(List<double> inputs)
        {
            List<double> outputs = inputs;
            foreach (var layer in Layers)
            {
                outputs = layer.GetOutputs(outputs);
            }
            return outputs;
        }

        // Метод обратного распространения ошибки (Backpropagation)
        public void Backpropagate(List<double> inputs, List<double> expectedOutputs)
        {
            // Прямой проход
            List<double> actualOutputs = ForwardPass(inputs);

            // Вычисление ошибки для выходного слоя
            Layer outputLayer = Layers.Last();
            for (int i = 0; i < outputLayer.Neurons.Count; i++)
            {
                Neuron neuron = outputLayer.Neurons[i];
                double error = expectedOutputs[i] - neuron.Output;
                neuron.Delta = error * Neuron.SigmoidDerivative(neuron.Output);
            }

            // Вычисление ошибки для скрытого слоя
            for (int i = Layers.Count - 2; i >= 0; i--)
            {
                Layer currentLayer = Layers[i];
                Layer nextLayer = Layers[i + 1];
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

            // Обновление весов и смещений
            for (int i = 0; i < Layers.Count; i++)
            {
                Layer layer = Layers[i];
                List<double> layerInputs = (i == 0) ? inputs : Layers[i - 1].Neurons.Select(n => n.Output).ToList();

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

        // Метод обучения сети на одном примере
        public void Train(List<double> inputs, List<double> expectedOutputs)
        {
            Backpropagate(inputs, expectedOutputs);
        }

        // Метод для оценки точности сети на наборе данных
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

    // Класс, представляющий обучающие данные
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

    // Класс для управления меню и взаимодействием с пользователем
    public class Menu
    {
        private NeuralNetwork network;
        private List<TrainingData> trainingSet;

        public Menu()
        {
            // Инициализация нейронной сети с 3 входами, 3 нейронами в скрытом слое и 1 нейроном в выходном слое
            Console.Write("Введите количество нейронов в скрытом слое (рекомендуется 2 или 3): ");
            int hiddenNeurons = 3;
            string inputNeurons = Console.ReadLine();
            if (!int.TryParse(inputNeurons, out hiddenNeurons) || hiddenNeurons <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Некорректный ввод. Используется значение по умолчанию: 3 нейрона.");
                Console.ResetColor();
                hiddenNeurons = 3;
            }

            network = new NeuralNetwork(8, hiddenNeurons, 1, 0.1); // Пользователь задаёт количество скрытых нейронов

            // Определение обучающего набора для XOR с тремя входами
            trainingSet = GenerateTrainingData(8);

        }
        
        // Метод для генерации обучающих данных программно
        private List<TrainingData> GenerateTrainingData(int numberOfInputs)
        {
            List<TrainingData> dataSet = new List<TrainingData>();
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
                    if (input == 1.0)
                        countOnes++;
                    temp >>= 1;
                }

                // Определение ожидаемого выхода (1 если нечётное количество единиц, иначе 0)
                double expectedOutput = (countOnes % 2 == 1) ? 1.0 : 0.0;

                dataSet.Add(new TrainingData(inputs, new List<double> { expectedOutput }));
            }

            return dataSet;
        }

        public void Display()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("=== Многослойный Персептрон для XOR (Три Входа) ===");
                Console.WriteLine("1. Обучить сеть");
                Console.WriteLine("2. Протестировать сеть");
                Console.WriteLine("3. Показать параметры сети");
                Console.WriteLine("4. Выход");
                Console.Write("Выберите опцию: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        TrainNetwork();
                        break;
                    case "2":
                        TestNetwork();
                        break;
                    case "3":
                        ShowNetworkParameters();
                        break;
                    case "4":
                        exit = true;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Неверный выбор. Пожалуйста, попробуйте снова.");
                        Console.ResetColor();
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }

        private void TrainNetwork()
        {
            Console.Write("Введите количество эпох обучения (например, 10000): ");
            if (!int.TryParse(Console.ReadLine(), out int epochs) || epochs <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Некорректный ввод количества эпох.");
                Console.ResetColor();
                return;
            }

            Console.Write("Введите порог ошибки (например, 0.001): ");
            if (!double.TryParse(Console.ReadLine(), out double threshold) || threshold <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Некорректный ввод порога ошибки.");
                Console.ResetColor();
                return;
            }

            Console.WriteLine("Обучение началось...");

            for (int epoch = 1; epoch <= epochs; epoch++)
            {
                foreach (var data in trainingSet)
                {
                    network.Train(data.Inputs, data.ExpectedOutputs);
                }

                if (epoch % 1000 == 0)
                {
                    double error = network.CalculateError(trainingSet);
                    Console.WriteLine($"Эпоха {epoch}, Ошибка: {error:F6}");
                    if (error < threshold)
                    {
                        Console.WriteLine("Порог ошибки достигнут. Обучение завершено.");
                        return;
                    }
                }
            }

            Console.WriteLine("Обучение завершено.");
        }

        private void TestNetwork()
        {
            Console.WriteLine("Тестирование сети на данных XOR (Три Входа):");
            foreach (var data in trainingSet)
            {
                var output = network.ForwardPass(data.Inputs);
                double predicted = output[0] >= 0.5 ? 1 : 0;
                Console.WriteLine($"Входы: {string.Join(", ", data.Inputs)} | Ожидаемый выход: {data.ExpectedOutputs[0]} | Предсказанный выход: {predicted}");
            }
        }

        private void ShowNetworkParameters()
        {
            Console.WriteLine("=== Параметры Нейронной Сети ===");
            for (int layerIndex = 0; layerIndex < network.Layers.Count; layerIndex++)
            {
                var layer = network.Layers[layerIndex];
                Console.WriteLine($"Слой {layerIndex + 1}:");
                for (int neuronIndex = 0; neuronIndex < layer.Neurons.Count; neuronIndex++)
                {
                    var neuron = layer.Neurons[neuronIndex];
                    Console.WriteLine($"  Нейрон {neuronIndex + 1}:");
                    Console.WriteLine($"    Веса: {string.Join(", ", neuron.Weights.Select(w => $"{w:F4}"))}");
                    Console.WriteLine($"    Смещение (Bias): {neuron.Bias:F4}");
                }
            }
        }
    }

    // Основной класс программы
    class Program
    {
        static void Main(string[] args)
        {
            Menu menu = new Menu();
            menu.Display();

            Console.WriteLine("Спасибо за использование приложения! Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
