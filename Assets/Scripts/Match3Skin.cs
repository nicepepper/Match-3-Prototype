using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

public class Match3Skin : MonoBehaviour
{
	[SerializeField] private Tile[] _tilePrefabs;
	[SerializeField] private FloatingScore _floatingScorePrefab;
	[SerializeField] private TextMeshPro _gameOverText;
	[SerializeField] private TextMeshPro _totalScoreText;
	[SerializeField] private Match3Game _game;
	[SerializeField, Range(0.1f, 1f)] private float _dragThreshold = 0.5f;
	[SerializeField, Range(0.1f, 20f)] private float _dropSpeed = 8f;
	[SerializeField, Range(0f, 10f)] private float _newDropOffset = 2f;
	[SerializeField] private TileSwapper _tileSwapper;

	[SerializeField] private float2 _tileOffset;
	[SerializeField] private float _betweenTiles = 0.65f;
	
	private float _busyDuration;
	private Grid2D<Tile> _tiles;
	private float _floatingScoreZ;

	public bool IsBusy => _busyDuration > 0f;
	public bool IsPlaying => IsBusy || _game.PossibleMove.IsValid;

	public void StartNewGame () {
		_busyDuration = 0f;
		_totalScoreText.SetText("0");
		_gameOverText.gameObject.SetActive(false);
		_game.StartNewGame();
		_tileOffset.x = -0.319f * (_game.Size.x - 1);
		_tileOffset.y = -0.504f * (_game.Size.y - 1);

		if (_tiles.IsUndefined)
		{
			_tiles = new Grid2D<Tile>(_game.Size);
		}
		else
		{
			for (int y = 0; y < _tiles.Size.y; y++)
			{
				for (int x = 0; x < _tiles.Size.x; x++)
				{
					_tiles[x, y].Despawn();
					_tiles[x, y] = null;
				}
			}
		}

		for (int y = 0; y < _tiles.Size.y; y++)
		{
			for (int x = 0; x < _tiles.Size.x; x++)
			{
				_tiles[x, y] = SpawnTile(_game[x, y], x, y);
			}
		}
	}

	public void DoWork () {
		if (_busyDuration > 0f)
		{
			_tileSwapper.Update();
			_busyDuration -= Time.deltaTime;
			if (_busyDuration > 0f)
			{
				return;
			}
		}

		if (_game.HasMatches)
		{
			ProcessMatches();
		}
		else if (_game.NeedsFilling)
		{
			DropTiles();
		}
		else if (!IsPlaying)
		{
			_gameOverText.gameObject.SetActive(true);
		}
	}

	public void DoAutomaticMove()
	{
		DoMove(_game.PossibleMove);
	}

	public bool EvaluateDrag (Vector3 start, Vector3 end)
	{
		float2 a = ScreenToTileSpace(start);
		float2 b = ScreenToTileSpace(end);
		
		var move = new Move(
			(int2)(a / _betweenTiles), (b - a) switch
			{
				var d when d.x > _dragThreshold => MoveDirection.Right,
				var d when d.x < -_dragThreshold => MoveDirection.Left,
				var d when d.y > _dragThreshold => MoveDirection.Up,
				var d when d.y < -_dragThreshold => MoveDirection.Down,
				_ => MoveDirection.None
			}
		);
		
		if (move.IsValid && _tiles.AreValidCoordinates(move.From) && _tiles.AreValidCoordinates(move.To))
		{
			DoMove(move);
			return false;
		}
		return true;
	}

	private void DropTiles ()
	{
		_game.DropTiles();
		
		for (int i = 0; i < _game.DroppedTiles.Count; i++)
		{
			TileDrop drop = _game.DroppedTiles[i];
			Tile tile;
			if (drop.FromY < _tiles.Size.y)
			{
				tile = _tiles[drop.Coordinates.x, drop.FromY];
			}
			else
			{
				tile = SpawnTile(_game[drop.Coordinates], drop.Coordinates.x, drop.FromY + _newDropOffset);
			}
			_tiles[drop.Coordinates] = tile;
			_busyDuration = Mathf.Max(
				tile.Fall(drop.Coordinates.y * _betweenTiles + _tileOffset.y, _dropSpeed), 
				_busyDuration
				);
		}
	}

	private void ProcessMatches ()
	{
		_game.ProcessMatches();

		for (int i = 0; i < _game.ClearedTileCoordinates.Count; i++)
		{
			int2 c = _game.ClearedTileCoordinates[i];
			_busyDuration = Mathf.Max(_tiles[c].Disappear(), _busyDuration);
			_tiles[c] = null;
		}

		_totalScoreText.SetText("{0}", _game.TotalScore);

		for (int i = 0; i < _game.Scores.Count; i++)
		{
			SingleScore score = _game.Scores[i];
			_floatingScorePrefab.Show(
				new Vector3(
					score.Position.x * _betweenTiles + _tileOffset.x,
					score.Position.y * _betweenTiles + _tileOffset.y,
					_floatingScoreZ
				),
				score.Value
			);
			_floatingScoreZ = _floatingScoreZ <= -0.02f ? 0f : _floatingScoreZ - 0.001f;
		}
	}

	private void DoMove (Move move)
	{
		bool succcess = _game.TryMove(move);
		Tile first = _tiles[move.From];
		Tile second = _tiles[move.To];
		_busyDuration = _tileSwapper.Swap(first, second, !succcess);
		if (succcess)
		{
			_tiles[move.From] = second;
			_tiles[move.To] = first;
		}
	}

	private float2 ScreenToTileSpace (Vector3 screenPosition)
	{
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);
		Vector3 p = ray.origin - ray.direction * (ray.origin.z / ray.direction.z);
		return float2(p.x - _tileOffset.x + 0.3f, p.y - _tileOffset.y + 0.3f);
	}

	private Tile SpawnTile(TileState t, float x, float y)
	{
		return _tilePrefabs[(int)t - 1].Spawn(new Vector3(x * _betweenTiles + _tileOffset.x, y * _betweenTiles + _tileOffset.y));
	}
}
