using Unity.Mathematics;

[System.Serializable]
public struct TileDrop
{
	public int2 Coordinates;
	public int FromY;

	public TileDrop (int x, int y, int distance)
	{
		Coordinates.x = x;
		Coordinates.y = y;
		FromY = y + distance;
	}
}
