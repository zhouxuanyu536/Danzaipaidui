using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : NetworkBehaviour
{
    public int rank;
    public float progress;
    public float MaxProgress;
    public Image playerNum;
    public Image playerColor;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerProgress;
    [SerializeField] private Sprite DefaultPlayerNum;
    [SerializeField,Range(0,1)] private float transparency;
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

    public bool completeForFirstTime;

    private int finalRank;
    // Start is called before the first frame update
    void Awake()
    {
        if (rank == 0)
            rank = 1;
        playerNum.sprite = DefaultPlayerNum;
        playerColor.color = new Color(0, 0, 0, 1 - transparency);
        lastRank = -1;
        lastRankTime = Time.time;
        MaxProgress = 0f;
        allPlayersNamesSO = new Dictionary<ulong, string>();
        CanUpdate = true;
        startTime = Time.time;
        completeForFirstTime = true;
    }

    public override void OnNetworkSpawn()
    {
        isSpawned = true;
        transform.SetParent(FindObjectOfType<ProgressBars>().transform);
        
    }
    private void OnValidate()
    {
        playerColor.color = new Color(playerColor.color.r, playerColor.color.g
            , playerColor.color.b, 1 - transparency);
    }

    private void Update()
    {
        if (isSpawned && playerController != null &&
            playerController.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            UpdateProgressBarDataServerRpc(progress,MaxProgress, playerController.m_PlayerAnimator.isGrounded);
        }
        transform.localScale = Vector3.one;
        //每隔0.5秒执行
        if (CanUpdate && (Time.time - startTime > 0.5f))
        {
            startTime = Time.time;
            UpdateProgressBar();
        }
        else if(!CanUpdate)
        {
            SetFixedProgress("OK");
        }
    }
    public void SetFixedProgress(string progressText)
    {
        CanUpdate = false;
        fixedProgressText = progressText;
        playerProgress.text = progressText;
    }
    [ServerRpc(RequireOwnership = false)]
    void UpdateProgressBarDataServerRpc(float progress, float maxProgress, bool isGrounded)
    {
        if (progress > maxProgress && isGrounded)
        {
            UpdateProgressBarDataClientRpc(progress);
        }
    }
    [ClientRpc]
    void UpdateProgressBarDataClientRpc(float progress)
    {
        MaxProgress = progress;
    }
    
    void UpdateProgressBar()
    {
        if (!playerProgress.text.Contains("%"))
        {
            CanUpdate = false;
            return;
        }
        playerProgress.text = $"{MaxProgress:F1}%"; //保留一位小数
        
        if(MaxProgress == 100f)
        {
            //统计前面有多少人完成了比赛
            lastRankTime = - 10000 + finishedPlayer;
            GameManager.Instance.playerFinishedRace += 1;
            playerProgress.text = "100%";
            CanUpdate = false;
        }
        
    }
    public void SetPlayerNameByPlayerController(PlayerController playerController)
    {
        if(playerController != null && IsServer)
        {
            
            playerNameText.text = playerController.playerName;
            allPlayersNamesSO[playerController.OwnerClientId]
                = playerNameText.text; 

            //Debug.Log("allPlayersNamesSO:" + allPlayersNamesSO[playerController.OwnerClientId]);
            ownerClientId = playerController.OwnerClientId;
            GetPlayerNameServerRpc(ownerClientId);
            
        }
        else 
        {
            playerNameText.text = playerController.playerName;
        }
        
    }

    public string GetPlayerName()
    {
        return playerController.playerName;
    }
    [ServerRpc(RequireOwnership = false)]
    public void GetPlayerNameServerRpc(ulong clientId, ServerRpcParams serverRpcParams = default)
    {
        if (allPlayersNamesSO.ContainsKey(clientId))
        {
            Debug.Log("allPlayersNamesSO:" + allPlayersNamesSO[clientId]);
            string playerNameOnServer = allPlayersNamesSO[clientId];
            GetPlayerNameClientRpc(clientId,serverRpcParams.Receive.SenderClientId, playerNameOnServer);
        }
        else
        {
            GetPlayerNameClientRpc(clientId,serverRpcParams.Receive.SenderClientId, "player");
        }
    }

    [ClientRpc]
    public void GetPlayerNameClientRpc(ulong clientId,ulong LocalClientId, string playerName)
    {
        allPlayersNamesSO[clientId] = playerName;
        if (LocalClientId == NetworkManager.Singleton.LocalClientId)
        {
            playerNameText.text = playerName;
        }
    }



}
