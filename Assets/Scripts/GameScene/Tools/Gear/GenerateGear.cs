using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GenerateGear
{
    public static void Generate(MeshFilter meshFilter,int teeth,float radius,float toothDepth)
    {
        //    Mesh mesh = new Mesh();
        //    Vector3[] vertices = new Vector3[teeth * 2];
        //    int[] triangles = new int[(teeth - 1) * 6];

        //    float angleStep = 360f / teeth;

        //    for (int i = 0; i < teeth; i++)
        //    {
        //        float angle = i * angleStep * Mathf.Deg2Rad;
        //        float nextAngle = (i + 1) * angleStep * Mathf.Deg2Rad;

        //        // 齿的外边缘
        //        vertices[i * 2] = new Vector3(Mathf.Cos(angle) * (radius + toothDepth), 0, Mathf.Sin(angle) * (radius + toothDepth));
        //        // 齿的内边缘
        //        vertices[i * 2 + 1] = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

        //        if (i < teeth - 1)
        //        {
        //            int start = i * 2;
        //            triangles[i * 6] = start;
        //            triangles[i * 6 + 1] = start + 2;
        //            triangles[i * 6 + 2] = start + 1;

        //            triangles[i * 6 + 3] = start + 1;
        //            triangles[i * 6 + 4] = start + 2;
        //            triangles[i * 6 + 5] = start + 3;
        //        }
        //    }

        //    mesh.vertices = vertices;
        //    mesh.triangles = triangles;
        //    mesh.RecalculateNormals();
        //    if(meshFilter != null)
        //    {
        //        Debug.Log("生成完成");
        //        meshFilter.mesh = mesh;
        //    }

    }
}
