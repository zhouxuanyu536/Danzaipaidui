using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;



public class SpeedBlockAutoGenerateEditor : EditorWindow
{
    private GUIStyle TextStyle;

    private SpeedBlockType type;
    private SpeedBlockDirection direction;
    private GameObject LevelGameObject;
    private float speed;
    [SerializeField] private Vector3 Position;
    [SerializeField] private Vector3 Scale;
    private SpeedBlockInformation information;
    private GameObject SpeedUpPrefab;
    private GameObject SpeedDownPrefab;
    private Material SpeedUpMaterialLeft;
    private Material SpeedUpMaterialRight;
    private Material SpeedUpMaterialForward;
    private Material SpeedUpMaterialBackward;
    private Material SpeedDownMaterialLeft;
    private Material SpeedDownMaterialRight;
    private Material SpeedDownMaterialForward;
    private Material SpeedDownMaterialBackward;
    
    private SerializedObject serializedObject;
    private SerializedProperty positionProperty;
    private SerializedProperty scaleProperty;

    private GameObject SpeedBlock;

    private readonly string SpeedBlockRootPath = "Assets/Materials/SpeedMaterial";
    [MenuItem("工具/速度块生成器")]
    public static void ShowWindow()
    {
        GetWindow<SpeedBlockAutoGenerateEditor>();
    }

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
        positionProperty = serializedObject.FindProperty("Position");
        scaleProperty = serializedObject.FindProperty("Scale");

