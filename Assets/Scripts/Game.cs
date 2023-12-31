﻿using System;
using UnityEngine;
using Zenject;

public class Game : MonoBehaviour
{
    [SerializeField] private Match3Skin _match3;
    [SerializeField] private bool _automaticPlay;

    private Vector3 _dragStart;
    private bool _isDragging;
    
    /*private IInputService _inputService;
    private Vector3 _inputPosition;
    private bool _isInput = false;

    [Inject]
    public void Construct(IInputService inputService)
    {
        _inputService = inputService;
        _inputService.OnClick += OnClick;
    }*/

    private void Awake ()
    {
        _match3.StartNewGame();
    }

    private void Update ()
    {
        if (_match3.IsPlaying)
        {
            if (!_match3.IsBusy)
            {
                HandleInput();
            }
            _match3.DoWork();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            _match3.StartNewGame();
        }
        
    }

    /*private void OnDestroy()
    {
        _inputService.OnClick -= OnClick;
    }

    private void OnClick(Vector3 position)
    {
        _inputPosition = position;
        _isInput = true;
    }*/
	
    private void HandleInput ()
    {
        if (_automaticPlay)
        {
            _match3.DoAutomaticMove();
        }
        else if (!_isDragging && Input.GetMouseButtonDown(0))
        {
            _dragStart = Input.mousePosition;
            _isDragging = true;
        }
        else if (_isDragging && Input.GetMouseButtonDown(0))
        {
            _isDragging = _match3.EvaluateDrag(_dragStart, Input.mousePosition);
        }
        else
        {
            _isDragging = false;
        }
    }
}
