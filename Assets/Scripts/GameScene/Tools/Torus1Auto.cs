using UnityEngine;

public class Torus1Auto : MonoBehaviour
{
    [SerializeField]
    private Material ShaderMaterial;
    private float Rotation = 0f;
    [SerializeField]
    private float MoveDelta = 5f;
    // Start is called before the first frame update
    void Start()
    {
        ShaderMaterial = GetComponent<Renderer>().sharedMaterial;
        if (ShaderMaterial != null)
        {
            ShaderMaterial.SetFloat("_Density", 3);
            ShaderMaterial.SetFloat("_BendFactor", 6);
            ShaderMaterial.SetFloat("_Rotation", 0);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused && !GameManager.Instance.GameOnLoad)
        {
            return;
        }
        if (ShaderMaterial != null)
        {
            Rotation += Time.deltaTime * MoveDelta;
            ShaderMaterial.SetFloat("_Rotation", Rotation);
        }
    }
}