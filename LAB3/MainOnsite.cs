using System;
using System.IO;
using System.Threading.Tasks;

class MainOnSiteProgram
{
    static async Task Main(string[] args)
    {
        string directory = "MatrixResults";

        // Create 50 matrices of size 500x100 and 50 matrices of size 100x500
        Matrix[] aMatrices = new Matrix[50];
        Matrix[] bMatrices = new Matrix[50];
        for (int i = 0; i < 50; i++)
        {
            aMatrices[i] = Matrix.Zero(500, 100);
            bMatrices[i] = Matrix.Zero(100, 500);
        }

        // Save tasks for actions performed via Task.Run
        Task[] tasks = new Task[4];

        tasks[0] = Task.Run(async () =>
        {
            for (int i = 0; i < 50; i++)
            {
                string filePath = Path.Combine(directory, $"Product_{i}.tsv");
                await MatrixIO.WriteToFileAsync(directory, $"MatrixA_{i}.txt", aMatrices[i], async (matrix, stream) => await MatrixIO.WriteTextAsync(matrix, stream));
                Console.WriteLine($"Product_{i}.tsv written");
            }
        });

        tasks[1] = Task.Run(async () =>
        {
            for (int i = 0; i < 50; i++)
            {
                string filePath = Path.Combine(directory, $"Product_{i + 50}.tsv");
                await MatrixIO.WriteToFileAsync(directory, $"MatrixA_{i}.txt", aMatrices[i], async (matrix, stream) => await MatrixIO.WriteTextAsync(matrix, stream));
                Console.WriteLine($"Product_{i + 50}.tsv written");
            }
        });

        tasks[2] = Task.Run(async () =>
        {
            for (int i = 0; i < 50; i++)
            {
                string filePath = Path.Combine(directory, $"ScalarProduct_{i}.tsv");
                await MatrixIO.WriteToFileAsync(directory, $"MatrixA_{i}.txt", aMatrices[i], async (matrix, stream) => await MatrixIO.WriteTextAsync(matrix, stream));
                Console.WriteLine($"ScalarProduct_{i}.tsv written");
            }
        });

        tasks[3] = Task.Run(async () =>
        {
            for (int i = 0; i < 50; i++)
            {
                string filePath = Path.Combine(directory, $"ScalarProduct_{i + 50}.tsv");
                await MatrixIO.WriteToFileAsync(directory, $"MatrixA_{i}.txt", aMatrices[i], async (matrix, stream) => await MatrixIO.WriteTextAsync(matrix, stream));
                Console.WriteLine($"ScalarProduct_{i + 50}.tsv written");
            }
        });

        await Task.WhenAll(tasks);

        // Create three directories
        string[] formatDirectories = { "Binary Format", "String Format", "JSON Format" };
        foreach (string formatDirectory in formatDirectories)
        {
            Directory.CreateDirectory(Path.Combine(directory, formatDirectory));
        }

        // Save tasks for actions performed via Task.Run
        Task[] fileTasks = new Task[4];

        fileTasks[0] = Task.Run(async () =>
        {
            for (int i = 0; i < 50; i++)
            {
                string filePath = Path.Combine(directory, "String Format", $"MatrixA_{i}.txt");
                await MatrixIO.WriteToFileAsync(directory, $"MatrixA_{i}.txt", aMatrices[i], async (matrix, stream) => await MatrixIO.WriteTextAsync(matrix, stream));
                Console.WriteLine($"MatrixA_{i}.txt written in String Format");
            }
        });

        fileTasks[1] = Task.Run(async () =>
        {
            for (int i = 0; i < 50; i++)
            {
                string filePath = Path.Combine(directory, "String Format", $"MatrixB_{i}.txt");
                await MatrixIO.WriteToFileAsync(directory, $"MatrixA_{i}.txt", aMatrices[i], async (matrix, stream) => await MatrixIO.WriteTextAsync(matrix, stream));
                Console.WriteLine($"MatrixB_{i}.txt written in String Format");
            }
        });

        fileTasks[2] = Task.Run(async () =>
        {
            for (int i = 0; i < 50; i++)
            {
                string filePath = Path.Combine(directory, "JSON Format", $"MatrixA_{i}.json");
                await MatrixIO.WriteToFileAsync(Path.Combine(directory, "JSON Format"), $"MatrixA_{i}.json", aMatrices[i], MatrixIO.WriteJsonAsync);
                Console.WriteLine($"MatrixA_{i}.json written in JSON Format");
            }
        });

        fileTasks[3] = Task.Run(async () =>
        {
            for (int i = 0; i < 50; i++)
            {
                string filePath = Path.Combine(directory, "JSON Format", $"MatrixB_{i}.json");
                await MatrixIO.WriteToFileAsync(Path.Combine(directory, "JSON Format"), $"MatrixB_{i}.json", bMatrices[i], MatrixIO.WriteJsonAsync);
                Console.WriteLine($"MatrixB_{i}.json written in JSON Format");
            }
        });

        await Task.WhenAll(fileTasks);

        // Read matrix arrays from files
        Matrix[] readAMatrices = new Matrix[50];
        Matrix[] readBMatrices = new Matrix[50];
        Task<Matrix[]>[] readTasks = new Task<Matrix[]>[2];

        readTasks[0] = Task.Run(async () =>
        {
            for (int i = 0; i < 50; i++)
            {
                string filePath = Path.Combine(directory, "String Format", $"MatrixA_{i}.txt");
                await MatrixIO.ReadFromFileAsync(filePath, async stream => await MatrixIO.ReadTextAsync(stream));
                Console.WriteLine($"MatrixA_{i}.txt read from String Format");
            }
            return readAMatrices;
        });

        readTasks[1] = Task.Run(async () =>
        {
            for (int i = 0; i < 50; i++)
            {
                string filePath = Path.Combine(directory, "String Format", $"MatrixB_{i}.txt");
                await MatrixIO.ReadFromFileAsync(filePath, async stream => await MatrixIO.ReadTextAsync(stream));
                Console.WriteLine($"MatrixB_{i}.txt read from String Format");
            }
            return readBMatrices;
        });

        await Task.WhenAll(readTasks);

        // Compare matrix arrays
        bool[] comparisonResults = new bool[2];
        Task[] comparisonTasks = new Task[2];

        comparisonTasks[0] = Task.Run(() =>
        {
            comparisonResults[0] = CompareMatrixArrays(aMatrices, readAMatrices);
            Console.WriteLine($"Comparison result for Matrix A arrays: {comparisonResults[0]}");
        });

        comparisonTasks[1] = Task.Run(() =>
        {
            comparisonResults[1] = CompareMatrixArrays(bMatrices, readBMatrices);
            Console.WriteLine($"Comparison result for Matrix B arrays: {comparisonResults[1]}");
        });

        await Task.WhenAll(comparisonTasks);

        // Delete the directory
        Directory.Delete(directory, true);
        Console.WriteLine("MatrixResults directory deleted.");
    }

    static bool CompareMatrixArrays(Matrix[] array1, Matrix[] array2)
    {
        if (array1.Length != array2.Length)
            return false;

        for (int i = 0; i < array1.Length; i++)
        {
            if (!array1[i].Equals(array2[i]))
                return false;
        }

        return true;
    }
}
