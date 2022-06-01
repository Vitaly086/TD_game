using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField] private Transform _ground;
    [SerializeField] private GameTile _tilePrefab;

    private Vector2Int _size;

    private GameTile[] _tiles;
    private GameTailContentFactory _contentFactory;

    private Queue<GameTile> _searchFrontier = new();

    public void Initialize(Vector2Int size, GameTailContentFactory contentFactory)
    {
        _size = size;
        _ground.localScale = new Vector3(size.x, size.y, 1f);

        InstantiateTileOnBoard(size, contentFactory);

        ToggleDestination(_tiles[_tiles.Length / 2]);
    }

    public bool FindPath()
    {
        foreach (var tile in _tiles)
        {
            if (tile.Content.Type == GameTileContentType.Destination)
            {
                tile.BecomeDestination();
                _searchFrontier.Enqueue(tile);
            }
            else
            {
                tile.ClearPath();
            }
        }

        if (_searchFrontier.Count == 0)
        {
            return false;
        }

        while (_searchFrontier.Count > 0)
        {
            GameTile tile = _searchFrontier.Dequeue();

            if (tile != null)
            {
                if (tile.IsAlternative)
                {
                    _searchFrontier.Enqueue(tile.GrowPassNorth());
                    _searchFrontier.Enqueue(tile.GrowPassEast());
                    _searchFrontier.Enqueue(tile.GrowPassSouth());
                    _searchFrontier.Enqueue(tile.GrowPassWest());
                }
                else
                {
                    _searchFrontier.Enqueue(tile.GrowPassWest());
                    _searchFrontier.Enqueue(tile.GrowPassSouth());
                    _searchFrontier.Enqueue(tile.GrowPassEast());
                    _searchFrontier.Enqueue(tile.GrowPassNorth());
                }
            }
        }

        foreach (var tile in _tiles)
        {
            tile.ShowPath();
        }

        return true;
    }

    public void ToggleDestination(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Destination)
        {
            tile.Content = _contentFactory.Get(GameTileContentType.Empty);
            if (!FindPath())
            {
                tile.Content = _contentFactory.Get(GameTileContentType.Destination);
                FindPath();
            }
        }
        else
        {
            tile.Content = _contentFactory.Get(GameTileContentType.Destination);
            FindPath();
        }
    }

    public GameTile GetTile(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            int x = (int) (hit.point.x + _size.x * 0.5f);
            int y = (int) (hit.point.z + _size.y * 0.5f);

            if (x >= 0 && x < _size.x && y >= 0 && y < _size.y)
            {
                return _tiles[x + y * _size.x];
            }
        }

        return null;
    }

    private void InstantiateTileOnBoard(Vector2Int size, GameTailContentFactory contentFactory)
    {
        Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);

        _tiles = new GameTile[size.x * size.y];
        _contentFactory = contentFactory;
        for (int i = 0, y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++, i++)
            {
                _tiles[i] = Instantiate(_tilePrefab);
                var tile = _tiles[i];
                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(x - offset.x, 0f, y - offset.y);

                if (x > 0)
                {
                    GameTile.MakeEastWestNeighbors(tile, _tiles[i - 1]);
                }

                if (y > 0)
                {
                    GameTile.MakeNorthSouthNeighbors(tile, _tiles[i - size.x]);
                }

                tile.IsAlternative = (x & 1) == 0;
                if ((y & 1) == 0)
                {
                    tile.IsAlternative = !tile.IsAlternative;
                }

                tile.Content = _contentFactory.Get(GameTileContentType.Empty);
            }
        }
    }
}