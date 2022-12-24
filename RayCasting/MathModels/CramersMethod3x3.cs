namespace RayCasting.MathModels;

internal class CramersMethod3x3Result
{
    public float[]? Values { get; set; }
    public bool Success { get; set; }
}

internal class CramersMethod3x3
{
    private float[,] _matrix;
    private float[] _buffer;

    public CramersMethod3x3(float[,] matrix, float[] freeMembers)
    {
        _matrix = matrix;
        _buffer = freeMembers;
    }

    public CramersMethod3x3Result Solve()
    {
        var delta = GetDeterminant();
        if (delta == 0)
        {
            return new CramersMethod3x3Result
            {
                Success = false
            };
        }

        var deltaX = GetDelta(0);
        var deltaY = GetDelta(1);
        var deltaZ = GetDelta(2);

        return new CramersMethod3x3Result
        {
            Success = true,
            Values = new float[] { deltaX / delta, deltaY / delta, deltaZ / delta }
        };
    }

    private float GetDelta(int columnIndex)
    {
        SwapColumnAndBuffer(columnIndex);
        var result = GetDeterminant();
        SwapColumnAndBuffer(columnIndex);

        return result;
    }

    private void SwapColumnAndBuffer(int columnIndex)
    {
        for (int row = 0; row < 3; row++)
        {
            Swap(ref _matrix[row, columnIndex], ref _buffer[row]);
        }
    }

    private static void Swap(ref float a, ref float b)
    {
        var c = a;
        a = b;
        b = c;
    }

    private float GetDeterminant()
    {
        return
            _matrix[0, 0] * _matrix[1, 1] * _matrix[2, 2]
            + _matrix[0, 1] * _matrix[1, 2] * _matrix[2, 0]
            + _matrix[0, 2] * _matrix[1, 0] * _matrix[2, 1]
            - _matrix[0, 2] * _matrix[1, 1] * _matrix[2, 0]
            - _matrix[0, 1] * _matrix[1, 0] * _matrix[2, 2]
            - _matrix[0, 0] * _matrix[1, 2] * _matrix[2, 1];
    }
}
