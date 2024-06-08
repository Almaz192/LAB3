using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public static class MatrixIO
{
    // Asynchronous methods
    public static async Task WriteTextAsync(Matrix matrix, Stream stream, string sep = ", ")
    {
        using (StreamWriter writer = new StreamWriter(stream))
        {
            await writer.WriteLineAsync($"{matrix.Rows} {matrix.Columns}");
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    await writer.WriteAsync(matrix[i, j].ToString());
                    if (j < matrix.Columns - 1)
                        await writer.WriteAsync(sep);
                }
                await writer.WriteLineAsync();
            }
        }
    }

    public static async Task<Matrix> ReadTextAsync(Stream stream, string sep = ", ")
    {
        using (StreamReader reader = new StreamReader(stream))
        {
            string[] dimensions = (await reader.ReadLineAsync()).Split(' ');
            int rows = int.Parse(dimensions[0]);
            int cols = int.Parse(dimensions[1]);

            double[,] values = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                string[] line = (await reader.ReadLineAsync()).Split(new string[] { sep }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < cols; j++)
                {
                    values[i, j] = double.Parse(line[j]);
                }
            }

            return new Matrix(values);
        }
    }

    public static async Task WriteBinaryAsync(Matrix matrix, Stream stream)
    {
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            writer.Write(matrix.Rows);
            writer.Write(matrix.Columns);
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    writer.Write(matrix[i, j]);
                }
            }
        }
    }

    public static async Task<Matrix> ReadBinaryAsync(Stream stream)
    {
        using (BinaryReader reader = new BinaryReader(stream))
        {
            int rows = reader.ReadInt32();
            int cols = reader.ReadInt32();

            double[,] values = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    values[i, j] = reader.ReadDouble();
                }
            }

            return new Matrix(values);
        }
    }

    public static async Task WriteJsonAsync(Matrix matrix, Stream stream)
    {
        double[][] values = new double[matrix.Rows][];
        for (int i = 0; i < matrix.Rows; i++)
        {
            values[i] = new double[matrix.Columns];
            for (int j = 0; j < matrix.Columns; j++)
            {
                values[i][j] = matrix[i, j];
            }
        }

        await JsonSerializer.SerializeAsync(stream, values);
    }

    public static async Task<Matrix> ReadJsonAsync(Stream stream)
    {
        double[][] values = await JsonSerializer.DeserializeAsync<double[][]>(stream);

        int rows = values.Length;
        int cols = values[0].Length;

        double[,] matrixValues = new double[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrixValues[i, j] = values[i][j];
            }
        }

        return new Matrix(matrixValues);
    }

    // Synchronous methods
    public static void WriteText(Matrix matrix, Stream stream, string sep = ", ")
    {
        WriteTextAsync(matrix, stream, sep).GetAwaiter().GetResult();
    }

    public static Matrix ReadText(Stream stream, string sep = ", ")
    {
        return ReadTextAsync(stream, sep).GetAwaiter().GetResult();
    }

    public static void WriteBinary(Matrix matrix, Stream stream)
    {
        WriteBinaryAsync(matrix, stream).GetAwaiter().GetResult();
    }

    public static Matrix ReadBinary(Stream stream)
    {
        return ReadBinaryAsync(stream).GetAwaiter().GetResult();
    }

    public static void WriteJson(Matrix matrix, Stream stream)
    {
        WriteJsonAsync(matrix, stream).GetAwaiter().GetResult();
    }

    public static Matrix ReadJson(Stream stream)
    {
        return ReadJsonAsync(stream).GetAwaiter().GetResult();
    }

    // Methods to write and read from files
    public static void WriteToFile(string directory, string fileName, Matrix matrix, Action<Matrix, Stream> writeMethod)
    {
        using (FileStream fileStream = File.Create(Path.Combine(directory, fileName)))
        {
            writeMethod(matrix, fileStream);
        }
    }

    public static Matrix ReadFromFile(string filePath, Func<Stream, Matrix> readMethod)
    {
        using (FileStream fileStream = File.OpenRead(filePath))
        {
            return readMethod(fileStream);
        }
    }

    public static async Task WriteToFileAsync(string directory, string fileName, Matrix matrix, Func<Matrix, Stream, Task> writeMethod)
    {
        using (FileStream fileStream = File.Create(Path.Combine(directory, fileName)))
        {
            await writeMethod(matrix, fileStream);
        }
    }

    public static async Task<Matrix> ReadFromFileAsync(string filePath, Func<Stream, Task<Matrix>> readMethod)
    {
        using (FileStream fileStream = File.OpenRead(filePath))
        {
            return await readMethod(fileStream);
        }
    }
}
