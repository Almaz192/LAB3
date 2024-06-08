using System;
using System.Linq;

public class Matrix
{
    private double[,] values;
    private int? hashCode = null;

    public Matrix(double[,] initialValues)
    {
        values = (double[,])initialValues.Clone();
    }

    public static Matrix Zero(int rows, int cols)
    {
        return new Matrix(new double[rows, cols]);
    }

    public static Matrix Zero(int size)
    {
        return Zero(size, size);
    }

    public static Matrix Identity(int size)
    {
        var matrix = Zero(size);
        for (int i = 0; i < size; i++)
        {
            matrix.values[i, i] = 1.0;
        }
        return matrix;
    }
    public int Rows => values.GetLength(0);
    public int Columns => values.GetLength(1);

    public double this[int row, int col] => values[row, col];

    public Matrix Transpose() => MatrixOperations.Transpose(this);

    public override string ToString() => MatrixOperations.MatrixToString(this);

    public override bool Equals(object obj) => MatrixOperations.Equals(this, obj as Matrix);

    public override int GetHashCode()
    {
        if (!hashCode.HasValue)
        {
            hashCode = MatrixOperations.ComputeHashCode(this);
        }
        return hashCode.Value;
    }

    // Operators
    public static Matrix operator +(Matrix a, Matrix b) => MatrixOperations.Add(a, b);
    public static Matrix operator -(Matrix a, Matrix b) => MatrixOperations.Subtract(a, b);
    public static Matrix operator *(Matrix a, Matrix b) => MatrixOperations.Multiply(a, b);
    public static Matrix operator *(Matrix a, double scalar) => MatrixOperations.Multiply(a, scalar);
    public static Matrix operator *(double scalar, Matrix a) => a * scalar;
    public static Matrix operator -(Matrix a) => MatrixOperations.Negate(a);
    public static Matrix operator ~(Matrix a) => a.Transpose();
}
