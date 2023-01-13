using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DungeonGenerator
{

    public class Grid : MonoBehaviour
    {
        public List<Cell> Cells { get; private set; } = new List<Cell>();
        public List<Cell> CollapsedCells { get; private set; } = new List<Cell>();
        public float WallWeight { get; set; }
        public float HallwayWeight { get; set; }
        public float CornerWeight { get; set; }
        public float StairWeight { get; set; }
        private List<CellObject> _potentialCells = new List<CellObject>();
        [SerializeField] private List<CellObject> _wallCells = new List<CellObject>();
        [SerializeField] private List<CellObject> _hallwayCells = new List<CellObject>();
        [SerializeField] private List<CellObject> _cornerCells = new List<CellObject>();
        [SerializeField] private List<CellObject> _stairCells = new List<CellObject>();
        [SerializeField] private List<CellObject> _specialCells = new List<CellObject>();
        private float _stairStartWeight = 0.6f;
        private int _rowSize;
        private int _columnSize;
        private int _dimensionsSize;
        private float _gridSize = 2.0f; // The size of the prefab pieces 

        // -----------------------------------
        // -- INITIALIZING THE GRID LAYOUT -- 
        // -----------------------------------
        #region Grid Initialization

        /// <summary>
        /// Setting up the layout of the 3D grid
        /// </summary>
        /// <param name="rowSize">The amount of rows</param>
        /// <param name="colSize">The amount of columns</param>
        /// <param name="dimensionsSize">The amount of levels</param>
        public void SetupGrid(int rowSize, int colSize, int dimensionsSize)
        {


            _potentialCells.Clear();
            _potentialCells.AddRange(_wallCells);
            _potentialCells.AddRange(_stairCells);
            _potentialCells.AddRange(_cornerCells);
            _potentialCells.AddRange(_hallwayCells);
            _potentialCells.AddRange(_specialCells);

            foreach (CellObject stairCell in _stairCells)
            {
                stairCell.Weight = _stairStartWeight;
            }

            _rowSize = rowSize;
            _columnSize = colSize;
            _dimensionsSize = dimensionsSize;

            _potentialCells[0].Weight = 0.69f;
            Cells = new List<Cell>(new Cell[rowSize * colSize * dimensionsSize]);

            for (int height = 0; height < _dimensionsSize; height++)
            {
                SetupLevel(height);
            }
        }

        public void SetupWeights()
        {
            foreach (CellObject cell in _wallCells)
            {
                cell.Weight = WallWeight;
            }

            foreach (CellObject cell in _stairCells)
            {
                cell.Weight = StairWeight;
            }

            foreach (CellObject cell in _cornerCells)
            {
                cell.Weight = CornerWeight;
            }

            foreach (CellObject cell in _hallwayCells)
            {
                cell.Weight = HallwayWeight;
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
                    if (!IsOutOfBounds(row, col, level))
                    {
                        cell.CollapseEdgeCell();
                    }

                    index = GetIndexFromCoordinate(row, col, level);

                    Cells[index] = cell;

                }
            }

        }

        #endregion

        // ----------------------------------------------------
        // -- FINDING AND COLLAPSING THE LOWEST ENTROPY CELL -- 
        // ----------------------------------------------------
        #region Cell Collapsing

        /// <summary>
        /// Function that calculates and collapses the given cell 
        /// Gets called from outside by the Generator.cs
        /// </summary>
        /// <param name="cell">Cell to collapse</param>
        /// <param name="parent">Transform to parent to the spawned object</param>
        public void CollapseCell(Cell cell, Transform parent)
        {

            CellObject value = CalculateCellValue(cell);
            cell.Collapse(value);
            CollapsedCells.Add(cell);
            // Spawn in cell
            float startPosX = -_rowSize * 0.5f * _gridSize;
            float startPosZ = -_columnSize * 0.5f * _gridSize;

            Vector3 cellPosition = new Vector3(
                startPosX + cell.Row * _gridSize,
                cell.Level * 0.5f * _gridSize + (cell.Level * 0.1f),
                startPosZ + cell.Column * _gridSize);
            cell.InstantiateCell(cellPosition, parent.GetChild(cell.Level));
        }

        /// <summary>
        /// Loops over and gets the lowest entropy cell
        /// </summary>
        /// <returns>Returns the cell with the lowest entropy</returns>
        public Cell GetLowestEntropyCell()
        {
            Cell lowestCell = GetRandomUncollapsedCell();
            int entropy = _potentialCells.Count;
            List<Cell> lowestCells = new();
            foreach (Cell cell in Cells)
            {
                if (cell == null)
                    continue;

                int newEntropy = CalculateEntropy(cell);
                if (newEntropy <= 0)
                {
                    continue;
                }

                if (!cell.IsCollapsed)
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


            if (lowestCells.Count > 0)
            {
                lowestCell = lowestCells[Random.Range(0, lowestCells.Count - 1)];

            }
            return lowestCell;
        }

        #endregion

        // ----------------------
        // -- WFC CALCULATIONS -- 
        // ----------------------
        #region WFC Calculations
        /// <summary>
        /// Calculates the entropy of the given cell
        /// </summary>
        /// <param name="cell">The given cell from which we need to know the entropy</param>
        /// <returns>The calculated entropy</returns>
        private int CalculateEntropy(Cell cell)
        {
            List<Cell> neighbours = GetCollapsedNeighbours(cell);
            List<CellObject> cellOptions = CalculatePossibleVariations(cell, neighbours);
            return cellOptions.Count;
        }

        /// <summary>
        /// Calculates the value of the given cell based on its entropy
        /// </summary>
        /// <param name="cell">The given cell from which we need to know the value</param>
        /// <returns>The calculated value</returns>
        private CellObject CalculateCellValue(Cell cell)
        {

            List<CellObject> potentialValues = CalculatePossibleVariations(cell, GetCollapsedNeighbours(cell));

            CellObject cellValue = null;

            // Randomize if no neighbours
            if (potentialValues.Count == 0)
            {
                cellValue = CalculateWeightedValue(_potentialCells);
            }
            else
            {
                cellValue = CalculateWeightedValue(potentialValues);
            }
            return cellValue;
        }

        /// <summary>
        /// Calculates the possible variations on an uncollapsed cell based on the previously calculated collapsedNeighbours
        /// </summary>
        /// <param name="mainCell">Cell for which we need to calculate the possible variations</param>
        /// <param name="collapsedNeighbours">Collapsed neighbours to the mainCell</param>
        /// <returns></returns>
        private List<CellObject> CalculatePossibleVariations(Cell mainCell, List<Cell> collapsedNeighbours)
        {
            List<CellObject> cellOptions = new(_potentialCells);
            List<CellObject> finalOptions = new();
            foreach (Cell cellItem in collapsedNeighbours)
            {

                // Find out the direction from the neighbour to the mainCell and back
                CellDirection neighbourToMain = GetDirection(cellItem, mainCell);
                CellDirection mainToNeighbour = GetDirection(mainCell, cellItem);

                bool hasWall = true;
                if (!cellItem.IsOutsideEdge)
                {
                    hasWall = cellItem.CurrentCellObject.HasObstruction(neighbourToMain);
                }

                RemoveInvalidOptions(mainCell, cellItem, mainToNeighbour, cellOptions, hasWall);

            }

            return cellOptions;
        }

        /// <summary>
        /// Helper that removes all the invalid options from the list of options
        /// </summary>
        /// <param name="mainCell"></param>
        /// <param name="cellItem"></param>
        /// <param name="mainToNeighbour"></param>
        /// <param name="cellOptions"></param>
        /// <param name="hasWall"></param>
        private void RemoveInvalidOptions(Cell mainCell, Cell cellItem, CellDirection mainToNeighbour, List<CellObject> cellOptions, bool hasWall)
        {
            // Add option if it has a wall in that direction
            foreach (CellObject potentialCell in _potentialCells)
            {
                if (!cellOptions.Contains(potentialCell))
                {
                    continue;
                }


                // Default checks
                if (potentialCell.HasObstruction(mainToNeighbour) != hasWall)
                {
                    cellOptions.Remove(potentialCell);
                }

            }
            if (mainToNeighbour != CellDirection.Above)
            {
                return;
            }

            foreach (CellObject potentialStair in _stairCells)
            {

                if (cellOptions.Contains(potentialStair) /*&& !CanPlaceStaircase(mainCell, cellItem, potentialStair)*/)
                {
                    cellOptions.Remove(potentialStair);
                }

            }
        }

        /// <summary>
        /// Randomly calculates the new value taking weight into account
        /// </summary>
        /// <param name="potentialOptions"></param>
        /// <returns></returns>
        private CellObject CalculateWeightedValue(List<CellObject> potentialOptions)
        {
            if (potentialOptions.Count == 1)
            {
                return potentialOptions[0];
            }
            List<CellObject> shuffledList = GetShuffledObjects(potentialOptions);
            float weightSum = 0.0f;
            weightSum = CalculateWeightSum(shuffledList);
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

        /// <summary>
        /// Calculates the sum of all the weights
        /// </summary>
        /// <param name="potentialOptions"></param>
        /// <returns></returns>
        private float CalculateWeightSum(List<CellObject> potentialOptions)
        {
            float weightSum = 0.0f;
            foreach (CellObject option in potentialOptions)
            {
                weightSum += option.Weight;
            }
            return weightSum;
        }
        #endregion

        // ---------------------
        // -- HELPER GETTERS -- 
        // ---------------------
        #region Helper Getters
        /// <summary>
        /// Get the neighbours inside of a 2D space
        /// </summary>
        /// <param name="cell">Cell from which the neighbours need to be calculated</param>
        /// <returns>Neighbours of the given cell</returns>
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


        /// <summary>
        /// Get the neighbours inside of a 3D space
        /// </summary>
        /// <param name="cell">Cell from which the neighbours need to be calculated</param>
        /// <returns>Neighbours of the given cell</returns>
        private List<Cell> GetNeighbours3D(Cell cell)
        {

            List<Cell> neigbhours = new List<Cell>();
            int row = cell.Row;
            int column = cell.Column;
            int level = cell.Level;
            neigbhours.AddRange(GetNeighbours2D(cell));


            neigbhours.Add(GetCell(row, column, level + 1));
            neigbhours.Add(GetCell(row, column, level - 1));
            return neigbhours;
        }

        /// <summary>
        /// Get all collapsed neighbours from the given cell
        /// Out of bounds cells also count as a form of collapsed cells 
        /// </summary>
        /// <param name="cell">Cell from which the neighbours need to be calculated</param>
        /// <returns>All the collapsed neighbours</returns>
        private List<Cell> GetCollapsedNeighbours(Cell cell)
        {
            if (cell == null)
                Debug.Log("Cell is null");
            List<Cell> neighbours = GetNeighbours3D(cell);
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

        /// <summary>
        /// </summary>
        /// <returns>Returns random uncollapsed cell</returns>
        private Cell GetRandomUncollapsedCell()
        {
            Cell cell = GetRandomCell();
            while (cell.IsCollapsed)
            {
                cell = GetRandomCell();
            }
            return cell;
        }

        /// <summary>
        /// </summary>
        /// <returns>Returns any random cell</returns>
        private Cell GetRandomCell()
        {
            return GetCell(Random.Range(0, _rowSize), Random.Range(0, _columnSize), Random.Range(0, _dimensionsSize));
        }


        /// <summary>
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="level"></param>
        /// <returns>Returns cell from list on the given location</returns>
        private Cell GetCell(int row, int col, int level)
        {
            if (!IsOutOfBounds(row, col, level))
            {
                Cell cell = new();
                cell.InitializeCell(row, col, level);
                cell.CollapseEdgeCell();
                return cell;
            }

            return Cells[GetIndexFromCoordinate(row, col, level)];
        }



        /// <summary>
        /// </summary>
        /// <param name="potentialOptions"></param>
        /// <returns>Returns list of shuffled objects</returns>
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

        /// <summary>
        /// Calculates the direction from - to
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
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
            else if (levelDif == -1)
            {
                return CellDirection.Above;
            }

            return CellDirection.Left;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <returns>Returns the opposite direction</returns>
        private CellDirection GetOppositeDirection(CellDirection direction)
        {
            switch (direction)
            {
                case CellDirection.Right:
                    return CellDirection.Left;
                case CellDirection.Left:
                    return CellDirection.Right;
                case CellDirection.Front:
                    return CellDirection.Back;
                case CellDirection.Back:
                    return CellDirection.Front;
                case CellDirection.Above:
                    return CellDirection.Below;
                case CellDirection.Below:
                    return CellDirection.Above;
            }
            return CellDirection.DirectionCount;
        }

        /// <summary>
        /// Calculates index from coordinate
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private int GetIndexFromCoordinate(int row, int column, int level)
        {
            if (!IsOutOfBounds(row, column, level))
                return -1;
            return row + (column * _rowSize) + (level * _rowSize * _columnSize);
        }


        private Cell GetNeighbourCell(Cell mainCell, CellDirection direction)
        {

            foreach (Cell cell in GetNeighbours2D(mainCell))
            {
                if (GetDirection(mainCell, cell) == direction)
                {
                    return cell;
                }
            }
            return null;
        }
        #endregion

        // --------------------------
        // -- HELPER BOOL CHECKERS -- 
        // --------------------------
        #region Helper Boolean Getters
        /// <summary>
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="level"></param>
        /// <returns>Returns whether or not the given coordinates are out of bounds</returns>
        private bool IsOutOfBounds(int row, int col, int level)
        {
            return row >= 0 && col >= 0 && level >= 0 &&
                row < _rowSize && col < _columnSize && level < _dimensionsSize;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mainCell"></param>
        /// <param name="otherCell"></param>
        /// <param name="staircase"></param>
        /// <returns>Returns if a staircase can be placed at the given cell</returns>
        private bool CanPlaceStaircase(Cell mainCell, Cell otherCell, CellObject staircase)
        {



            List<Cell> neighboursBelow = GetNeighbours2D(mainCell);
            List<Cell> neigboursAbove = GetNeighbours2D(otherCell);
            CellDirection openDirection = staircase.GetOpenDirection2D();
            CellDirection openDirectionOpposite = GetOppositeDirection(openDirection);

            Cell opposingCell = GetNeighbourCell(otherCell, openDirectionOpposite);
            Cell wallCell = GetNeighbourCell(otherCell, openDirection);
            // Can't be out of bounds
            if (opposingCell.IsOutsideEdge || otherCell.IsOutsideEdge)
            {
                return false;
            }

            if (opposingCell.CurrentCellObject != null && opposingCell.CurrentCellObject.HasObstruction(openDirection))

            {
                return false;
            }
            //foreach (Cell neighbour in neigboursAbove)
            //{
            //    if (neighbour.CurrentCellObject != null && neighbour != wallCell &&
            //        !neighbour.CurrentCellObject.HasObstruction(openDirection))

            //    {
            //        return true;
            //    }
            //}

            return true;
        }




        #endregion


    }
}
