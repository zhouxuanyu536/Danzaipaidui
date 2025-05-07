using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    const int textureSize = 512; //512 * 512
    const TextureFormat textureFormat = TextureFormat.RGB565;

    public Layer[] layers;
    float savedMinHeight;
    float savedMaxHeight;
    private float relativeHeight;

    public void ApplyToMaterial(Material material)
    {
        material.SetInt("_layerCount",layers.Length);
        Color[] colors = layers.Select(x => x.tint).ToArray();
        for (int i = 0;i < layers.Length; i++)
        {
            material.SetColor(string.Format("_Color_{0}",i), colors[i]);
        }
        float[] startHeights = layers.Select(x => x.startHeight).ToArray();
        for(int i = 0;i < layers.Length; i++)
        {
            material.SetFloat(string.Format("_baseStartHeight_{0}", i), startHeights[i]);
        }
        float[] blendStrengths = layers.Select(x => x.blendStrength).ToArray();
        for(int i = 0;i < layers.Length; i++)
        {
            material.SetFloat(string.Format("_blend_{0}", i), blendStrengths[i]);
        }
        float[] baseColorStrengths = layers.Select(x => x.tintStrength).ToArray();
        for (int i = 0; i < layers.Length; i++)
        {
            material.SetFloat(string.Format("_baseColorStrength_{0}", i), baseColorStrengths[i]);
        }
        float[] baseTextureScales = layers.Select(x => x.textureScale).ToArray();
        for (int i = 0; i < layers.Length; i++)
        {
            material.SetFloat(string.Format("_baseTextureScale_{0}", i), baseTextureScales[i]);
        }

        Texture2DArray texturesArray = GenerateTextureArray(layers.Select(x => x.texture).ToArray());
        material.SetTexture("_baseTextures", texturesArray);
#if UNITY_EDITOR
        AssetDatabase.CreateAsset(texturesArray, "Assets/Scripts/TerrainAssets/Texture2DArray.asset");
        //AssetDatabase.SaveAssets();
#endif
        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight,0);
    }

    Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
        Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
        for(int i = 0;i < textures.Length; i++)
        {
            textureArray.SetPixels(textures[i].GetPixels(), i);
        }
        textureArray.Apply();
        return textureArray;
    }
    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight,float relativeHeight)
    {
        savedMinHeight = minHeight;
        savedMaxHeight = maxHeight;
        if(relativeHeight != 0)
        {
            this.relativeHeight = relativeHeight;
        }
        Debug.Log("heights updated");

        material.SetFloat("_minHeight",minHeight + this.relativeHeight);
        material.SetFloat("_maxHeight",maxHeight + this.relativeHeight);
    }
    [System.Serializable]
    public class Layer
    {
        public Texture2D texture;
        public Color tint;
        [Range(0,1)]
        public float tintStrength;
        [Range(0,1)]
        public float startHeight;
        [Range(0,1)]
        public float blendStrength;
        public float textureScale;
    }
}
