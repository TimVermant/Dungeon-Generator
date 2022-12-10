using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [Header("Grid setup")]
    [SerializeField] private Grid _grid;
    [SerializeField] private int _rows;
    [SerializeField] private int _columns;
    [SerializeField] private Transform _gridParent;

    [Header("Algorithm variables")]
    [SerializeField] private bool _loopTillEnd = true;
    [SerializeField] private int _maxIterations = 0; // If this variable is 0 we loop infinitely
    private int _iterationCounter = 0;


    public void CollapseGrid()
    {
        ResetValues();
        while (!IsCollapsed())
        {
            if (!_loopTillEnd && _iterationCounter >= _maxIterations)
            {
                break;
            }

            // Break loop when CollapseCell can't find any cells to collapse anymore
            if (!CollapseCell())
            {
                break;
            }
            ++_iterationCounter;
        }
        Debug.Log("You finished after " + _iterationCounter + " iterations");
    }

    public void ClearGrid()
    {
        foreach (List<Cell> cells in _grid.Cells)
        {
            foreach (Cell cell in cells)
            {
                cell.DestroyCell();

            }
        }

    }


    private bool CollapseCell()
    {
        // Find and collapse lowest entropy cell
        Cell cell = _grid.GetLowestEntropyCell();
        if (cell.IsCollapsed)
        {
            return false;
        }
        _grid.CollapseCell(cell, _gridParent);
        

        return true;
    }



    private void ResetValues()
    {
        _grid.Cells.Clear();
        _grid.SetupGrid(_rows, _columns);
        _iterationCounter = 0;
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
