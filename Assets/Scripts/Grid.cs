using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Grid : MonoBehaviour
{

    public List<List<Cell>> Cells { get; private set; } = new List<List<Cell>>();
    [SerializeField] private List<CellObject> _potentialCells = new List<CellObject>();

    private int _rowSize;
    private int _columnSize;
    private float _gridSize = 2.0f; // The size of the prefab pieces 

    public void SetupGrid(int rowSize, int colSize)
    {

        _rowSize = rowSize;
        _columnSize = colSize;
        // Loops and initializes the grid 
        for (int row = 0; row < rowSize; row++)
        {
            Cells.Add(new List<Cell>());
            for (int col = 0; col < colSize; col++)
            {
                Cell cell = new();
                cell.InitializeCell(row, col);
                Cells[row].Add(cell);
            }
        }
    }



    public void CollapseCell(Cell cell,Transform parent)
    {
        CellObject value = CalculateCellValue(cell);
        cell.Collapse(value);
        SpawnCell(cell, parent);
    }

    public void SpawnCell(Cell cell,Transform parent)
    {
        float startPosX = -_rowSize * 0.5f * _gridSize;
        float startPosY = -_columnSize * 0.5f * _gridSize;
        Vector3 cellPosition = new Vector3(
            startPosX + cell.Row * _gridSize,
            0,
            startPosY + cell.Column * _gridSize);
        cell.InstantiateCell(cellPosition,parent);
    }

    // GETTERS
    // Public
    public Cell GetLowestEntropyCell(bool canBeCollapsed = false)
    {
        int entropy = _potentialCells.Count;
        Cell lowestCell = Cells[0][0];
        foreach (List<Cell> cellList in Cells)
        {
            foreach (Cell cell in cellList)
            {
                int newEntropy = GetEntropy(cell);
                if (newEntropy < entropy && canBeCollapsed == cell.IsCollapsed)
                {
                    entropy = newEntropy;
                    lowestCell = cell;
                }
            }
        }

        // Return null if all nodes are collapsed
        if (lowestCell.IsCollapsed != canBeCollapsed)
        {
            return null;
        }
        return lowestCell;
    }


    // Private
    private List<Cell> GetNeighbours2D(Cell cell)
    {
        int row = cell.Row;
        int column = cell.Column;
        List<Cell> neighbours = new List<Cell>();
        neighbours.Add(GetCell(row + 1, column)); // Right
        neighbours.Add(GetCell(row, column - 1)); // Back
        neighbours.Add(GetCell(row - 1, column)); // Left
        neighbours.Add(GetCell(row, column + 1)); // Front

        return neighbours;
    }

    // Filters out and adds all the unique collapsed cells
    private List<Cell> GetUniqueNeighbours2D(Cell cell)
    {
        List<Cell> neighbours = GetNeighbours2D(cell);
        List<Cell> uniqueList = new();
        foreach (Cell cellItem in neighbours)
        {
            if (cellItem != null && !uniqueList.Contains(cellItem) &&
                cellItem.IsCollapsed)
            {
                uniqueList.Add(cellItem);
            }
        }
        return uniqueList;
    }

    private int GetEntropy(Cell cell)
    {
        List<Cell> neighbours = GetUniqueNeighbours2D(cell);
        List<CellObject> uniqueObjects = CalculateCellOptions(cell, neighbours);

        return uniqueObjects.Count;
    }

    private CellObject CalculateCellValue(Cell cell)
    {
        List<CellObject> cellValues = CalculateCellOptions(cell, GetUniqueNeighbours2D(cell));
        CellObject cellValue = new();

        if (cellValues.Count == 1)
        {
            cellValue = cellValues[0];
        }
        else if (cellValues.Count == 0)
        {
            int index = Random.Range(0, _potentialCells.Count - 1);
            cellValue = _potentialCells[index];

        }
        else
        {
            int index = Random.Range(0, cellValues.Count - 1);
            cellValue = cellValues[index];
        }
        return cellValue;
    }

    // Finds the overlap of all the unique neighbours and calculates all the options
    private List<CellObject> CalculateCellOptions(Cell mainCell, List<Cell> uniqueNeighbours)
    {
        List<CellObject> cellOptions = new List<CellObject>();
        foreach (Cell cellItem in uniqueNeighbours)
        {
            // Find out the direction from the neighbour to the mainCell
            CellDirection direction = GetDirection(cellItem, mainCell);

            // Add options from that specific direction
            foreach (CellObjectNeighbourRules cellNeighbour in cellItem.CurrentCellObject.AllowedNeighbourCell)
            {
                if (cellNeighbour.Direction == direction)
                {
                    cellOptions.AddRange(cellOptions);
                }
            }
        }

        // Loop over and remove non unique items
        List<CellObject> overlappingOptions = new();
        foreach (CellObject cellObject in cellOptions)
        {
            if (!overlappingOptions.Contains(cellObject))
            {
                overlappingOptions.Add(cellObject);
            }
        }
        return overlappingOptions;
    }



    // Helpers
    private Cell GetCell(Cell cell)
    {
        return GetCell(cell.Row, cell.Column);
    }

    private Cell GetCell(int row, int col)
    {
        if (!IsValidIndex(row, col))
        {
            return null;
        }

        return Cells[row][col];
    }


    private bool IsValidIndex(int row, int col)
    {
        return row > 0 && col > 0 && row < _rowSize && col < _columnSize;
    }

    // Find the row/col difference to find the relative position on the grid
    private CellDirection GetDirection(Cell from, Cell to)
    {
        int rowDif = from.Row - to.Row;
        int colDif = from.Column - to.Column;
        if (colDif == 1)
        {
            return CellDirection.Front;
        }
        else if (colDif == -1)
        {
            return CellDirection.Back;
        }
        else if (rowDif == 1)
        {
            return CellDirection.Right;
        }
        else if (rowDif == -1)
        {
            return CellDirection.Left;
        }

        return CellDirection.Left;
    }

}
