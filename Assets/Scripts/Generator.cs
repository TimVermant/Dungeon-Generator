using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Generator : MonoBehaviour
{
    [Header("Grid setup")]
    [SerializeField] private DungeonGenerator.Grid _grid;
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private Transform _gridParent;

    [Header("Algorithm variables")]
    [SerializeField] private bool _loopTillEnd = true;
    [SerializeField] private int _maxIterations = 0; // If this variable is 0 we loop infinitely
    private int _iterationCounter = 0;


    public void CollapseGrid()
    {
        ClearGrid();
        _iterationCounter = 0;
        _grid.SetupGrid(_rows, _columns);
        while (_iterationCounter < _rows * _columns)
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
          
        while(_gridParent.childCount > 0)
        {
            DestroyImmediate(_gridParent.GetChild(0).gameObject);

        }
        _grid.Cells.Clear();
    }


    private void CollapseCell()
    {
        // Find and collapse lowest entropy cell
        Cell cell = _grid.GetLowestEntropyCell();

        _grid.CollapseCell(cell, _gridParent);
    }



  
    private bool IsCollapsed()
    {
        foreach (List<Cell> cells in _grid.Cells)
        {
            foreach (Cell cell in cells)
            {
                if (!cell.IsCollapsed)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
