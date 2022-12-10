using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{

    // Scriptable object containing the data required for the WFC 
    public CellObject CurrentCellObject { get; private set; }
    public bool IsCollapsed { get; private set; }
    public int Row { get; private set; }
    public int Column { get; private set; }

    private GameObject _spawnedCell = null;



    // Called when initializing the grid layout 
    public void InitializeCell(int row, int col)
    {
        Row = row;
        Column = col;

    }

    public void Collapse(CellObject cellObject)
    {
        CurrentCellObject = cellObject;
        IsCollapsed = true;

    }

    public void InstantiateCell(Vector3 pos,Transform parent)
    {
        if(_spawnedCell != null)
        {
            GameObject.DestroyImmediate(_spawnedCell);

        }
        _spawnedCell = GameObject.Instantiate(CurrentCellObject.CellPrefab, pos, Quaternion.identity, parent);

    }

    public void DestroyCell()
    {
       GameObject.DestroyImmediate(_spawnedCell);
        _spawnedCell = null;
    }


 



}
