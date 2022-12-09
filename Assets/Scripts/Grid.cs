using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Grid : MonoBehaviour
{
    
    public List<Cell> Cells { get; private set; }
    [SerializeField] private List<CellObject> _potentialCells = new List<CellObject>();

    public void SetupGrid(int rowSize, int colSize)
    {
        // Loops and initializes the grid 
        for(int row = 0;row < rowSize;row++)
        {
            for(int col = 0;col < colSize;col++)
            {

            }
        }
    }

}
