using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    // List of neighbours (2D will have 4 for now)
    public List<Cell> neighbours { get; private set; }

    // Scriptable object containing the data required for the WFC 
    public CellObject CellObject { get; private set; }
    private GameObject _spawnedCell;

    // Index inside the gride
    private int _cellRow;
    private int _cellCol;
    // private int _gridIndex // For when I go to 3D 


    // Called when initializing the grid layout 
    public void InitializeCell(int row, int col)
    {
        _cellRow = row;
        _cellCol = col;

    }




}
