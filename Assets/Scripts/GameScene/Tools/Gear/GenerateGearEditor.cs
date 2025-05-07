//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEditor;
//using UnityEngine;

//public class GenerateGearEditor : EditorWindow
//{
//    public int teeth = 12;
//    public float radius = 1f; // 齿轮半径
//    public float toothDepth = 0.2f; // 齿的深度
//    public GameObject selectedGameObject;
//    [MenuItem("工具/齿轮生成器")]
//    public static void ShowWindow()
//    {
//        GetWindow<GenerateGearEditor>();
//    }
//    private void OnGUI()
//    {
//        GUILayout.Label("创建齿轮");
//        string teethNumStr = EditorGUILayout.TextField("齿数量","12");
//        int.TryParse(teethNumStr,out teeth);
//        string RadiusStr = EditorGUILayout.TextField("齿轮半径","1");
//        float.TryParse(RadiusStr,out radius);
//        string toothDepthStr = EditorGUILayout.TextField("齿的深度","0.2");
//        float.TryParse(toothDepthStr,out toothDepth);
//        selectedGameObject = EditorGUILayout.ObjectField("预制体", selectedGameObject, typeof(GameObject), true) as GameObject;
//        if (selectedGameObject == null)
//        {
//            EditorGUILayout.HelpBox("请选择一个预制体", MessageType.Warning);
//            return;
//        }
//        if (!selectedGameObject.GetComponent<MeshFilter>())
//        {
//            selectedGameObject.AddComponent<MeshFilter>();
//        }
//        if (GUILayout.Button("生成齿轮"))
//        {
//            GenerateGear.Generate(selectedGameObject.GetComponent<MeshFilter>(), teeth, radius, toothDepth) ;
//        }
//    }
//}
