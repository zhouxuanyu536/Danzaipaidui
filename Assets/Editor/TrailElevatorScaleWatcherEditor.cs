using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(TrailElevatorScaleWatcher))]
public class TrailElevatorScaleWatcherEditor : Editor
{
    private Vector3 lastScale;
    private MeshRenderer meshRenderer;

    private void OnEnable()
    {
        TrailElevatorScaleWatcher scaleWatcher = target as TrailElevatorScaleWatcher;
        if (scaleWatcher != null)
        {
            Transform transform = scaleWatcher.transform;
            lastScale = transform.localScale;
            meshRenderer = transform.GetComponent<MeshRenderer>();
        }
    }

    public override void OnInspectorGUI()
    {
        

        DrawDefaultInspector();
        TrailElevatorScaleWatcher scaleWatcher = target as TrailElevatorScaleWatcher ;

        if(scaleWatcher == null)
        {
            return;
        }
        Transform transform = scaleWatcher.transform;

        //因为hlsl的bug,先加上这段代码
        if(transform.localScale != lastScale)
        {
            transform.localScale = new Vector3(
                Mathf.Round(transform.localScale.x),
                Mathf.Round(transform.localScale.y),
                Mathf.Round(transform.localScale.z)
            );
        }
        //结束
        if (meshRenderer != null && transform.localScale != lastScale)
        {
            Material[] materials = meshRenderer.sharedMaterials;
            foreach(Material material in materials)
            {
                material.SetVector("_scale", new Vector2(transform.localScale.x, transform.localScale.z));
            }
            meshRenderer.sharedMaterials = materials.ToArray();
            lastScale =transform.localScale;
        }
    }
}
