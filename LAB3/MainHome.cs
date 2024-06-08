using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Create a matrix with random values
        Matrix randomMatrix = CreateRandomMatrix(3, 3);
        Console.WriteLine("Random Matrix:");
        Console.WriteLine(randomMatrix);

        // Multiply arrays of matrices alternately
        Matrix firstArray = CreateRandomMatrix(2, 2);
        Matrix secondArray = CreateRandomMatrix(2, 2);
        try
        {
            Matrix result = MultiplyArraysAlternately(firstArray, secondArray);
            Console.WriteLine("Result of multiplying arrays alternately:");
            Console.WriteLine(result);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }

        // Calculate the scalar product of two matrix arrays
        try
        {
            Matrix scalarProduct = CalculateScalarProduct(firstArray, secondArray);
            Console.WriteLine("Scalar product of two matrix arrays:");
            Console.WriteLine(scalarProduct);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }

        // Write matrix array to directory
        Matrix[] matrixArray = { randomMatrix, firstArray, secondArray };
        string directory = "MatrixDirectory";
        string prefix = "matrix";
        string extension = ".txt";
        await WriteMatrixArrayToDirectory(matrixArray, directory, prefix, extension, async (matrix, stream) => await MatrixIO.WriteTextAsync(matrix, stream));

        // Read matrix array from directory
        Matrix[] readMatrixArray = await ReadMatrixArrayFromDirectory(directory, prefix, extension, async stream => await MatrixIO.ReadTextAsync(stream));
        Console.WriteLine("Read matrix array from directory:");
        foreach (var matrix in readMatrixArray)
        {
            Console.WriteLine(matrix);
        }

        // Compare two matrix arrays for equality
        bool isEqual = CompareMatrixArrays(matrixArray, readMatrixArray);
        Console.WriteLine($"Matrix arrays are equal: {isEqual}");
    }

    static Matrix CreateRandomMatrix(int rows, int columns)
    {
        Random rand = new Random();
        double[,] values = new double[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                values[i, j] = rand.NextDouble() * (10 - (-10)) + (-10);
            }
        }
        return new Matrix(values);
    }

    static Matrix MultiplyArraysAlternately(Matrix firstArray, Matrix secondArray)
    {
        if (firstArray.Rows != secondArray.Rows || firstArray.Columns != secondArray.Columns)
            throw new ArgumentException("Matrix dimensions do not match");

        Matrix result = firstArray;
        for (int i = 0; i < secondArray.Rows; i++)
        {
            result *= secondArray;
            result *= firstArray;
        }

        return result;
    }

    static Matrix CalculateScalarProduct(Matrix firstArray, Matrix secondArray)
    {
        if (firstArray.Rows != secondArray.Rows || firstArray.Columns != secondArray.Columns)
            throw new ArgumentException("Matrix dimensions do not match");

        Matrix result = firstArray * secondArray;
        for (int i = 1; i < secondArray.Rows; i++)
        {
            result += (firstArray * secondArray);
        }

        return result;
    }

    static async Task WriteMatrixArrayToDirectory(Matrix[] matrixArray, string directory, string prefix, string extension, Action<Matrix, Stream> writeMethod)
    {
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        for (int i = 0; i < matrixArray.Length; i++)
        {
            string fileName = $"{prefix}{i}{extension}";
            using (FileStream fileStream = File.Create(Path.Combine(directory, fileName)))
            {
                writeMethod(matrixArray[i], fileStream);
            }

            if ((i + 1) % 10 == 0)
            {
                Console.WriteLine($"Matrix {i + 1} written to directory.");
            }
        }
    }

    static async Task<Matrix[]> ReadMatrixArrayFromDirectory(string directory, string prefix, string extension, Func<Stream, Task<Matrix>> readMethod)
    {
        string[] filePaths = Directory.GetFiles(directory, $"{prefix}*{extension}");
        Matrix[] matrixArray = new Matrix[filePaths.Length];
        for (int i = 0; i < filePaths.Length; i++)
        {
            using (FileStream fileStream = File.OpenRead(filePaths[i]))
            {
                matrixArray[i] = await readMethod(fileStream);
            }
        }
        return matrixArray;
    }

    static bool CompareMatrixArrays(Matrix[] firstArray, Matrix[] secondArray)
    {
        if (firstArray.Length != secondArray.Length)
            return false;

        for (int i = 0; i < firstArray.Length; i++)
        {
            if (!firstArray[i].Equals(secondArray[i]))
                return false;
        }

        return true;
    }
}
