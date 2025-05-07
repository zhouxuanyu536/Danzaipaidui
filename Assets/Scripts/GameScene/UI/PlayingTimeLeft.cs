using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayingTimeLeft : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI Level;
    [SerializeField] private TextMeshProUGUI timeLeft;
    [SerializeField] private Image bg;

    private RectTransform rectTransform;
    private bool isShowTimeLeft;
    private NetworkVariable<float> hasTimeLeft = new NetworkVariable<float>(1);
    private NetworkVariable<float> allTimeLeft = new NetworkVariable<float>(1);
    // Start is called before the first frame update
    void Awake()
    {
        
        rectTransform = GetComponent<RectTransform>();
    }

    public override void OnNetworkSpawn()
    {
        isShowTimeLeft = true;
        GameManager.Instance.OnSceneLoadCompletedEvent += ResetTimeLeft;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (isShowTimeLeft)
        {
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector3(0, 0, 0);
            Level.text = $"第{GameMultiplayer.Instance.level.Value}关";
            SetTimeLeft();
        }   
    }

    private void ResetTimeLeft(object sender,EventArgs e)
    {
        if (!IsServer) return;
        int level = GameMultiplayer.Instance.level.Value;
        Debug.Log("level:" + level);
        if (level == 1)
        {
            //80
            hasTimeLeft.Value = 180;
        }
        else if(level == 2)
        {
            //100
            hasTimeLeft.Value = 300;
        }
        else if(level == 3)
        {
            //120
            hasTimeLeft.Value = 300;
        }
        else if(level == 4)
        {
            //160
            hasTimeLeft.Value = 500;
        }
        allTimeLeft.Value = hasTimeLeft.Value;
    }

    private void SetTimeLeft()
    {
        if (GameManager.Instance == null) return;
        if (IsServer && GameManager.Instance.state.Value == GameManager.State.GamePlaying
            && !GameManager.Instance.finalSettlement.Value)
        {
            hasTimeLeft.Value -= Time.deltaTime;
            if(hasTimeLeft.Value < 0)
            {
                hasTimeLeft.Value = 0;
            }
        }
        int time = Mathf.CeilToInt((int)hasTimeLeft.Value);
        //显示格式
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        timeLeft.text = "剩余时间：" + string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
        //调整颜色
        if (hasTimeLeft.Value / allTimeLeft.Value < 0.25f)
        {
            bg.color = Color.red;
            timeLeft.color = Color.white;
        }
        else
        {
            bg.color = Color.white;
            timeLeft.color = Color.black;
        }
    }

    public bool IsTimeUp()
    {
        return hasTimeLeft.Value <= 0;
    }
    private void OnDestroy()
    {
        GameManager.Instance.OnSceneLoadCompletedEvent -= ResetTimeLeft;
    }


}
