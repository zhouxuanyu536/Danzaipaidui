using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarMono : MonoBehaviour
{
    public int rank;
    public float progress;
    public float MaxProgress;
    public Image playerNum;
    public Image playerColor;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerProgress;
    [SerializeField] private Sprite DefaultPlayerNum;
    [SerializeField, Range(0, 1)] private float transparency;
    public LayoutElement playerNumLayoutElement;

    public Dictionary<ulong, string> allPlayersNamesSO;
    public int lastRank;
    public float lastRankTime;

    private ulong ownerClientId;

    public PlayerController playerController;

    private int finishedPlayer => GameManager.Instance.playerFinishedRace;
    public bool CanUpdate;
    float startTime;
    string fixedProgressText = "";

    private bool isSpawned;
    private ProgressBars progressBars;
    // Start is called before the first frame update
    void Awake()
    {
        progressBars = FindObjectOfType<ProgressBars>();    
        playerNum.sprite = DefaultPlayerNum;
        playerColor.color = new Color(0, 0, 0, 1 - transparency);
        lastRank = -1;
        lastRankTime = Time.time;
        MaxProgress = 0f;
        allPlayersNamesSO = new Dictionary<ulong, string>();
        CanUpdate = true;
        startTime = Time.time;
    }


    private void Update()
    {
        playerNum.sprite = progressBars.allNumTextures[rank];
    }
    public void SetPlayerRank(int rank)
    {
        this.rank = rank;
    }
    
    public void SetFixedProgress(string progressText)
    {
        CanUpdate = false;
        fixedProgressText = progressText;
        playerProgress.text = progressText;
    }
    
    void UpdateProgressBar()
    {
        if (!CanUpdate) { return; }
        if (!playerProgress.text.Contains("%"))
        {
            CanUpdate = false;
            return;
        }
        playerProgress.text = $"{MaxProgress:F1}%"; //保留一位小数

        if (MaxProgress == 100f)
        {
            //统计前面有多少人完成了比赛
            lastRankTime = -10000 + finishedPlayer;
            GameManager.Instance.playerFinishedRace += 1;
        }

    }
    public void SetPlayerNameByPlayerController(PlayerController playerController)
    {
        if (playerController != null)
        {

            playerNameText.text = playerController.playerName;
            //allPlayersNamesSO[playerController.OwnerClientId]
            //    = playerNameText.text;

            //Debug.Log("allPlayersNamesSO:" + allPlayersNamesSO[playerController.OwnerClientId]);
            //ownerClientId = playerController.OwnerClientId;
            //GetPlayerNameServerRpc(ownerClientId);

        }
        else
        {
            //playerNameText.text = playerController.playerName;
        }

    }

    public string GetPlayerName()
    {
        return playerController.playerName;
    }
   



}
