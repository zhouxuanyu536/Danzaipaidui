using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class ShowLevel : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image bgImage;
    private bool CanShowLevel;
    private int oldLevel;
    private Tween changeLevelTween;
    private void Awake()
    {
        CanShowLevel = false;
    }

    public override void OnNetworkSpawn()
    {
        CanShowLevel = true;
        oldLevel = GameMultiplayer.Instance.level.Value;
        bgImage.color = Color.white;
    }

    private void Update()
    {
        if (CanShowLevel)
        {
            ChangeLevel(GameMultiplayer.Instance.level.Value);
        }
    }
    private void ChangeLevel(int newVal)
    {
        if (CanShowLevel)
        {
            string level;
            switch (newVal) 
            {
                case 1:level = "一";break;
                case 2:level = "二";break;
                case 3:level = "三";break;
                case 4:level = "四";break;
                case 5:level = "五";break;
                case 6:level = "六";break;
                case 7:level = "七";break;
                case 8:level = "八";break;
                default:level = "一";break;
            }
            levelText.text = $"第{level}关";
            if(newVal != oldLevel) 
            {
                oldLevel = newVal;
                ChangeColor();
            }
        }
    }
    private void ChangeColor()
    {
        changeLevelTween?.Kill();
        bgImage.color = new Color(255f / 255f, 187f / 255f, 173f / 255f);
        changeLevelTween = bgImage.DOColor(Color.white,0.4f);
    }
}
