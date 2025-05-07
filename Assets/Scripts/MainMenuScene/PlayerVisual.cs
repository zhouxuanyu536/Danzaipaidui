using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public enum PlayerVisualColor
{
    Yellow,
    Red,
    Blue,
    Green,
    Magenta,
    White,
    Null
}
public class PlayerVisual : MonoBehaviour
{
    private PlayerComp comp;
    [SerializeField] private Material headMaterial;
    [SerializeField] private Material ArmAndLegMaterial;
    [SerializeField] private Material EyeMaterial;

    public PlayerVisualColor color;
    private PlayerVisualColor lastColor;
    // Start is called before the first frame update
    void Start()
    {
        comp = GetComponent<PlayerComp>();
        SetMaterials(GetColor(color));
    }

    // Update is called once per frame
    void Update()
    {
        if (color != lastColor)
        {
            SetMaterials(GetColor(color));
            lastColor = color;
        }
    }
    private void SetMaterials(Color color)
    {
        comp.Head.GetComponent<Renderer>().material = headMaterial;
        comp.Head.GetComponent<Renderer>().material.SetColor("_Color", color);
        foreach (Transform c in comp.ArmsAndLegs)
        {
            c.GetComponent<Renderer>().material = ArmAndLegMaterial;
            c.GetComponent<Renderer>().material.color = color;
        }
        foreach (Transform e in comp.Eyes)
        {
            e.GetComponent<Renderer>().material = EyeMaterial;
            if (color.r > 1)
            {
                color.r = color.r / 255;
                color.g = color.g / 255;
                color.b = color.b / 255;
            }
            if (color == Color.red || color == Color.blue)
                e.GetComponent<Renderer>().material.color = Color.white;
            else
                e.GetComponent<Renderer>().material.color = Color.black;
        }
    }
    private Color GetColor(PlayerVisualColor color)
    {
        switch (color)
        {
            case PlayerVisualColor.Yellow:
                return Color.yellow;
            case PlayerVisualColor.Red:
                return Color.red;
            case PlayerVisualColor.Blue:
                return Color.blue;
            case PlayerVisualColor.Green:
                return Color.green;
            case PlayerVisualColor.Magenta:
                return Color.magenta;
            case PlayerVisualColor.White:
                return Color.white; 
            default:
                return Color.yellow;
        }
    }
    
    public void SetVisualColorFromIndex(int index)
    {
        if (index >= 0 && index < Enum.GetValues(typeof(PlayerVisualColor)).Length)
        {
            color = (PlayerVisualColor)index;
        }
        else
        {
            color = PlayerVisualColor.Yellow; // 默认值
        }
    }
    
}
