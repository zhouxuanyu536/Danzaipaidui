using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading;
using System;

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}
public struct MapData
{
    public readonly float[,] heightMap;
    //public readonly Color[] colorMap;

    public MapData(float[,] heightMap)
    {
        this.heightMap = heightMap;
        //this.colorMap = colorMap;
    }
}
public class MapGenerator : MonoBehaviour
{
    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
    public enum DrawMode
    {
        NoiseMap,Mesh,FalloffMap
    };
    public DrawMode drawMode;


    public TerrainData terrainData;
    public NoiseData noiseData;

    [Range(0, 6)]
    public int editorPreviewLOD;

    public bool autoUpdate;

    //public TerrainType[] regions;
    //static MapGenerator instance;
    float[,] falloffMap;
    

    public TextureData textureData;

    public Material terrainMaterial;
    private MeshData meshData;

    private MapDisplay display;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

    private void Awake()
    {
        textureData.ApplyToMaterial(terrainMaterial);
        MapDisplay mapDisplay = transform.GetComponent<MapDisplay>();
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight, mapDisplay.meshTextureRenderer.transform.position.y);
        if (terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }
        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
        //falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }

    private void OnValidate()
    {
        if (terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }
        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
            noiseData.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
        //falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }
    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);

    }
    public int mapChunkSize
    {
        get
        {
            if (terrainData.useFlatShading)
            {
                return 95;
            }
            else
            {
                return 239;
            }
        }
    }
    public void DrawMapInEditor()
    {
        MapDisplay mapDisplay = transform.GetComponent<MapDisplay>();
        textureData.UpdateMeshHeights(terrainMaterial, terrainData.minHeight, terrainData.maxHeight, mapDisplay.meshTextureRenderer.transform.position.y);
        
        MapData mapData = GenerateMapData(Vector2.zero);
        display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            //textureData = TextureGenerator.TextureFromHeightMap(mapData.heightMap);
            //display.DrawTexture(textureData);
        }
        //else if (drawMode == DrawMode.ColorMap)
        //{
        //    textureData = TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize);
        //    display.DrawTexture(textureData);
        //}
        else if (drawMode == DrawMode.Mesh)
        {
            meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, editorPreviewLOD, terrainData.useFlatShading);
            //textureData = TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize);
            display.DrawMesh(meshData);
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapChunkSize)));
        }
    }
    public void RequestMapData(Vector2 center,Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center,callback);
        };
        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 center,Action<MapData> callback) 
    { 
        MapData mapData = GenerateMapData(center);
        lock (mapDataThreadInfoQueue)
        {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback,mapData));
        }
        
        
    
    
    }
    public void RequestMeshData(MapData mapData,int lod,Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData,lod,callback);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData,int lod,Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, lod, terrainData.useFlatShading);
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }
        
    }


    private void Update()
    {
        
        if (mapDataThreadInfoQueue.Count > 0)
        {
            for(int i = 0;i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                try
                {
                    threadInfo.callback(threadInfo.parameter);
                }
                catch(Exception e)
                {
                    mapDataThreadInfoQueue.Enqueue(threadInfo);
                }
            }
        }
        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                try
                {
                    threadInfo.callback(threadInfo.parameter);
                }
                catch (Exception e)
                {
                    meshDataThreadInfoQueue.Enqueue(threadInfo);
                }
            }
        }
    }
    MapData GenerateMapData(Vector2 center)
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize + 2, mapChunkSize + 2, noiseData.seed, noiseData.noiseScale, noiseData.octaves, noiseData.persistance, noiseData.lacunarity,center + noiseData.offset, noiseData.normalizeMode);

        if (terrainData.useFalloff)
        {
            if(falloffMap == null)
            {
                falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize + 2);
            }
            //Color[] colorMap = new Color[mapChunkSize * mapChunkSize];
            for (int y = 0; y < mapChunkSize + 2; y++)
            {
                for (int x = 0; x < mapChunkSize + 2; x++)
                {
                    if (x >= mapChunkSize || y >= mapChunkSize)
                    {
                        noiseMap[x, y] = 0;
                        continue;
                    }
                    if (terrainData.useFalloff)
                    {
                        noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                    }

                }//float currentHeight = noiseMap[x,y];
                 //for (int i = 0; i < regions.Length; i++)
                 //{
                 //    if(currentHeight >= regions[i].height)
                 //    {
                 //        colorMap[y * mapChunkSize + x] = regions[i].color;
                 //    }
                 //    else
                 //    {
                 //        break;
                 //    }

                //}
            }
        }
        
        return new MapData(noiseMap);
        
    }
    void SaveMesh(Mesh mesh, string filename)
    {
        string filePath = Path.Combine(Application.dataPath, filename);

        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            // 保存顶点
            writer.Write(mesh.vertices.Length);
            foreach (Vector3 vertex in mesh.vertices)
            {
                writer.Write(vertex.x);
                writer.Write(vertex.y);
                writer.Write(vertex.z);
            }

            // 保存三角形索引
            writer.Write(mesh.triangles.Length);
            foreach (int index in mesh.triangles)
            {
                writer.Write(index);
            }

            // 保存 UV
            writer.Write(mesh.uv.Length);
            foreach (Vector2 uv in mesh.uv)
            {
                writer.Write(uv.x);
                writer.Write(uv.y);
            }

   
        }
    }
    public void SaveTexture()
    {
        if (display == null) return;
        
        if (drawMode != DrawMode.Mesh)
        {
            if (display.textureRender != null)
            {
                List<Material> materials = new List<Material>();
                display.textureRender.GetMaterials(materials);
                foreach (Material material in materials)
                {
                    //material.SetTexture("_MainTex", textureData);
                }
                //重新赋值给Renderer,否则修改不会生效
                display.textureRender.sharedMaterials = materials.ToArray();
            }
            else return;
            string filePath = Application.dataPath + "./Textures/SavedTexture.png";
            //byte[] bytes = textureData.EncodeToPNG(); // 或 texture.EncodeToJPG(100)
            //File.WriteAllBytes(filePath, bytes);
        }
        else
        {
            if (display.meshTextureRenderer != null)
            {
                List<Material> materials = new List<Material>();
                display.meshTextureRenderer.GetMaterials(materials);
                foreach (Material material in materials)
                {
                    //material.SetTexture("_MainTex", textureData);
                }
                //重新赋值给Renderer,否则修改不会生效
                display.meshTextureRenderer.sharedMaterials = materials.ToArray();
            }

            string filePath = Application.dataPath + "./Textures/SavedMeshTexture.png";
            //byte[] bytes = textureData.EncodeToPNG(); // 或 texture.EncodeToJPG(100)
            //File.WriteAllBytes(filePath, bytes);
        }
    }
    private void OnDestroy()
    {
        if (terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
        }
        if (noiseData != null)
        {
            noiseData.OnValuesUpdated -= OnValuesUpdated;
        }
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
        }
    }
}

