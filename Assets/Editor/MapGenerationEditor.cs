using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MapGenerator))]
public class MapGenerationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.DrawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGen.DrawMapInEditor();
        }
        if(GUILayout.Button("Save Texture"))
        {
            mapGen.DrawMapInEditor();
            mapGen.SaveTexture();
        }
    }
}