        Position = new Vector3(0, 0, 20);
        Scale = Vector3.one;
        speed = 1f;
        information = FindFirstObjectByType<SpeedBlockInformation>();
        ReadInformation();
    }
    private void OnGUI()
    {
        //Style
        TextStyle = new GUIStyle(GUI.skin.label)
        {
            fontStyle = FontStyle.Bold,
        };

        GUILayout.Label("创建速度块",TextStyle);
        
        type = (SpeedBlockType)EditorGUILayout.EnumPopup("选择type",type);
        direction = (SpeedBlockDirection)EditorGUILayout.EnumPopup("选择direction", direction);
        LevelGameObject = EditorGUILayout.ObjectField("选择Level物体",LevelGameObject,typeof(GameObject),true) as GameObject;
        if(LevelGameObject == null)
        {
            EditorGUILayout.HelpBox("请选择Level子物体", MessageType.Warning);
        }
        else
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(positionProperty, new GUIContent("位置"));
            EditorGUILayout.PropertyField(scaleProperty, new GUIContent("大小"));
            speed = EditorGUILayout.FloatField("速度",speed);

            serializedObject.ApplyModifiedProperties(); //应用更改


            //检查Level物体下是否有SpeedBlocks,若没有则创建
            if (!HasSpeedBlocks(LevelGameObject))
            {
                GameObject speedBlocks = new GameObject("SpeedBlocks");
                speedBlocks.transform.SetParent(LevelGameObject.transform);
                speedBlocks.transform.localPosition = new Vector3(0, -0.2f, 0);
            }
            if (GUILayout.Button("创建"))
            {
                string saveMaterialPath = $"{SpeedBlockRootPath}/{LevelGameObject.name}";
                int materialInDirectory;
                if (!Directory.Exists(saveMaterialPath))
                {
                    Directory.CreateDirectory(saveMaterialPath);
                    materialInDirectory = 0;
                }
                else
                {
                    materialInDirectory = Directory.GetFiles(saveMaterialPath, "*.mat").Length;
                }

                if (type == SpeedBlockType.Up)
                {
                    Transform speedBlocksTransform = LevelGameObject.transform.Find("SpeedBlocks");
                    SpeedBlock = Instantiate(SpeedUpPrefab, 
                        new Vector3(Position.x * speedBlocksTransform.lossyScale.x,
                                    Position.y * speedBlocksTransform.lossyScale.y,
                                    Position.z * speedBlocksTransform.lossyScale.z) + speedBlocksTransform.position,
                        Quaternion.identity, speedBlocksTransform);
                    SpeedBlock.transform.localScale = new Vector3(Scale.x,Scale.z,Scale.y);
                    SpeedBlock.transform.rotation = Quaternion.Euler(90, 0, 0);
                    MeshRenderer renderer = SpeedBlock.GetComponent<MeshRenderer>();
                    Material material = renderer.sharedMaterial;
                    if (material == null) Debug.LogError("创建失败!");
                    Material materialInstance = null;
                    
                    switch (direction)
                    {
                        case SpeedBlockDirection.Left:
                            materialInstance = SpeedUpMaterialLeft;
                            break;
                        case SpeedBlockDirection.Right:
                            materialInstance = SpeedUpMaterialRight;
                            break;
                        case SpeedBlockDirection.Forward:
                            materialInstance = SpeedUpMaterialForward;
                            break;
                        case SpeedBlockDirection.Backward:
                            materialInstance = SpeedUpMaterialBackward;
                            break;
                    }
                    if (materialInstance != null)
                    {
                        renderer.sharedMaterial = 
                                GetMaterialCopy(materialInstance, saveMaterialPath, materialInDirectory);
                        if (direction == SpeedBlockDirection.Left || direction == SpeedBlockDirection.Right)
                        {
                            renderer.sharedMaterial.SetVector("_scale", new Vector2(Scale.x, 1));
                        }
                        else
                        {
                            renderer.sharedMaterial.SetVector("_scale", new Vector2(1, Scale.z));
                        }

                    }
                    Debug.Log("创建加速块成功!");
                }
                else
                {
                    Transform speedBlocksTransform = LevelGameObject.transform.Find("SpeedBlocks");
                    SpeedBlock = Instantiate(SpeedDownPrefab,
                        new Vector3(Position.x * speedBlocksTransform.lossyScale.x,
                                    Position.y * speedBlocksTransform.lossyScale.y,
                                    Position.z * speedBlocksTransform.lossyScale.z) + speedBlocksTransform.position,
                        Quaternion.identity, speedBlocksTransform);
                    SpeedBlock.transform.localScale = new Vector3(Scale.x, Scale.z, Scale.y);
                    SpeedBlock.transform.rotation = Quaternion.Euler(90,0,0);
                    MeshRenderer renderer = SpeedBlock.GetComponent<MeshRenderer>();
                    Material material = renderer.sharedMaterial;
                    if (material == null) Debug.LogError("创建失败!");
                    Material materialInstance = null;
                    switch (direction)
                    {
                        case SpeedBlockDirection.Left:
                            materialInstance = SpeedDownMaterialLeft;
                            break;
                        case SpeedBlockDirection.Right:
                            materialInstance = SpeedDownMaterialRight;
                            break;
                        case SpeedBlockDirection.Forward:
                            materialInstance = SpeedDownMaterialForward;
                            break;
                        case SpeedBlockDirection.Backward:
                            materialInstance = SpeedDownMaterialBackward;
                            break;
                    }
                    if (materialInstance != null)
                    {
                        renderer.sharedMaterial =
                                GetMaterialCopy(materialInstance, saveMaterialPath, materialInDirectory);
                        if (direction == SpeedBlockDirection.Left || direction == SpeedBlockDirection.Right)
                        {
                            renderer.sharedMaterial.SetVector("_scale", new Vector2(Scale.x, 1));
                        }
                        else
                        {
                            renderer.sharedMaterial.SetVector("_scale", new Vector2(1, Scale.z));
                        }
                    }
                    Debug.Log("创建减速块成功!");
                }
                SpeedBlock speedBlock = SpeedBlock.GetComponent<SpeedBlock>();
                //创建边界子物体
                GameObject collider1 = new GameObject("collider1");
                collider1.transform.SetParent(SpeedBlock.transform);
                collider1.transform.localPosition = new Vector3(0, 0.5f,-0.5f);
                collider1.AddComponent<BoxCollider>();
                speedBlock.SetSpeedBlockType(type);
                speedBlock.SetSpeedBlockDirection(direction);
                SpeedBlockScaleWatcherSpeed scaleWatcherSpeed = speedBlock.GetComponent<SpeedBlockScaleWatcherSpeed>();
                scaleWatcherSpeed.speed = speed;
            }
        
        }
    }
    private Material GetMaterialCopy(Material material,string saveMaterialPath,
        int materialInDirectory)
    {
        Material createdMaterial = new Material(material);
        AssetDatabase.CreateAsset(createdMaterial, $"{saveMaterialPath}/" +
            $"Material_{materialInDirectory + 1}.mat");
        Debug.Log($"{createdMaterial.name}已成功保存至{saveMaterialPath}/" +
            $"Material_{materialInDirectory + 1}.mat");
        return createdMaterial;
    }
    private bool HasSpeedBlocks(GameObject obj)
    {
        foreach(Transform child in obj.transform)
        {
            if(child != null && child.name == "SpeedBlocks")
            {
                return true;
            }
        }
        return false;
    }
    private void ReadInformation()
    {
        SpeedUpPrefab = information.SpeedUpPrefab;
        SpeedDownPrefab = information.SpeedDownPrefab;
        SpeedUpMaterialLeft = information.SpeedUpMaterialLeft;
        SpeedUpMaterialRight = information.SpeedUpMaterialRight;
        SpeedUpMaterialForward = information.SpeedUpMaterialForward;
        SpeedUpMaterialBackward = information.SpeedUpMaterialBackward;
        SpeedDownMaterialLeft = information.SpeedDownMaterialLeft;
        SpeedDownMaterialRight = information.SpeedDownMaterialRight;
        SpeedDownMaterialForward = information.SpeedDownMaterialForward;
        SpeedDownMaterialBackward = information.SpeedDownMaterialBackward;
    }
}
