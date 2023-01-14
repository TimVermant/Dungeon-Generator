using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CellObject", order = 1)]
public class CellObject : ScriptableObject
{
    // Prefab variables
    public GameObject CellPrefab;
    public float Rotation = 0;
    // WFC variables
    [Range(0f, 1f)]
    public float Weight = 1f;
    public bool IsStair = false;
    public List<CellObjectNeighbbourDirection> AllowedNeighbourCell = new List<CellObjectNeighbbourDirection>(new CellObjectNeighbbourDirection[(int)CellDirection.DirectionCount]);
    

    // Helper fnctions
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

    [HideInInspector]
    public CellDirection GetOpenDirection()
    {
        foreach (CellObjectNeighbbourDirection cellObjectNeighbourRules in AllowedNeighbourCell)
        {
            if (!cellObjectNeighbourRules.HasObstruction)
            {
                return cellObjectNeighbourRules.Direction;
            }
        }
        return CellDirection.DirectionCount;
    }

    public CellDirection GetOpenDirection2D()
    {
        foreach (CellObjectNeighbbourDirection cellObjectNeighbourRules in AllowedNeighbourCell)
        {
            if (!cellObjectNeighbourRules.HasObstruction && (int)cellObjectNeighbourRules.Direction < 4)
            {
                return cellObjectNeighbourRules.Direction;
            }
        }
        return CellDirection.DirectionCount;
    }
    public bool HasObstruction(CellDirection direction)
    {
        CellObjectNeighbbourDirection cellObjectNeighbourDirection = GetAllowedNeighbourInDirection(direction);
        if(cellObjectNeighbourDirection == null)
        {
            return true;
        }
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