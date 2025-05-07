using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProgressBars : NetworkBehaviour
{
    public static ProgressBars Instance;

    [SerializeField] private GameObject progressBar;
    [SerializeField] private GameObject progressBarMono;
    public WayProgress wayProgress;
    public List<Sprite> allNumTextures;
    [SerializeField] private Color GoldColor;
    [SerializeField] private Color SilverColor;
    [SerializeField] private Color BronzeColor;
    [SerializeField] private Color DefaultColor;
    [SerializeField] private GameObject finalPanel;
    [SerializeField] private GameObject finalPanelProgressBars;
    [SerializeField] private PlayingTimeLeft playingTimeLeft;
    
    private List<KeyValuePair<PlayerController, ProgressBar>> savedRankedPairs;

    private Dictionary<ulong, float> allPlayersProgressSO;
    
    private float otherPlayerProgress;

    private ProgressBar localProgressBar;

    private UnityAction changeNameAction;

    private Dictionary<PlayerController, ProgressBar> PlayerBars;

    private bool isRankCreated;

    private bool isFinalRankCreated;

    private int completedPlayers;
    void Awake()
    {
        Instance = this;

        PlayerBars = new Dictionary<PlayerController, ProgressBar>();
        savedRankedPairs = new List<KeyValuePair<PlayerController, ProgressBar>>();
        
        allPlayersProgressSO = new Dictionary<ulong, float>();

        finalPanel.SetActive(false);
        isRankCreated = false;

        isFinalRankCreated = false;
        completedPlayers = 0;
    }

    public override void OnNetworkSpawn()
    {
        
        InvokeRepeating(nameof(CheckPlayerProgress), 0.1f, 0.1f);
        InvokeRepeating(nameof(RankPlayer), 0.1f, Time.deltaTime);
        
        
    }
    private void OnDestroy()
    {
        
        PlayerBars.Clear();
        CancelInvoke(nameof(CheckPlayerProgress));
        CancelInvoke(nameof(RankPlayer));
        
    }
    // Update is called once per frame
    void LateUpdate()
    {
        
    }
    private void CheckPlayerProgress()
    {
        if(wayProgress == null || isFinalRankCreated) { return; }
        foreach (Transform player in PlayerTools.players)
        {
            Debug.Log("allPlayersProgressSO:" + PlayerTools.players.Count + " " + player);
            if (player == null) continue;
            PlayerController playerController = player.GetComponent<PlayerController>();
            if(playerController != null)
            {
                float progress = 0f;
                ulong clientId;
                bool isLocalPlayer = CheckIfLocalPlayer(playerController, out clientId);
                if (isLocalPlayer)
                {
                    Debug.Log("allPlayersProgressSO:" + clientId);
                    if(PlayerBars.ContainsKey(playerController) && PlayerBars[playerController].MaxProgress != 100f)
                    {
                        progress = wayProgress.GetPlayerProgress(playerController);
                    }
                    SetPlayerProgressServerRpc(progress);
                    if (PlayerBars.ContainsKey(playerController))
                    {
                        PlayerBars[playerController].progress = progress;
                        localProgressBar = PlayerBars[playerController];
                    }

                }
                else
                {
                    GetPlayerProgressServerRpc(clientId);
                }
                
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void GetPlayerProgressServerRpc(ulong clientId,ServerRpcParams serverRpcParams = default)
    {
        if (allPlayersProgressSO.ContainsKey(clientId))
        {
            GetPlayerProgressClientRpc(serverRpcParams.Receive.SenderClientId,clientId,allPlayersProgressSO[clientId]);
        }
        else
        {
            GetPlayerProgressClientRpc(serverRpcParams.Receive.SenderClientId,clientId,0f);
        }
    }
    [ClientRpc]
    private void GetPlayerProgressClientRpc(ulong senderClientId,ulong clientId,float progress)
    {
        if (senderClientId == NetworkManager.Singleton.LocalClientId)
        {
            foreach (Transform player in PlayerTools.players)
            {
                try
                {
                    if (player.GetComponent<NetworkObject>().OwnerClientId == clientId)
                    {
                        PlayerController playerController = player.GetComponent<PlayerController>();
                        if (PlayerBars.ContainsKey(playerController))
                        {
                            Debug.Log("senderClientId:" + senderClientId);
                            PlayerBars[playerController].progress = progress;
                        }
                        break;
                    }
                }
                catch { }
                
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerProgressServerRpc(float progress,ServerRpcParams serverRpcParams = default)
    {
        Debug.Log("allPlayersProgressSO:" + serverRpcParams.Receive.SenderClientId + " " + progress);
        allPlayersProgressSO[serverRpcParams.Receive.SenderClientId] = progress;
    }

    private bool CheckIfLocalPlayer(PlayerController playerController,out ulong clientId)
    {
        clientId = playerController.transform.GetComponent<NetworkObject>().OwnerClientId;
        if (clientId != NetworkManager.Singleton.LocalClientId) return false;
        return true;
    }

    private List<KeyValuePair<PlayerController, ProgressBar>> rankedPairs;
    private void RankPlayer()
    {
        //for(int i = savedRankedPairs.Count() - 1;i > 0; i--)
        //{
        //    ProgressBar progressBar = savedRankedPairs[i].Value;
        //    float progress = progressBar.MaxProgress;
        //    float previousProgress = savedRankedPairs[i - 1].Value.MaxProgress;
        //    if (progress <= previousProgress) continue;
            
        //}
        //根据Progress排名 
        //在Progress相同情况下，lastRankTime越小越靠前
         rankedPairs = PlayerBars
         .OrderByDescending(pair => (pair.Value.MaxProgress > 100f ? 100f: pair.Value.MaxProgress))  // 按 progress 降序
         .ThenBy(pair => pair.Value.lastRankTime)         // 相同 progress 时，lastRankTime 小的排前
         .ToList();                                       // 转换为 List<KeyValuePair<PlayerController, ProgressBar>>
        foreach(ProgressBar bar in PlayerBars.Values.ToList())
        {
            Debug.Log("rankedPairs:" + bar.lastRankTime);
        }
        float MinProgress = 110f;
        //在屏幕上显示
        for (int rank = 0;rank < rankedPairs.Count; rank++)
        {
            ProgressBar RankedBar = rankedPairs[rank].Value;
            if(RankedBar.lastRank != rank)
            {
                RankedBar.lastRank = rank;
                RankedBar.lastRankTime = Time.time;
            }
            MinProgress = Mathf.Min(MinProgress, RankedBar.MaxProgress);
        }
        Debug.Log("MinProgress:" + MinProgress);
        if(MinProgress == 100f || playingTimeLeft.IsTimeUp())
        {
             ShowFinalPanel();
        }
        if (isFinalRankCreated) return;
        int rankIndex = 0;
        for (int rank = 0; rank < rankedPairs.Count; rank++)
        {
            ProgressBar RankedBar = rankedPairs[rank].Value;
            RankedBar.rank = rank;
            try
            {
                RankedBar.playerNum.sprite = allNumTextures[rank];
                RankedBar.transform.GetComponent<Image>().color = DefaultColor;
                Transform barTransform = rankedPairs[rank].Value.transform;
                barTransform.SetSiblingIndex(rank);

                //排名对应颜色
                if (rankIndex == 0)
                {
                    RankedBar.transform.GetComponent<Image>().color = GoldColor;
                }
                else if (rankIndex == 1)
                {
                    RankedBar.transform.GetComponent<Image>().color = SilverColor;
                }
                else if (rankIndex == 2)
                {
                    RankedBar.transform.GetComponent<Image>().color = BronzeColor;
                }
                rankIndex += 1;
            }
            catch { }
            
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void GetRankServerRpc()
    {
        List<int> ranks = new List<int>();
        List<Color> colors = new List<Color>();
        List<ulong> rankClientIds = new List<ulong>();
        Debug.Log("rankedPairs:" + rankedPairs.Count);
        for (int rank = 0; rank < rankedPairs.Count; rank++)
        {
            ProgressBar RankedBar = rankedPairs[rank].Value;
            RankedBar.rank = rank;
            RankedBar.playerNum.sprite = allNumTextures[rank];
            // 添加排名和颜色信息
            ranks.Add(rank);
            if (rank == 0)
                colors.Add(GoldColor);
            else if (rank == 1)
                colors.Add(SilverColor);
            else if (rank == 2)
                colors.Add(BronzeColor);
            else
                colors.Add(DefaultColor);
            rankClientIds.Add(RankedBar.playerController.OwnerClientId);
        }
        // 将排名和颜色发送给客户端
        GetRankClientRpc(ranks.ToArray(), colors.ToArray(),rankClientIds.ToArray());
    }

    
    [ClientRpc]
    private void GetRankClientRpc(int[] ranks, Color[] colors,ulong[] rankClientIds)
    {
        isRankCreated = true;
        for (int i = 0; i < ranks.Length; i++)
        {
            ProgressBar RankedBar = rankedPairs[i].Value;
            int rank = ranks[i];
            RankedBar.rank = rank;
            RankedBar.playerNum.sprite = allNumTextures[rank];

            //Transform barTransform = rankedPairs[i].Value.transform;
            
            Transform copiedBarTransform = Instantiate(progressBarMono, finalPanelProgressBars.transform).transform;
            var progressBarTransform = copiedBarTransform.GetComponent<ProgressBarMono>();
            if (RankedBar.MaxProgress == 100)
            {
                progressBarTransform.SetFixedProgress("通关");
                copiedBarTransform.GetComponent<Image>().color = colors[i];
            }
            else
            {
                progressBarTransform.SetFixedProgress("未通关");
                copiedBarTransform.GetComponent<Image>().color = DefaultColor;
            }

            progressBarTransform.transform.GetComponent<LayoutElement>().preferredHeight = 30;
            //图片宽高
            progressBarTransform.playerNumLayoutElement.preferredWidth = 30;
            progressBarTransform.playerNumLayoutElement.preferredHeight = 30;
            
            progressBarTransform.
                SetPlayerNameByPlayerController(RankedBar.playerController);

            progressBarTransform.SetPlayerRank(rank);
            
            
            copiedBarTransform.SetSiblingIndex(rank);
           
        }
    }
    public void ShowFinalPanel()
    {
        if (!isFinalRankCreated)
        {
            isFinalRankCreated = true;
            if (IsServer)
            {
                GameManager.Instance.finalSettlement.Value = true;
            }
            for (int rank = 0; rank < rankedPairs.Count; rank++)
            {
                ProgressBar RankedBar = rankedPairs[rank].Value;
                RankedBar.rank = rank;
                RankedBar.playerNum.sprite = allNumTextures[rank];
                Transform barTransform = rankedPairs[rank].Value.transform;
                barTransform.GetComponent<ProgressBar>().CanUpdate = false;
                barTransform.SetSiblingIndex(rank);

                //排名对应颜色
                if (rank == 0)
                {
                    RankedBar.transform.GetComponent<Image>().color = GoldColor;
                }
                else if (rank == 1)
                {
                    RankedBar.transform.GetComponent<Image>().color = SilverColor;
                }
                else if (rank == 2)
                {
                    RankedBar.transform.GetComponent<Image>().color = BronzeColor;
                }
            }
            Levelbgm.Instance.PauseBgm();
            GameManager.Instance.GameOnLoad = false;
            Transform FinalPanel = finalPanel.transform.parent;
            //播放音效
            if (localProgressBar.MaxProgress < 100)
            {
                Audioplay.Instance.PlayDefeat();
                FinalPanel.GetComponent<FinalPanel>().SetTitle("失败！");
            }
            else
            {
                Audioplay.Instance.PlayVictory();
                FinalPanel.GetComponent<FinalPanel>().SetTitle("获胜！");
            }
            finalPanel.SetActive(true);
            if (NetworkManager.Singleton.IsServer)
            {
                GetRankServerRpc();
            }
        }
    }
    public void AddPlayerToProgressBar(ulong clientId)
    {
        Transform player = PlayerTools.GetLocalPlayerFromClientId(clientId);
        if (player == null) return;
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            GameObject bar = Instantiate(progressBar);
            if (IsServer)
            {
                bar.GetComponent<NetworkObject>().Spawn();
                SyncProgressBarDataClientRpc(clientId, bar);
            }
            //PlayerBars.Add(playerController, bar.GetComponent<ProgressBar>());
            //playerController.SetPlayerName(GameMultiplayer.Instance
            //    .GetPlayerDataFromNetworkList(clientId).playerName.ToString());
            //ChangePlayer(bar, playerController);
            //savedRankedPairs.Add(new KeyValuePair<PlayerController, ProgressBar>(
            //   playerController, bar.GetComponent<ProgressBar>()));
            
        }
    }
    [ClientRpc]
    void SyncProgressBarDataClientRpc(ulong clientId, NetworkObjectReference barRef)
    {
        StartCoroutine(SyncProgressBarDataClientRpcCor(clientId, barRef));
    }

    private IEnumerator SyncProgressBarDataClientRpcCor(ulong clientId, NetworkObjectReference barRef)
    {
        
            if (barRef.TryGet(out NetworkObject barObject))
            {
                ProgressBar progressBar = barObject.GetComponent<ProgressBar>();

                while (PlayerTools.GetLocalPlayerFromClientId(clientId) == null)
                {
                    yield return null;
                }
                Debug.Log("clientIdName:" + PlayerTools.GetLocalPlayerFromClientId(clientId));
                PlayerController playerController = PlayerTools.GetLocalPlayerFromClientId(clientId).GetComponent<PlayerController>();
                
                if (playerController != null)
                {
                    try
                    {
                        PlayerBars.Add(playerController, progressBar);
                        playerController.SetPlayerName(GameMultiplayer.Instance.GetPlayerDataFromNetworkList(clientId).playerName.ToString());
                        ChangePlayer(barObject.gameObject, playerController);
                        savedRankedPairs.Add(new KeyValuePair<PlayerController, ProgressBar>(playerController, progressBar));
                    }
                    catch { }
            }
        }
    }
    public void RemovePlayerFromProgressBar(ulong clientId)
    {
        Transform player = PlayerTools.GetLocalPlayerFromClientId(clientId);

        if (player == null) return;
        PlayerController playerController = player.GetComponent<PlayerController>();
        Debug.Log("RemovePlayer");
        if (playerController != null && PlayerBars.ContainsKey(playerController))
        {
            GameObject bar = PlayerBars[playerController].gameObject;
            if (bar.GetComponent<NetworkObject>().IsSpawned)
            {
                if (IsServer)
                {
                    bar.GetComponent<NetworkObject>().Despawn();
                    Destroy(bar.gameObject);
                }
            }
            else
            {
                Destroy(bar.gameObject);
            }
            PlayerBars.Remove(playerController);
            savedRankedPairs.RemoveAll(pair => pair.Key == playerController);
        }
    }
    private void ChangePlayer(GameObject bar,PlayerController playerController)
    {
        bar.GetComponent<ProgressBar>().playerController = playerController;
        bar.GetComponent<ProgressBar>().SetPlayerNameByPlayerController(playerController);
    }
    public void RemoveAllProgressBars()
    {
        foreach (Transform child in transform)
        {
            if(child != null)
            {
                if (child.GetComponent<NetworkObject>().IsSpawned)
                {
                    if (IsServer)
                    {
                        child.GetComponent<NetworkObject>().Despawn();
                        Destroy(child.gameObject);
                    }
                }
                else
                {
                    Destroy(child.gameObject);
                }
                
                
            }
        }
        PlayerBars.Clear();
        savedRankedPairs.Clear();
    }

    
}
