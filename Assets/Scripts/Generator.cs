using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DungeonGenerator;

public class Generator : MonoBehaviour
{
    [Header("Grid setup")]
    [SerializeField] private DungeonGenerator.Grid _grid;
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private int _levels;
    [SerializeField] private Transform _gridParent;

    [Header("Algorithm variables")]
    [SerializeField] private bool _loopTillEnd = true;
    [SerializeField] private int _maxIterations = 0; // If this variable is 0 we loop infinitely
    private int _iterationCounter = 0;

    [Header("Random weight sliders")]
    [Range(0f, 1f)]
    [SerializeField] private float _wallWeight = 0.48f;
    [Range(0f, 1f)]
    [SerializeField] private float _stairWeight = 0.6f;
    [Range(0f, 1f)]
    [SerializeField] private float _hallwayWeight = 0.48f;
    [Range(0f, 1f)]
    [SerializeField] private float _cornerWeight = 0.48f;

    [SerializeField] private List<ExampleObject> _exampleObjects = new List<ExampleObject>();

    public bool IsCollapsed
    {
        get
        {
            foreach (Cell cell in _grid.Cells)
            {
                if (!cell.IsCollapsed)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public void OnValidate()
    {
        UpdateWeights();
    }

    public void CollapseGrid()
    {
        ClearGrid();
        _iterationCounter = 0;
        _grid.SetupGrid(_rows, _columns, _levels);
        string name = "Level ";
        for(int i =0; i < _levels;i++)
        {
            GameObject levelObj = new GameObject(name + i);
            levelObj.transform.parent = _gridParent;
        }
        while (!IsCollapsed)
        {
            if (!_loopTillEnd && _iterationCounter >= _maxIterations)
            {
                break;
            }

            CollapseCell();
            ++_iterationCounter;
        }
        Debug.Log("You finished after " + _iterationCounter + " iterations");
    }

    public void ClearGrid()
    {

        while (_gridParent.childCount > 0)
        {
            DestroyImmediate(_gridParent.GetChild(0).gameObject);

        }
        _grid.Cells.Clear();
    }


    // Examples
    public void Example(int index)
    {

        ExampleObject exampleObject = _exampleObjects[index];
        if(exampleObject == null)
        {
            return;
        }
        _rows = exampleObject.Rows;
        _columns = exampleObject.Columns;
        _levels = exampleObject.Levels;

        _wallWeight = exampleObject.WallWeight;
        _stairWeight= exampleObject.StairWeight;
        _hallwayWeight = exampleObject.HallwayWeight;
        _cornerWeight = exampleObject.CornerWeight;
        UpdateWeights();

        CollapseGrid();
    }


    private void CollapseCell()
    {
        // Find and collapse lowest entropy cell
        Cell cell = _grid.GetLowestEntropyCell();

        _grid.CollapseCell(cell, _gridParent);
    }

    private void UpdateWeights()
    {
        if (_grid)
        {
            _grid.WallWeight = _wallWeight;
            _grid.StairWeight = _stairWeight;
            _grid.HallwayWeight = _hallwayWeight;
            _grid.CornerWeight = _cornerWeight;
            _grid.SetupWeights();
        }
    }


}
