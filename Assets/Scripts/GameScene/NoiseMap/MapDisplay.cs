using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRender;
    public Renderer meshTextureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public string meshName;
    //float[,] 存储数的矩阵
    public void DrawTexture(Texture2D texture)
    {
        
        //sharedMaterial在Editor中可直接看到,不需要进入play
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height);
        
    }

    public void DrawMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        //meshRenderer.sharedMaterial.mainTexture = texture;
        meshFilter.transform.localScale = Vector3.one * FindObjectOfType<MapGenerator>().terrainData.uniformScale;
    }

#if UNITY_WEBGL && !UNITY_EDITOR
[System.Runtime.InteropServices.DllImport("__Internal")]
private static extern void SaveFileWebGL(string filename, string data);
#endif

    public void DownloadMeshToDisk()
    {
        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            Debug.LogWarning("No mesh to download.");
            return;
        }

        string objContent = MeshToObj(mesh);
        string fileName = string.Format("GeneratedMesh{0}.obj",Random.Range(1000000,10000000));

#if UNITY_WEBGL && !UNITY_EDITOR
    SaveFileWebGL(fileName, objContent);
#else
        System.IO.File.WriteAllText(System.IO.Path.Combine(Application.dataPath, fileName), objContent);
#endif
    }
    private string MeshToObj(Mesh mesh)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        foreach (Vector3 v in mesh.vertices)
            sb.AppendLine($"v {v.x} {v.y} {v.z}");

        foreach (Vector3 n in mesh.normals)
            sb.AppendLine($"vn {n.x} {n.y} {n.z}");

        foreach (Vector2 uv in mesh.uv)
            sb.AppendLine($"vt {uv.x} {uv.y}");

        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int i1 = triangles[i] + 1;
            int i2 = triangles[i + 1] + 1;
            int i3 = triangles[i + 2] + 1;
            sb.AppendLine($"f {i1}/{i1}/{i1} {i2}/{i2}/{i2} {i3}/{i3}/{i3}");
        }

        return sb.ToString();
    }
    public void LoadMeshFromDisk()
    {

    }

}
