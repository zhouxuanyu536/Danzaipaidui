using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class SpeedBlockScaleWatcherSpeed : MonoBehaviour
{
    private Material ShaderMaterial;
    public float speed = 1.0f;
    private float progress;
    private void Start()
    {
        progress = 0f;
        ShaderMaterial = GetComponent<Renderer>().sharedMaterials.FirstOrDefault();
    }
    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isPaused && !GameManager.Instance.GameOnLoad)
        {
            return;
        }
        progress += Time.deltaTime;
        ShaderMaterial.SetFloat("_Offset", progress * speed);
    }
}
