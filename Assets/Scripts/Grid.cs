using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DungeonGenerator
{



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



        public void CollapseCell(Cell cell, Transform parent)
        {
            CellObject value = CalculateCellValue(cell);
            cell.Collapse(value);
            SpawnCell(cell, parent);
        }

        public void SpawnCell(Cell cell, Transform parent)
        {
            float startPosX = -_rowSize * 0.5f * _gridSize;
            float startPosY = -_columnSize * 0.5f * _gridSize;
            Vector3 cellPosition = new Vector3(
                startPosX + cell.Row * _gridSize,
                0,
                startPosY + cell.Column * _gridSize);
            cell.InstantiateCell(cellPosition, parent);
        }


        // GETTERS
        // Public
        public Cell GetLowestEntropyCell(bool canBeCollapsed = false)
        {
            int entropy = _potentialCells.Count + 1;
            Cell lowestCell = GetRandomCell();
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
        private List<Cell> GetCollapsedNeighbours(Cell cell)
        {
            List<Cell> neighbours = GetNeighbours2D(cell);
            List<Cell> uniqueList = new();
            foreach (Cell cellItem in neighbours)
            {
                if (cellItem != null && cellItem.IsCollapsed)
                {
                    uniqueList.Add(cellItem);
                }
            }
            return uniqueList;
        }

        private int GetEntropy(Cell cell)
        {
            List<Cell> neighbours = GetCollapsedNeighbours(cell);
            // Return max possible options when there are no neighbours yet
            if (neighbours.Count <= 0)
            {
                return _potentialCells.Count;
            }
            List<CellObject> cellOptions = GetAllPossibleVariations(cell, neighbours);


            return cellOptions.Count;
        }

        private CellObject CalculateCellValue(Cell cell)
        {

            List<CellObject> potentialValues = GetAllPossibleVariations(cell, GetCollapsedNeighbours(cell));

            CellObject cellValue = null;

            // Randomize if no neighbours
            if (potentialValues.Count == 0)
            {
                int index = Random.Range(0, _potentialCells.Count - 1);
                cellValue = _potentialCells[index];

            }
            else
            {

                int index = Random.Range(0, potentialValues.Count - 1);
                cellValue = potentialValues[index];

            }
            return cellValue;
        }

        // Finds the overlap of all the unique neighbours and calculates all the options
        private List<CellObject> GetAllPossibleVariations(Cell mainCell, List<Cell> uniqueNeighbours)
        {

            List<CellObject> cellOptions = new List<CellObject>();

            foreach (Cell cellItem in uniqueNeighbours)
            {

                // Find out the direction from the neighbour to the mainCell
                CellDirection direction = GetDirection(cellItem, mainCell);
                // Add options from that specific direction
                CellObjectNeighbourRules rules = GetNeighbourRules(cellItem, direction);
                if (rules != null)
                {
                    cellOptions.AddRange(rules.PossibleVariations);

                }

            }


            List<CellObject> uniqueValues = new List<CellObject>();
            List<CellObject> overlappingValues = new List<CellObject>();
            foreach (CellObject cellItem in cellOptions)
            {

                uniqueValues.Add(cellItem);
                if (uniqueValues.Contains(cellItem))
                {
                    overlappingValues.Add(cellItem);

                }
            }

            return overlappingValues;
        }


        private Cell GetRandomCell()
        {
            return Cells[Random.Range(0, _rowSize)][Random.Range(0, _columnSize)];
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
            return row >= 0 && col >= 0 && row < _rowSize && col < _columnSize;
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

        private CellDirection GetOppositeDirection(CellDirection direction)
        {
            int amountOfDirections = 4;
            int dir = (int)direction + 2;
            if (dir >= amountOfDirections)
            {
                dir -= amountOfDirections;
            }
            return (CellDirection)dir;
        }

        private bool HasWall(Cell from, Cell to)
        {
            CellDirection direction = GetDirection(from, to);
            return GetNeighbourRules(from, direction).HasWall;

        }



        private CellObjectNeighbourRules GetNeighbourRules(Cell cell, CellDirection direction)
        {
            if (cell.CurrentCellObject == null)
            {
                return null;
            }
            foreach (CellObjectNeighbourRules cellNeighbour in cell.CurrentCellObject.AllowedNeighbourCell)
            {
                if (cellNeighbour.Direction == direction)
                {
                    return cellNeighbour;
                }
            }
            return null;
        }


    }
}
