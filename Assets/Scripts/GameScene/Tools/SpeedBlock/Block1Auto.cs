using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block1Auto :  MonoBehaviour
{
    [SerializeField]
    private Material ShaderMaterial;
    private float progress = 0f;
    // Start is called before the first frame update
    void Start()
    {
        if (ShaderMaterial != null)
        {
            ShaderMaterial.SetFloat("_Density", 6);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isPaused && !GameManager.Instance.GameOnLoad)
        {
            return;
        }
        if (ShaderMaterial != null)
        {
            progress += Time.deltaTime;
            ShaderMaterial.SetFloat("_Progress", progress);
        }
    }
}
