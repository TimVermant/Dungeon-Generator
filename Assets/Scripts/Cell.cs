using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{

    // Scriptable object containing the data required for the WFC 
    public CellObject CurrentCellObject { get; private set; }
    public bool IsCollapsed { get; private set; }
    public bool IsOutsideEdge { get; private set; }
    public bool IsStair { get; private set; }
    public int Row { get; private set; }
    public int Column { get; private set; }
    public int Level { get; private set; }

    private GameObject _spawnedCell = null;



    // Called when initializing the grid layout 
    public void InitializeCell(int row, int col, int height)
    {
        Row = row;
        Column = col;
        Level = height;
    }

    public void CollapseEdgeCell()
    {
        IsOutsideEdge = true;
        IsCollapsed = true;
    }

    public void Collapse(CellObject cellObject, bool isStair)
    {
        CurrentCellObject = cellObject;
        IsCollapsed = true;
        IsStair = isStair;
    }

    public void InstantiateCell(Vector3 pos,Transform parent)
    {
        if(_spawnedCell != null)
        {
            GameObject.DestroyImmediate(_spawnedCell);

        }
        _spawnedCell = GameObject.Instantiate(CurrentCellObject.CellPrefab, pos, Quaternion.Euler(0, CurrentCellObject.Rotation,0), parent);
        _spawnedCell.name = CurrentCellObject.name;

    }

    public void DestroyCell()
    {
       GameObject.DestroyImmediate(_spawnedCell);
        _spawnedCell = null;
    }


 



}
