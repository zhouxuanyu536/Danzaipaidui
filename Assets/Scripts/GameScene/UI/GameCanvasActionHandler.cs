using System;
using UnityEngine;
using UnityEngine.UI;

public class GameCanvasActionHandler : MonoBehaviour
{
    [Header("播放和暂停")]
    [SerializeField] private GameObject PauseBg;
    private Image PlayImage;
    public bool isPlaying;
    [Header("加速")]
    public bool isBoost;
    private float OnBoostTime;
    [SerializeField] private BoostCoolDown boostCoolDown;

    [SerializeField]private float boostTime;
    public ProgressBars progressBars;
    [SerializeField] private GameObject Progress;

    
    
    
    private void Start()
    {
        isPlaying = true;
        isBoost = false;
        OnBoostTime = 0f;
        if (boostTime == 0f)
        {
            boostTime = 5f;
        }
        ProgressShow();
        GameManager.Instance.OnMultiplayerGamePaused += OnPlayerPaused;
        GameManager.Instance.OnMultiplayerGameUnpaused += OnMultiplayerUnPaused;
    }
    private void FixedUpdate()
    {
        if (isPlaying)
        {
            ProgressShow();
        }
        else
        {
            ProgressHide();
        }
        BoostHandler();
    }
    private void OnPlayerPaused(object sender,EventArgs e)
    {
        isPlaying = false;
    }

    private void OnMultiplayerUnPaused(object sender,EventArgs e)
    {
        isPlaying = true;
    }
    private void OnDestroy()
    {

    }

    public void SetBoost()
    {
        if(!boostCoolDown.IsBoostCoolingDown())
        {
            isBoost = true;
        }
    }
    private void BoostHandler()
    {
        if (GameManager.Instance.isPaused) return;

        //点击Boost 加速5秒
        if (isBoost)
        {
            if(OnBoostTime < boostTime)
            {
                OnBoostTime += Time.deltaTime;
            }
            else
            {
                OnBoostTime = 0;
                boostCoolDown.StartCoolDown();
                isBoost = false;
            }
        }
        
    }

    private void SetProgressAlpha(float alpha)
    {
        try
        {
            Image ProgressImage = Progress.GetComponent<Image>();
            Color color = ProgressImage.color;
            color.a = Mathf.Clamp01(alpha); // 确保 alpha 在 0-1 之间
            ProgressImage.color = color;
        }
        catch { }
    }
    private void ProgressShow()
    {
        SetProgressAlpha(1); //完全不透明
    }
    private void ProgressHide()
    {
        SetProgressAlpha(0); //完全透明
    }
}
