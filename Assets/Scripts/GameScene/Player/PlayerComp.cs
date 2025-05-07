using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PlayerComp : MonoBehaviour
{
    public Transform Head;
    public Transform[] ArmsAndLegs;
    public Transform[] Eyes;
    public TextMeshProUGUI playerName;
    private void Start()
    {
        SetMaterialAlpha(1);
    }
    public void SetAlpha(float alpha)
    {
        Debug.Log("alpha:" + alpha);
        SetMaterialAlpha(alpha);
    }
    private void SetMaterialAlpha(float alpha)
    {
        Head.GetComponent<Renderer>().material.SetFloat("_Alpha", alpha);
        foreach(Transform armOrLeg in ArmsAndLegs)
        {
            Color color = armOrLeg.GetComponent<Renderer>().material.color;
            armOrLeg.GetComponent<Renderer>().material.color = 
                new Color(color.r,color.g, color.b, alpha);
        }
        foreach(Transform eye in Eyes)
        {
            Color color = eye.GetComponent<Renderer>().material.color;
            eye.GetComponent<Renderer>().material.color =
                new Color(color.r, color.g, color.b, alpha);
        }
        if(playerName != null)
        {
            Color playerNameColor = playerName.color;
            playerName.color = new Color(playerNameColor.r, playerNameColor.g, playerNameColor.b, alpha == 1f ? 1: alpha * 0.2f);
        }
        
    }
}
