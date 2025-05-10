using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class LevelGenerateEditor : EditorWindow
{
    private GameObject selectedLevel;
    private GameObject selectedPrefab;
    private string newPrefabName = "Level 1";

    private int selectedPrefabIndex = 0; //下拉框的索引
    private string[] prefabNames;
    private readonly string SAVELEVELPATH = "Assets/Resources/Prefabs/Levels/";

    private SerializedObject serializedObject;
    [SerializeField] private Vector3 GeneratePosition;
    private SerializedProperty PositionProperty;
    private GameObject player;
    // Start is called before the first frame update
    [MenuItem("Level编辑器/编辑")]
    public static void ShowWindow()
    {
        GetWindow<LevelGenerateEditor>("Level编辑器");
    }

    private void OnEnable()
    {
        //对象处于已启动 活跃状态调用此函数
        LoadLevel();
        serializedObject = new SerializedObject(this);
        PositionProperty = serializedObject.FindProperty("GeneratePosition");
    }

    private void OnGUI()
    {
        GUILayout.Label("创建新关卡", EditorStyles.boldLabel);

        newPrefabName = EditorGUILayout.TextField("关卡名称", newPrefabName);
        selectedLevel = EditorGUILayout.ObjectField("关卡GameObject", selectedLevel,typeof(GameObject),true) as GameObject;   
        if (GUILayout.Button("创建新关卡"))
        {
            CreateNewPrefab(selectedLevel);
        }
        
        GUILayout.Space(50);
        GUILayout.Label("加载关卡", EditorStyles.boldLabel);
        
        if(prefabNames != null && prefabNames.Length > 0)
        {
            selectedPrefabIndex = EditorGUILayout.Popup("选择关卡预制体", selectedPrefabIndex, prefabNames);
            serializedObject.Update();
            EditorGUILayout.PropertyField(PositionProperty, new GUIContent("位置"));
            serializedObject.ApplyModifiedProperties(); //应用更改
            player = EditorGUILayout.ObjectField("玩家", player, typeof(GameObject), true) as GameObject;
            if (GUILayout.Button("加载关卡"))
            {
                LoadLevelPrefab(prefabNames[selectedPrefabIndex], player);
            }
        }
        else
        {
            GUILayout.Label("没有找到Prefab资源");
        }

        GUILayout.Label("预制体编辑器", EditorStyles.boldLabel);
        selectedPrefab = EditorGUILayout.ObjectField("预制体", selectedPrefab, typeof(GameObject), false) as GameObject;

        if (selectedPrefab == null)
        {
            EditorGUILayout.HelpBox("请选择一个预制体", MessageType.Warning);
            return;
        }
        
        //判断是否是Prefab资源 
        string assetPath = AssetDatabase.GetAssetPath(selectedPrefab);
        bool isPrefab = PrefabUtility.IsPartOfPrefabAsset(selectedPrefab);

        if (!isPrefab)
        {
            EditorGUILayout.HelpBox("请选择一个预制体,而不是场景中的对象！", MessageType.Error);
            return;
        }

        //打开Prefab按钮

        if (GUILayout.Button("打开预制体"))
        {
            AssetDatabase.OpenAsset(selectedPrefab);
        }

        //“应用更改”按钮
        if (GUILayout.Button("应用更改"))
        {
            PrefabUtility.SavePrefabAsset(selectedPrefab);
            AssetDatabase.Refresh();
            Debug.Log("预制体变更已保存！");

        }
    }
    private void LoadLevel()
    {
        if(Directory.Exists(SAVELEVELPATH))
        {
            // 强制刷新 Unity 资源数据库
            AssetDatabase.Refresh();

            string[] prefabPaths = Directory.GetFiles(SAVELEVELPATH, "*.prefab", SearchOption.TopDirectoryOnly);
            prefabNames = prefabPaths.Select(p => Path.GetFileNameWithoutExtension(p)).ToArray();
        }
        else
        {
            prefabNames = new string[0];
        }
    }
    private void CreateNewPrefab(GameObject obj){
        string path = SAVELEVELPATH + newPrefabName + ".prefab";

        //检查是否已存在
        if(AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        {
            Debug.LogError("Prefab已存在，请更换名称");
            return;
        }
        GameObject newObject;
        if (obj != null)
        {
            newObject = obj;
        }
        else
        {
            newObject = new GameObject(newPrefabName);
        }
        PrefabUtility.SaveAsPrefabAsset(newObject, path);
        //DestroyImmediate(newObject);

        AssetDatabase.Refresh();
        Debug.Log("Prefab创建成功：" + path);
    }

    private void LoadLevelPrefab(string prefabName,GameObject player = null)
    {
        string path = SAVELEVELPATH + prefabName + ".prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if(prefab != null)
        {
            Instantiate(prefab,GeneratePosition,Quaternion.identity);
            if(player != null)
            {
                player.transform.position = GeneratePosition;
                player.transform.rotation = Quaternion.identity;
            }
            Debug.Log("已加载关卡：" + prefabName);
        }
        else
        {
            Debug.LogError("关卡未加载成功：" + prefabName);
        }
    }

}
