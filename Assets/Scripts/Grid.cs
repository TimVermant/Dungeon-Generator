using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DungeonGenerator
{

    public class Grid : MonoBehaviour
    {
        public List<Cell> Cells { get; private set; } = new List<Cell>();
        [SerializeField] private List<CellObject> _potentialCells = new List<CellObject>();
        private List<CellDirection> _possibleDirections = new() { CellDirection.Right, CellDirection.Left, CellDirection.Front, CellDirection.Back };
        private int _rowSize;
        private int _columnSize;
        private int _dimensionsSize;
        private float _gridSize = 2.0f; // The size of the prefab pieces 



        public void SetupGrid(int rowSize, int colSize, int dimensionsSize)
        {

            _rowSize = rowSize;
            _columnSize = colSize;
            _dimensionsSize = dimensionsSize;


            Cells = new List<Cell>(new Cell[rowSize * colSize * dimensionsSize]);

            for (int height = 0; height < _dimensionsSize; height++)
            {
                SetupLevel(height);
            }
        }

        private void SetupLevel(int level)
        {

            int index = 0;

            // Loops and initializes the grid 
            for (int col = 0; col < _columnSize; col++)
            {
                for (int row = 0; row < _rowSize; row++)
                {
                    Cell cell = new();
                    cell.InitializeCell(row, col, level);
                    if (!IsValidIndex(row, col, level))
                    {
                        cell.CollapseEdgeCell();
                    }
             
                    index = CoordinateToIndex(row, col, level);

                    Cells[index] = cell;

                }
            }
        }



        // Base function that calculates the cell value based on its entropy
        public void CollapseCell(Cell cell, Transform parent)
        {
            CellObject value = CalculateCellValue(cell);
            cell.Collapse(value);
            SpawnCell(cell, parent);
        }

        public void SpawnCell(Cell cell, Transform parent)
        {
            float startPosX = -_rowSize * 0.5f * _gridSize;
            float startPosZ = -_columnSize * 0.5f * _gridSize;
            Vector3 cellPosition = new Vector3(
                startPosX + cell.Row * _gridSize,
                cell.Level * _gridSize,
                startPosZ + cell.Column * _gridSize);
            cell.InstantiateCell(cellPosition, parent);
        }


        // GETTERS
        // Public
        public Cell GetLowestEntropyCell(bool canBeCollapsed = false)
        {
            Cell lowestCell = GetRandomUncollapsedCell();
            int entropy = _potentialCells.Count + 1;
            List<Cell> lowestCells = new();
            foreach (Cell cell in Cells)
            {
                if (cell == null)
                    continue;

                int newEntropy = GetEntropy(cell);
                if (newEntropy <= 0)
                {
                    continue;
                }
                if (canBeCollapsed == cell.IsCollapsed)
                {
                    if (newEntropy < entropy)
                    {
                        entropy = newEntropy;
                        lowestCells.Clear();
                        lowestCells.Add(cell);
                    }
                    else if (newEntropy == entropy)
                    {
                        lowestCells.Add(cell);
                    }
                }


            }
            Debug.Log(entropy);

            if (lowestCells.Count > 0)
            {
                lowestCell = lowestCells[Random.Range(0, lowestCells.Count - 1)];

            }
            return lowestCell;
        }


        // Private
        private List<Cell> GetNeighbours2D(Cell cell)
        {
            List<Cell> neighbours = new List<Cell>();
            int row = cell.Row;
            int column = cell.Column;
            int level = cell.Level;
            neighbours.Add(GetCell(row + 1, column, level)); // Right
            neighbours.Add(GetCell(row, column + 1, level)); // Front
            neighbours.Add(GetCell(row - 1, column, level)); // Left
            neighbours.Add(GetCell(row, column - 1, level)); // Back

            return neighbours;
        }

        private List<Cell> GetNeighbours3D(Cell cell)
        {

            List<Cell> neigbhours = new List<Cell>();
            int row = cell.Row;
            int column = cell.Column;
            int level = cell.Level;
            neigbhours.AddRange(GetNeighbours2D(cell));

            if (IsValidLevel(level + 1))
                neigbhours.AddRange(GetNeighbours2D(GetCell(row, column, level + 1))); ;
            if (IsValidLevel(level - 1))
                neigbhours.AddRange(GetNeighbours2D(GetCell(row, column, level - 1)));
            return neigbhours;
        }

        // Filters out and adds all the unique collapsed cells
        private List<Cell> GetCollapsedNeighbours(Cell cell)
        {
            if (cell == null)
                Debug.Log("Cell is null");
            List<Cell> neighbours = GetNeighbours2D(cell);
            List<Cell> uniqueList = new();
            foreach (Cell cellItem in neighbours)
            {

                if (cellItem != null)
                {
                    if (cellItem.IsCollapsed)
                    {
                        uniqueList.Add(cellItem);
                    }
                }
                else
                {
                    Cell edgeCell = new();
                    edgeCell.CollapseEdgeCell();
                    uniqueList.Add(edgeCell);
                }
            }
            return uniqueList;
        }



        private int GetEntropy(Cell cell)
        {
            List<Cell> neighbours = GetCollapsedNeighbours(cell);
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
                cellValue = GetWeightedValue(_potentialCells);
            }
            else
            {
                cellValue = GetWeightedValue(potentialValues);
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
                CellDirection oppositeDirection = GetDirection(mainCell, cellItem);

                // For now ignore the neighbour check on tiles above/below
                if (direction == CellDirection.Above || direction == CellDirection.Below)
                    continue;
                bool hasWall = false;
                if (cellItem.IsOutsideEdge)
                {
                    hasWall = true;

                }
                else
                {
                    hasWall = cellItem.CurrentCellObject.HasWall(direction);
                }
                // Add option if it has a wall in that direction
                foreach (CellObject potentialCell in _potentialCells)
                {
                    if (potentialCell.HasWall(oppositeDirection) == hasWall)
                    {
                        cellOptions.Add(potentialCell);
                    }
                }
            }

            cellOptions = GetOnlyPossibleVariations(mainCell, uniqueNeighbours, cellOptions);

            return cellOptions;
        }

        private List<CellObject> GetOnlyPossibleVariations(Cell mainCell, List<Cell> uniqueNeighbours, List<CellObject> options)
        {
            List<CellObject> cellOptions = new(options);
            List<CellObject> duplicates = new();
            List<CellObject> finalList = new();
            foreach (Cell cellItem in uniqueNeighbours)
            {

                // Find out the direction from the neighbour to the mainCell
                CellDirection neighbourToMain = GetDirection(cellItem, mainCell);
                CellDirection mainToNeighbour = GetDirection(mainCell, cellItem);
                
                bool hasWall = false;
                if (cellItem.IsOutsideEdge)
                {
                    hasWall = true;

                }
                else
                {
                    hasWall = cellItem.CurrentCellObject.HasWall(neighbourToMain);
                }
                // Add option if it has a wall in that direction
                foreach (CellObject potentialCell in options)
                {

                    if (potentialCell.HasWall(mainToNeighbour) != hasWall)
                    {
                        cellOptions.Remove(potentialCell);
                    }



                }

            }
            foreach (CellObject potentialCell in cellOptions)
            {
                if (!finalList.Contains(potentialCell))
                {
                    finalList.Add(potentialCell);
                }
            }

            return finalList;
        }

        private Cell GetRandomUncollapsedCell()
        {
            Cell cell = GetRandomCell();
            while (cell.IsCollapsed)
            {
                cell = GetRandomCell();
            }
            return cell;
        }

        private Cell GetRandomCell()
        {
            return GetCell(Random.Range(0, _rowSize), Random.Range(0, _columnSize), Random.Range(0, _dimensionsSize));
        }




        // Helpers
        private Cell GetCell(Cell cell)
        {
            return GetCell(cell.Row, cell.Column, cell.Level);
        }

        private Cell GetCell(int row, int col, int level)
        {
            if (!IsValidIndex(row, col, level))
            {
                Cell cell = new();
                cell.InitializeCell(row, col, level);
                cell.CollapseEdgeCell();
                return cell;
            }

            return Cells[CoordinateToIndex(row, col, level)];
        }



        private bool IsValidIndex(int row, int col, int level)
        {
            return row >= 0 && col >= 0 && level >= 0 &&
                row < _rowSize && col < _columnSize && level < _dimensionsSize;
        }

        private bool IsValidLevel(int level)
        {
            return level >= 0 && level < _dimensionsSize;
        }

        // Find the row/col difference to find the relative position on the grid
        private CellDirection GetDirection(Cell from, Cell to)
        {
            int rowDif = from.Row - to.Row;
            int colDif = from.Column - to.Column;
            int levelDif = from.Level - to.Level;
            if (colDif == -1)
            {
                return CellDirection.Front;
            }
            else if (colDif == 1)
            {
                return CellDirection.Back;
            }
            else if (rowDif == -1)
            {
                return CellDirection.Right;
            }
            else if (rowDif == 1)
            {
                return CellDirection.Left;
            }
            else if (levelDif == 1)
            {
                return CellDirection.Below;
            }
            else if(levelDif == -1)
            {
                return CellDirection.Above;
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

        private CellObject GetWeightedValue(List<CellObject> potentialOptions)
        {
            if (potentialOptions.Count == 1)
            {
                return potentialOptions[0];
            }
            List<CellObject> shuffledList = GetShuffledObjects(potentialOptions);
            float weightSum = 0.0f;
            weightSum = GetWeightSum(shuffledList);
            float randomNumber = Random.Range(0, weightSum);
            foreach (CellObject cellObject in shuffledList)
            {
                // To account for floating point errors
                if (Mathf.Abs(weightSum - cellObject.Weight) <= 0.0001f)
                {
                    return cellObject;
                }
                weightSum -= cellObject.Weight;

            }
            Debug.LogError("No valid weighted value found: " + weightSum);

            return null;

        }

        private List<CellObject> GetShuffledObjects(List<CellObject> potentialOptions)
        {
            List<CellObject> shuffledObjects = new List<CellObject>(potentialOptions);
            for (int i = 0; i < shuffledObjects.Count; i++)
            {
                CellObject tempObj = shuffledObjects[i];
                int randomIndex = Random.Range(i, shuffledObjects.Count);
                shuffledObjects[i] = shuffledObjects[randomIndex];
                shuffledObjects[randomIndex] = tempObj;
            }
            return shuffledObjects;
        }

        private float GetWeightSum(List<CellObject> potentialOptions)
        {
            float weightSum = 0.0f;
            foreach (CellObject option in potentialOptions)
            {
                weightSum += option.Weight;
            }
            return weightSum;
        }


        private int CoordinateToIndex(int row, int column, int level)
        {
            if (!IsValidIndex(row, column, level))
                return -1;
            return row + (column * _rowSize) + (level * _rowSize * _columnSize);
        }

    }
}
