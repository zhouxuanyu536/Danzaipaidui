using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpeedBlockScaleWatcherSpeed))]
public class SpeedBlockScaleWatcherSpeedEditor : Editor
{
    private Vector3 lastScale;
    private MeshRenderer meshRenderer; // 存储目标物体的 MeshRenderer

    private void OnEnable()
    {
        SpeedBlockScaleWatcherSpeed scaleWatcher = target as SpeedBlockScaleWatcherSpeed;
        if (scaleWatcher != null)
        {
            Transform transform = scaleWatcher.transform;
            lastScale = transform.localScale;
            meshRenderer = transform.GetComponent<MeshRenderer>(); // 获取 MeshRenderer
        }


    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpeedBlockScaleWatcherSpeed scaleWatcher = target as SpeedBlockScaleWatcherSpeed;
        if (scaleWatcher == null)
        {
            return;
        }
        Transform transform = scaleWatcher.transform;

        if (meshRenderer != null && transform.localScale != lastScale)
        {

            // **找到目标材质**
            Material[] materials = meshRenderer.sharedMaterials;
            foreach (Material mat in materials)
            {
                //Left Down Up Right
                Vector2 scale = mat.GetVector("_scale");
                if(mat.GetInt("_DIRECTION") == 1 || mat.GetInt("_DIRECTION") == 2)
                {
                    mat.SetVector("_scale", new Vector2(scale.x, transform.localScale.y));
                }
                else
                {
                    mat.SetVector("_scale", new Vector2(transform.localScale.x, scale.y));
                }

                
            }
            meshRenderer.sharedMaterials = materials.ToArray();
            lastScale = transform.localScale; // 记录新 Scale
        }
    }
}