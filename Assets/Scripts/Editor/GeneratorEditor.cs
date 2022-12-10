using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Generator generator = (Generator)target;
        if (GUILayout.Button("Clear"))
        {
            generator.ClearGrid();
        }

        if (GUILayout.Button("Collapse"))
        {
            generator.CollapseGrid();
        }

   
    }

}
