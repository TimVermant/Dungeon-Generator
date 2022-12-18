using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CellObject", order = 1)]
public class CellObject : ScriptableObject
{
    public GameObject CellPrefab;
    
    // Using an enum to get the correct list of cellobjects to see whats allowed
    public List<CellObjectNeighbbourDirection> AllowedNeighbourCell;
    
    [HideInInspector]
    public CellObjectNeighbbourDirection GetAllowedNeighbourInDirection(CellDirection direction)
    {
        foreach(CellObjectNeighbbourDirection cellObjectNeighbourRules in AllowedNeighbourCell)
        {
            if(cellObjectNeighbourRules.Direction == direction)
            {
                return cellObjectNeighbourRules;
            }
        }
        return null;
    }
  
    public bool HasWall(CellDirection direction)
    {
        CellObjectNeighbbourDirection cellObjectNeighbourDirection = GetAllowedNeighbourInDirection(direction);
        return cellObjectNeighbourDirection.HasWall;
    }
}

[Serializable]
public class CellObjectNeighbbourDirection
{
    public CellDirection Direction;
    public bool HasWall;
}



public enum CellDirection
{
    Right = 0,
    Left = 1,
    Front = 2,
    Back = 3,

}