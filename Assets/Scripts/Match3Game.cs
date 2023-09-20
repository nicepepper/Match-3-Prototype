﻿using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using static Unity.Mathematics.math;

public class Match3Game : MonoBehaviour
{
	[SerializeField] private int2 _size = 7;
	
	private Grid2D<TileState> _grid;
	private List<Match> _matches;
	private const int MinTileState = 1;
	private const int MaxTileState = 8;

	public TileState this[int2 c] => _grid[c];
	public TileState this[int x, int y] => _grid[x, y];
	public int2 Size => _size;

	public List<int2> ClearedTileCoordinates { get; private set; }
	public List<TileDrop> DroppedTiles { get; private set; }
	public List<SingleScore> Scores { get; private set; }
	public bool NeedsFilling { get; private set; }
	public Move PossibleMove { get; private set; }
	public int TotalScore { get; private set; }
	public bool HasMatches => _matches.Count > 0;

	public void StartNewGame ()
	{
		TotalScore = 0;
		if (_grid.IsUndefined)
		{
			_grid = new Grid2D<TileState>(_size);
			_matches = new List<Match>();
			ClearedTileCoordinates = new List<int2>();
			DroppedTiles = new List<TileDrop>();
			Scores = new List<SingleScore>();
		}
		
		do
		{
			FillGrid();
			PossibleMove = Move.FindMove(this);
		}
		while (!PossibleMove.IsValid);
	}

	public bool TryMove (Move move)
	{
		_grid.Swap(move.From, move.To);
		if (FindMatches())
		{
			return true;
		}
		_grid.Swap(move.From, move.To);
		return false;
	}

	public void ProcessMatches ()
	{
		ClearedTileCoordinates.Clear();
		Scores.Clear();

		for (int m = 0; m < _matches.Count; m++)
		{
			Match match = _matches[m];
			int2 step = match.IsHorizontal ? int2(1, 0) : int2(0, 1);
			int2 c = match.Coordinates;
			for (int i = 0; i < match.Length; c += step, i++)
			{
				if (_grid[c] != TileState.None)
				{
					_grid[c] = TileState.None;
					ClearedTileCoordinates.Add(c);
				}
			}

			var score = new SingleScore
			{
				Position = match.Coordinates  + (float2)step * (match.Length - 1) * 0.5f,
				Value = match.Length * (match.Length - 2)
			};
			Scores.Add(score);
			TotalScore += score.Value;
		}

		_matches.Clear();
		NeedsFilling = true;
	}

	public void DropTiles ()
	{
		DroppedTiles.Clear();

		for (int x = 0; x < _size.x; x++)
		{
			int holeCount = 0;
			for (int y = 0; y < _size.y; y++)
			{
				if (_grid[x, y] == TileState.None)
				{
					holeCount++;
				}
				else if (holeCount > 0)
				{
					_grid[x, y - holeCount] = _grid[x, y];
					DroppedTiles.Add(new TileDrop(x, y - holeCount, holeCount));
				}
			}

			for (int h = 1; h <= holeCount; h++)
			{
				_grid[x, _size.y - h] = (TileState)Random.Range(MinTileState, MaxTileState);
				DroppedTiles.Add(new TileDrop(x, _size.y - h, holeCount));
			}
		}

		NeedsFilling = false;
		if (!FindMatches())
		{
			PossibleMove = Move.FindMove(this);
		}
	}

	private void FillGrid ()
	{
		for (int y = 0; y < _size.y; y++)
		{
			for (int x = 0; x < _size.x; x++)
			{
				TileState a = TileState.None, b = TileState.None;
				int potentialMatchCount = 0;
				if (x > 1)
				{
					a = _grid[x - 1, y];
					if (a == _grid[x - 2, y])
					{
						potentialMatchCount = 1;
					}
				}
				if (y > 1)
				{
					b = _grid[x, y - 1];
					if (b == _grid[x, y - 2])
					{
						potentialMatchCount += 1;
						if (potentialMatchCount == 1)
						{
							a = b;
						}
						else if (b < a)
						{
							(a, b) = (b, a);
						}
					}
				}

				TileState t = (TileState)Random.Range(MinTileState, MaxTileState - potentialMatchCount);
				if (potentialMatchCount > 0 && t >= a)
				{
					t++;
				}
				if (potentialMatchCount == 2 && t >= b)
				{
					t++;
				}
				_grid[x, y] = t;
			}
		}
	}

	private bool FindMatches ()
	{
		_matches.Clear();

		for (int y = 0; y < _size.y; y++)
		{
			TileState start = _grid[0, y];
			int length = 1;
			for (int x = 1; x < _size.x; x++)
			{
				TileState t = _grid[x, y];
				if (t == start)
				{
					length++;
				}
				else
				{
					if (length >= 3)
					{
						_matches.Add(new Match(x - length, y, length, true));
					}
					start = t;
					length = 1;
				}
			}
			if (length >= 3)
			{
				_matches.Add(new Match(_size.x - length, y, length, true));
			}
		}

		for (int x = 0; x < _size.x; x++)
		{
			TileState start = _grid[x, 0];
			int length = 1;
			for (int y = 1; y < _size.y; y++)
			{
				TileState t = _grid[x, y];
				if (t == start)
				{
					length++;
				}
				else
				{
					if (length >= 3)
					{
						_matches.Add(new Match(x, y - length, length, false));
					}
					start = t;
					length = 1;
				}
			}
			if (length >= 3)
			{
				_matches.Add(new Match(x, _size.y - length, length, false));
			}
		}

		return HasMatches;
	}
}
