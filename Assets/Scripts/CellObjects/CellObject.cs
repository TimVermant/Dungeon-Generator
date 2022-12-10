using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CellObject", order = 1)]
public class CellObject : ScriptableObject
{
    public int Identifier; // Used to easily identify each scriptable object
    public GameObject CellPrefab;

    // Using an enum to get the correct list of cellobjects to see whats allowed
    public List<CellObjectNeighbourRules> AllowedNeighbourCell;
}

[Serializable]
public class CellObjectNeighbourRules
{
    public CellDirection Direction;
    public List<CellObject> PossibleVariations;
}

public enum CellDirection
{
    Right = 0,
    Left = 1,
    Front = 2,
    Back = 3,

}