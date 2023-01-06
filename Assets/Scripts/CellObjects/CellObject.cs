using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CellObject", order = 1)]
public class CellObject : ScriptableObject
{
    public GameObject CellPrefab;
    public float Rotation = 0;
    [Range(0f, 1f)]
    public float Weight = 1f;
    
    public List<CellObjectNeighbbourDirection> AllowedNeighbourCell = new List<CellObjectNeighbbourDirection>(new CellObjectNeighbbourDirection[(int)CellDirection.DirectionCount]);
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
  
    public bool HasObstruction(CellDirection direction)
    {
        CellObjectNeighbbourDirection cellObjectNeighbourDirection = GetAllowedNeighbourInDirection(direction);
        return cellObjectNeighbourDirection.HasObstruction;
    }
}

[Serializable]
public class CellObjectNeighbbourDirection
{
    public CellDirection Direction;
    public bool HasObstruction;
}



public enum CellDirection
{
    Right = 0,
    Left = 1,
    Front = 2,
    Back = 3,
    Above = 4,
    Below = 5,
    DirectionCount = 6,
}