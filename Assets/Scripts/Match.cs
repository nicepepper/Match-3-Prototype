using Unity.Mathematics;

[System.Serializable]
public struct Match
{
    public int2 Coordinates;
    public int Length;
    public bool IsHorizontal;

    public Match (int x, int y, int length, bool isHorizontal)
    {
        Coordinates.x = x;
        Coordinates.y = y;
        Length = length;
        IsHorizontal = isHorizontal;
    }
}