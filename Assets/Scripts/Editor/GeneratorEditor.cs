using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Generator))]
public class GeneratorEditor : Editor
{
    private int _CurrentIndex = 1;

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
        EditorGUILayout.Space(10);
        GUILayout.Label("Examples");
        if (GUILayout.Button("Generate Example"))
        {
            generator.GenerateExample(_CurrentIndex);
            _CurrentIndex++;
        }

        for(int i =0; i < _CurrentIndex;i++)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Example "+(i+1)))
            {
                generator.Example(i);
            }
            if(GUILayout.Button("Clear Example " + (i+1)))
            {
                generator.ClearExample(i);
                _CurrentIndex--;
            }
            GUILayout.EndHorizontal();
        }
       
    }

}
