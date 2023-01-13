using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ExampleObject", order = 2)]
public class ExampleObject : ScriptableObject
{
    public int Rows = 10;
    public int Columns = 10;
    public int Levels = 3;

    public float WallWeight = 0.48f;
    public float StairWeight = 0.6f;
    public float HallwayWeight = 0.7f;
    public float CornerWeight = 0.48f;
}
