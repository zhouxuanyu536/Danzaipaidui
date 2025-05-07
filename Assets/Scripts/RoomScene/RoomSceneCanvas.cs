//using Mono.CSharp;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using TMPro;
//using Unity.Netcode;
//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;

//public class RoomSceneCanvas : NetworkBehaviour
//{

//    [SerializeField] private TextMeshProUGUI Title;
//    [SerializeField] private GameObject PlayerDetailPrefab;
//    [SerializeField] private GameObject bg;
//    [SerializeField] private Button ready;
//    [SerializeField] private Button Quit;

//    private NetworkVariable<int> playerCount = new NetworkVariable<int>(
//        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
//    );
//    private void Start()
//    {
//        ready.onClick.AddListener(SetPlayerReady);
//        Quit.onClick.AddListener(OnPlayerQuit);
//        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged += OnPlayerDataNetworkListChanged;
//        GameMultiplayer.Instance.OnPlayerKicked += OnPlayerKicked;
//        GameMultiplayer.Instance.OnServerExit += OnServerExit;
//    }

//    private void OnPlayerDataNetworkListChanged(object sender, EventArgs e)
//    {
//        UpdatePlayerList();
//    }
//    private void OnPlayerKicked(object sender, EventArgs e)
//    {
//        RecalculateClientIds();
//    }
//    private void OnPlayerQuit()
//    {
//        GameMultiplayer.Instance.OnPlayerQuit(NetworkManager.Singleton.LocalClientId);
//    }
//    private void OnServerExit(object sender, EventArgs e)
//    {
//        DestroyAllPlayers();
//    }
//    private void HostIsCreated()
//    {
//        UpdatePlayerList();
//    }
//    private void SetPlayerReady()
//    {
//        SetPlayerReadyServerRpc(NetworkManager.Singleton.LocalClientId);
//    }
    
    

//    private void UpdatePlayerList()
//    {
//        if (bg.IsDestroyed()) return;

//        int count = GameMultiplayer.Instance.playerDataNetworkList.Count;
//        DestroyAllPlayers();

//        for (int i = 0; i < count; i++)
//        {
//            AddPlayer(i);
//        }
//    }
//    public override void OnDestroy()
//    {
//        DestroyAllPlayers();
//        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= OnPlayerDataNetworkListChanged;
//        GameMultiplayer.Instance.OnPlayerKicked -= OnPlayerKicked;
//        GameMultiplayer.Instance.OnServerExit -= OnServerExit;
//    }
//    private void DestroyAllPlayers()
//    {
//        if (bg.IsDestroyed()) return;
//        foreach (Transform child in bg.transform)
//        {
//            Destroy(child.gameObject);
//        }
//    }
    
    
//    private void AddPlayer(int i)
//    {
//        GameObject playerDetail = Instantiate(PlayerDetailPrefab, bg.transform);
//        var detail = playerDetail.GetComponent<PlayerDetail>();

//        if (i < GameMultiplayer.Instance.playerDataNetworkList.Count)
//        {
//            detail.SetClientId(GameMultiplayer.Instance.playerDataNetworkList[i].clientNum);
//            detail.SetPlayerName(GameMultiplayer.Instance.playerDataNetworkList[i].PlayerName.ToString()
//                + GameMultiplayer.Instance.playerDataNetworkList[i].clientNum);
//            detail.SetPlayerReady(GameMultiplayer.Instance.playerDataNetworkList[i].playerReady);
//            detail.SetNum(i);
//        }
//    }
//    private void RecalculateClientIds()
//    {
//        if (GameMultiplayer.Instance.isRecalculating) return; // 避免递归
//        if (!NetworkManager.Singleton.IsServer) return;

//        GameMultiplayer.Instance.isRecalculating = true;
//        List<PlayerData> updatedList = new List<PlayerData>();

//        for (int i = 0; i < GameMultiplayer.Instance.playerDataNetworkList.Count; i++)
//        {
//            PlayerData data = GameMultiplayer.Instance.playerDataNetworkList[i];
//            data.clientNum = (ulong)i;
//            updatedList.Add(data);
//        }

//        GameMultiplayer.Instance.playerDataNetworkList.Clear();
//        foreach (var data in updatedList)
//        {
//            GameMultiplayer.Instance.playerDataNetworkList.Add(data);
//        }

//        GameMultiplayer.Instance.isRecalculating = false;
//    }
//    private bool IsAllPlayersReady()
//    {
//        bool isAllReady = true;
//        int personNum = 0;
//        foreach(PlayerData playerData in GameMultiplayer.Instance.playerDataNetworkList)
//        {
//            if(playerData.playerReady == false)
//            {
//                isAllReady = false;
//                break;
//            }
//            personNum++;
//        }
//        //if(personNum < 3)
//        //{
//        //    isAllReady = false;
//        //}
//        return isAllReady;
//    }

//    public override void OnNetworkSpawn()
//    {
//        HostIsCreated();
//    }
//    private void Update()
//    {
//        Title.text = "新房间";
//        //实时检测是否已经IsAllPlayersReady
//        //每个房间必须大于等于3人
//        if(NetworkManager.Singleton.IsServer 
//            && IsAllPlayersReady())
//        {
//            NetworkManager.Singleton.SceneManager.LoadScene("GameScene",LoadSceneMode.Single);
//        }
//    }
//}