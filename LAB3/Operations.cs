using System;
using System.Threading.Tasks;

public static class MatrixOperations
{
    public static Matrix Transpose(Matrix matrix)
    {
        int rows = matrix.Columns;
        int cols = matrix.Rows;
        double[,] transposed = new double[rows, cols];

        Parallel.For(0, rows, i =>
        {
            for (int j = 0; j < cols; j++)
            {
                transposed[i, j] = matrix[j, i];
            }
        });

        return new Matrix(transposed);
    }

    public static Matrix Add(Matrix a, Matrix b)
    {
        ValidateSameDimensions(a, b);
        double[,] result = new double[a.Rows, a.Columns];
        Parallel.For(0, a.Rows, i =>
        {
            for (int j = 0; j < a.Columns; j++)
            {
                result[i, j] = a[i, j] + b[i, j];
            }
        });

        return new Matrix(result);
    }

    public static Matrix Subtract(Matrix a, Matrix b)
    {
        ValidateSameDimensions(a, b);
        double[,] result = new double[a.Rows, a.Columns];
        Parallel.For(0, a.Rows, i =>
        {
            for (int j = 0; j < a.Columns; j++)
            {
                result[i, j] = a[i, j] - b[i, j];
            }
        });

        return new Matrix(result);
    }

    public static Matrix Multiply(Matrix a, double scalar)
    {
        double[,] result = new double[a.Rows, a.Columns];
        Parallel.For(0, a.Rows, i =>
        {
            for (int j = 0; j < a.Columns; j++)
            {
                result[i, j] = a[i, j] * scalar;
            }
        });

        return new Matrix(result);
    }

    public static Matrix Multiply(Matrix a, Matrix b)
    {
        if (a.Columns != b.Rows)
            throw new ArgumentException("Matrix dimensions do not match");

        double[,] result = new double[a.Rows, b.Columns];
        Parallel.For(0, a.Rows, i =>
        {
            for (int j = 0; j < b.Columns; j++)
            {
                double sum = 0;
                for (int k = 0; k < a.Columns; k++)
                {
                    sum += a[i, k] * b[k, j];
                }
                result[i, j] = sum;
            }
        });

        return new Matrix(result);
    }

    public static Matrix Negate(Matrix matrix)
    {
        return Multiply(matrix, -1);
    }

    public static bool Equals(Matrix a, Matrix b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null || a.Rows != b.Rows || a.Columns != b.Columns) return false;

        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < a.Columns; j++)
            {
                if (a[i, j] != b[i, j]) return false;
            }
        }

        return true;
    }

    public static string MatrixToString(Matrix matrix)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Columns; j++)
            {
                sb.Append(matrix[i, j]).Append(j == matrix.Columns - 1 ? "" : ", ");
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public static int ComputeHashCode(Matrix matrix)
    {
        unchecked
        {
            int hash = 17;
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Columns; j++)
                {
                    hash = hash * 31 + matrix[i, j].GetHashCode();
                }
            }
            return hash;
        }
    }

    private static void ValidateSameDimensions(Matrix a, Matrix b)
    {
        if (a.Rows != b.Rows || a.Columns != b.Columns)
            throw new ArgumentException("Matrix dimensions do not match");
    }
}
