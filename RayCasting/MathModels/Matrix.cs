using System.Numerics;

namespace RayCasting.MathModels;

internal class Matrix
{
    public float[,] Data { get; private set; }
    public int RowCount
    {
        get => Data.GetLength(0);
    }
    public int ColumnCount
    {
        get => Data.GetLength(1);
    }

    public Matrix(int rowCount, int columnCount)
    {
        Data = new float[rowCount, columnCount];
    }

    public static Matrix CreateVertical(Vector3 vector)
    {
        var result = new Matrix(3, 1);
        result.Data = new float[,]{ { vector.X }, { vector.Y }, { vector.Z } };
        return result;
    }

    public static Matrix CreateHorizontal(Vector3 vector)
    {
        var result = new Matrix(1, 3);
        result.Data = new float[,] { { vector.X, vector.Y, vector.Z } };
        return result;
    }
}
