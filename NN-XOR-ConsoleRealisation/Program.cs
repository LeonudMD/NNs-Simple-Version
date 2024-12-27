using System.Runtime.Serialization.Formatters.Binary;

namespace TripleXORNeuralNetwork
{
    // Класс, представляющий отдельный нейрон
    [Serializable]
    public class Neuron
    {
        public List<double> Weights { get; set; }
        public double Bias { get; set; }
        public double Output { get; private set; }
        public double Delta { get; set; }
        [NonSerialized]
        private static Random rnd = new Random();

        public Neuron(int inputCount)
        {
            Weights = new List<double>();
            for (int i = 0; i < inputCount; i++)
            {
                // Инициализация весов случайными значениями от -1 до 1
                Weights.Add(rnd.NextDouble() - 0.5); // Диапазон [-0.5, 0.5]

            }
            Bias = rnd.NextDouble() - 0.5;
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
    [Serializable]
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
    // Класс, представляющий нейронную сеть
    public class NeuralNetwork
    {
        public List<Layer> Layers { get; private set; }
        private double learningRate;
        private const string FilePath = "network_parameters.nn";

        public NeuralNetwork(int inputSize, int hiddenNeurons, int outputNeurons, double learningRate = 0.1)
        {
            Layers = new List<Layer>();

            // Проверяем наличие файла с параметрами
            if (File.Exists(FilePath))
            {
                LoadParametersBinary();
            }
            else
            {
                // Инициализация сети с нуля
                Layers.Add(new Layer(hiddenNeurons, inputSize));
                Layers.Add(new Layer(hiddenNeurons, hiddenNeurons));
                Layers.Add(new Layer(outputNeurons, hiddenNeurons));
                this.learningRate = learningRate;
            }
        }

        // Сохранение параметров сети в файл .nn
        public void SaveParametersBinary()
        {
            try
            {
                using (var fileStream = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(fileStream, Layers);
                }
                Console.WriteLine("Параметры сети успешно сохранены в файл .nn.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка сохранения параметров сети: {ex.Message}");
                Console.ResetColor();
            }
        }

        // Загрузка параметров сети из файла .nn
        public void LoadParametersBinary()
        {
            try
            {
                using (var fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                {
                    var formatter = new BinaryFormatter();
                    Layers = (List<Layer>)formatter.Deserialize(fileStream);
                }
                Console.WriteLine("Параметры сети успешно загружены из файла .nn.");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка загрузки параметров сети: {ex.Message}");
                Console.ResetColor();
            }
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

            // Вычисление ошибки для скрытых слоёв
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
            /*Console.Write("Введите количество нейронов в скрытом слое (рекомендуется 2 или 3): ");
            int hiddenNeurons = 3;
            string inputNeurons = Console.ReadLine();
            if (!int.TryParse(inputNeurons, out hiddenNeurons) || hiddenNeurons <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Некорректный ввод. Используется значение по умолчанию: 3 нейрона.");
                Console.ResetColor();
                hiddenNeurons = 3;
            }*/
            int input = 784;
            network = new NeuralNetwork(input, 30, 10); // Пользователь задаёт количество скрытых нейронов

            // Определение обучающего набора для XOR с тремя входами
            trainingSet = GenerateTrainingData(input);


        }

        // Метод для генерации обучающих данных программно
        private List<TrainingData> GenerateTrainingData(int numberOfInputs)
        {
            List<TrainingData> dataSet = new List<TrainingData>();

            // Пути к файлам
            string dataFilePath = "../../../inputs.txt"; // Файл с пикселями изображения
            string labelsFilePath = "../../../labels.txt"; // Файл с метками (классами)

            // Проверяем существование файлов
            if (!File.Exists(dataFilePath) || !File.Exists(labelsFilePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Файлы с данными не найдены. Убедитесь, что 'inputs.txt' и 'labels.txt' существуют.");
                Console.ResetColor();
                return dataSet;
            }

            try
            {
                // Считываем данные из файлов
                var dataLines = File.ReadAllLines(dataFilePath);
                var labelLines = File.ReadAllLines(labelsFilePath);

                // Убедимся, что количество строк совпадает
                if (dataLines.Length != labelLines.Length)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Количество строк в 'inputs.txt' и 'labels.txt' не совпадает.");
                    Console.ResetColor();
                    return dataSet;
                }

                // Парсим данные
                for (int i = 0; i < dataLines.Length; i++)
                {
                    // Преобразуем строку пикселей в список чисел
                    var inputValues = dataLines[i]
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(value => double.Parse(value) / 255.0) // Нормализация в диапазон [0, 1]
                        .ToList();


                    // Преобразуем метку в список чисел (например, one-hot encoding)
                    var label = int.Parse(labelLines[i]);
                    var outputValues = new List<double>(new double[10]); // Предполагаем 10 классов
                    outputValues[label] = 1.0;

                    // Проверяем количество входных данных
                    if (inputValues.Count != numberOfInputs)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Ошибка в строке {i + 1}: количество входных значений ({inputValues.Count}) не совпадает с ожидаемым ({numberOfInputs}).");
                        Console.ResetColor();
                        continue;
                    }

                    // Добавляем данные в набор
                    dataSet.Add(new TrainingData(inputValues, outputValues));
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Данные успешно загружены из файлов.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка при чтении файлов: {ex.Message}");
                Console.ResetColor();
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
                Random rnd = new Random();
                var shuffledTrainingSet = trainingSet.OrderBy(_ => rnd.Next()).ToList();

                foreach (var data in shuffledTrainingSet)
                {
                    network.Train(data.Inputs, data.ExpectedOutputs);
                }

                double error = network.CalculateError(trainingSet);
                Console.WriteLine($"Эпоха {epoch}, Ошибка: {error:F6}");
                if (error < threshold)
                {
                    Console.WriteLine("Порог ошибки достигнут. Обучение завершено.");

                    // Сохранение параметров после успешного завершения
                    network.SaveParametersBinary();
                    return;
                }
            }

            // Сохранение параметров после завершения обучения по эпохам
            network.SaveParametersBinary();
            Console.WriteLine("Обучение завершено.");
        }


        private void TestNetwork()
        {
            // Пути к файлам с проверочными данными
            string testDataFilePath = "../../../inputs2.txt"; // Файл с пикселями проверочных данных
            string testLabelsFilePath = "../../../labels2.txt"; // Файл с метками проверочных данных

            // Проверяем существование файлов
            if (!File.Exists(testDataFilePath) || !File.Exists(testLabelsFilePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Файлы с проверочными данными не найдены. Убедитесь, что 'input2.txt' и 'label2.txt' существуют.");
                Console.ResetColor();
                return;
            }

            try
            {
                // Считываем данные из файлов
                var testDataLines = File.ReadAllLines(testDataFilePath);
                var testLabelLines = File.ReadAllLines(testLabelsFilePath);

                // Убедимся, что количество строк совпадает
                if (testDataLines.Length != testLabelLines.Length)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Количество строк в 'inputs2.txt' и 'labels2.txt' не совпадает.");
                    Console.ResetColor();
                    return;
                }

                // Инициализация счётчика ошибок
                int totalSamples = testDataLines.Length;
                int correctPredictions = 0;

                // Тестирование сети
                Console.WriteLine("Тестирование сети на проверочных данных:");

                for (int i = 0; i < testDataLines.Length; i++)
                {
                    // Преобразуем строку пикселей в список чисел

                    var inputValues = testDataLines[i]
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(value => double.Parse(value) / 255.0) // Нормализация в диапазон [0, 1]
                        .ToList();

                    // Читаем ожидаемую метку
                    var expectedLabel = int.Parse(testLabelLines[i]);

                    // Получаем предсказания сети
                    var output = network.ForwardPass(inputValues);

                    // Находим класс с максимальной вероятностью
                    int predictedLabel = output.IndexOf(output.Max());

                    // Проверяем корректность предсказания
                    if (predictedLabel == expectedLabel)
                    {
                        correctPredictions++;
                    }

                    Console.WriteLine($"Ожидаемый выход: {expectedLabel} | Предсказанный выход: {predictedLabel}");
                }

                // Расчёт процента ошибок
                double accuracy = (double)correctPredictions / totalSamples * 100;
                double errorRate = 100 - accuracy;

                Console.WriteLine($"\nВсего тестовых выборок: {totalSamples}");
                Console.WriteLine($"Правильных предсказаний: {correctPredictions}");
                Console.WriteLine($"Точность: {accuracy:F2}%");
                Console.WriteLine($"Процент ошибок: {errorRate:F2}%");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Тестирование завершено.");
                Console.ResetColor();
            }

            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка при чтении проверочных данных: {ex.Message}");
                Console.ResetColor();
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
