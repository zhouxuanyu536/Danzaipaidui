using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SpringWatcher : MonoBehaviour
{
    public float springJumpHeight;
    private Material material;
    [SerializeField] private Color Color1;
    [SerializeField] private Color Color2;
    [SerializeField] private Color Color3;
    [SerializeField] private float ChangeSpeed;

    private Sequence sequence;


    private void Awake()
    {
        if (springJumpHeight == 0)
        {
            springJumpHeight = 30f; //默认值
        }
        material = GetComponent<Renderer>().sharedMaterial;
        if(ChangeSpeed <= 0)
        {
            ChangeSpeed = 1f;
        }
        ChangeMaterialColor();
    }
    private void OnValidate()
    {
        material = GetComponent<Renderer>().sharedMaterial;
        material.color = Color1;
    }
    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused && !GameManager.Instance.GameOnLoad)
        { 
            sequence.timeScale = 0f;
        }
        else
        {
            sequence.timeScale = 1f;
        }
    }
    private void ChangeMaterialColor()
    {
        float ChangeInterval = 1f / ChangeSpeed;
        sequence = DOTween.Sequence();
        sequence.Append(material.DOColor(Color2, ChangeInterval))
            .Append(material.DOColor(Color3, ChangeInterval))
            .Append(material.DOColor(Color1, ChangeInterval))
            .SetLoops(-1);
    }
    private void OnDestroy()
    {
        sequence.Kill();
    }
}
