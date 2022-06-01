using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] private Vector2Int _boardSize;

    [SerializeField] private GameBoard _board;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameTailContentFactory _contentFactory;

    private Ray _touchRay => _mainCamera.ScreenPointToRay(Input.mousePosition);

    void Start()
    {
        _board.Initialize(_boardSize, _contentFactory);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleTouch();
        }
    }

    private void HandleTouch()
    {
        GameTile tile = _board.GetTile(_touchRay);
        if (tile != null)
        {
            _board.ToggleDestination(tile);
        }
    }
}