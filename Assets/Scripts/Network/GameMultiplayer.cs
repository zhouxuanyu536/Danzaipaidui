//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Unity.Collections;
//using Unity.Netcode;
//using Unity.Netcode.Transports.UTP;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using Random = UnityEngine.Random;

//[Serializable]
//public class NetworkData
//{
//    public string NetworkBindPort;
//}

//[Serializable]
//public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
//{
//    public ulong clientNum;
//    public ulong clientId;
//    public FixedString32Bytes PlayerName;
//    public bool playerReady;

//    public bool Equals(PlayerData other)
//    {
//        return clientNum == other.clientNum && clientId == other.clientId && PlayerName == other.PlayerName
//            && playerReady == other.playerReady;
//    }

//    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
//    {
//        serializer.SerializeValue(ref clientNum);
//        serializer.SerializeValue(ref clientId);
//        serializer.SerializeValue(ref playerReady);
//        serializer.SerializeValue(ref PlayerName);
//    }
//}

//public class GameMultiplayer : NetworkBehaviour
//{
//    public static GameMultiplayer Instance;
//    public GameObject PlayerPrefab;
//    public Vector3 playerSpawnPosition;
//    public event EventHandler OnPlayerDataNetworkListChanged;
//    public event EventHandler OnPlayerKicked;
//    public event EventHandler OnServerExit;

//    public NetworkVariable<ushort> randomPort = new NetworkVariable<ushort>(0);
//    private ushort port;
//    private readonly string JsonPath = "D:/SaveData/SaveNetWorkJson.json";
//    public NetworkList<PlayerData> playerDataNetworkList;

//    public bool isRecalculating;
//    public List<Transform> playerTransformsServer; //只有服务器能够存储所有的playerTransform
//    private void Awake()
//    {
//        playerDataNetworkList = new NetworkList<PlayerData>();
//        playerDataNetworkList.OnListChanged += PlayerDataListOnListChanged;
//        Instance = this;
//        DontDestroyOnLoad(gameObject);
//        isRecalculating = false;
//    }
//    public override void OnDestroy()
//    {
//        base.OnDestroy();
//        if (playerDataNetworkList != null)
//        {
//            playerDataNetworkList.OnListChanged -= PlayerDataListOnListChanged;
//        }
//    }

//    public override void OnNetworkSpawn()
//    {
//        if (IsServer)
//        {
//            randomPort.Value = port;
//            SavePortToJson();
//            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
//        }
//    }

//    private void SavePortToJson()
//    {
//        NetworkData data = new NetworkData { NetworkBindPort = port.ToString() };
//        string jsonData = JsonUtility.ToJson(data);
//        string directoryPath = Path.GetDirectoryName(JsonPath);
//        if (!Directory.Exists(directoryPath))
//        {
//            Directory.CreateDirectory(directoryPath);
//        }
//        File.WriteAllText(JsonPath, jsonData);
//    }

//    public void OnClientConnect(ulong clientId)
//    {
//        playerDataNetworkList.Add(new PlayerData { clientNum = clientId, clientId = clientId ,PlayerName = "zxy921", playerReady = false });

//    }

//    public void OnClientDisconnect(ulong clientId)
//    {
//        if (IsServer)
//        {
//            for (int i = 0; i < playerDataNetworkList.Count; i++)
//            {

//                if (playerDataNetworkList[i].clientId == clientId)
//                {
//                    playerDataNetworkList.RemoveAt(i);
//                    break;
//                }
//            }
//        }
//        if (clientId == NetworkManager.Singleton.LocalClientId)
//        {
//            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
//            NetworkManager.Singleton.OnServerStopped -= OnServerShutdown;
//            SceneManager.LoadScene("MainMenuScene");
//        }
//    }

//    public void PlayerDataListOnListChanged(NetworkListEvent<PlayerData> changeEvent)
//    {
//        OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
//        OnPlayerKicked?.Invoke(this, EventArgs.Empty);
//    }

//    public void StartHost()
//    {
//        if (NetworkManager.Singleton.NetworkConfig.NetworkTransport is UnityTransport transport)
//        {
//            port = (ushort)Random.Range(6000, 20000);
//            transport.ConnectionData.Port = port;
//        }
//        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
//        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
//        NetworkManager.Singleton.OnServerStopped += OnServerShutdown;
//        NetworkManager.Singleton.StartHost();
//        NetworkManager.Singleton.SceneManager.LoadScene("RoomScene", LoadSceneMode.Single);
//    }

//    public void StartClient()
//    {
//        string jsonData = File.ReadAllText(JsonPath);
//        NetworkData loadedData = JsonUtility.FromJson<NetworkData>(jsonData);
//        if (ushort.TryParse(loadedData.NetworkBindPort, out ushort getPort))
//        {
//            if (NetworkManager.Singleton.NetworkConfig.NetworkTransport is UnityTransport transport)
//            {
//                transport.ConnectionData.Port = getPort;
//            }
           
//            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
//            NetworkManager.Singleton.OnServerStopped += OnServerShutdown;
//            NetworkManager.Singleton.StartClient();
//        }
//        else
//        {
//        }
//    }
//    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
//    {
//        if(sceneName != "GameScene") return;
//        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
//        {
//            GameObject playerInstance = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
//            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
//            //PlayerTools.player = playerInstance.transform;
//            Vector3 pos = GetPlayerSpawnPosition(clientId);
//            //PlayerTools.player.GetComponent<PlayerController>().controller.Move(pos);
//            //加入ProgressBar中
//            GameCanvasActionHandler actionHandler = GameObject.FindObjectOfType<GameCanvasActionHandler>();

//            LevelDetails levelDetails = UnityTools.Instance.GetLevelDetails();

//            actionHandler.progressBars.AddPlayerControllerServerRpc(NetworkManager.Singleton.LocalClientId);
//            //levelDetails.AddPlayerControllerServerRpc(NetworkManager.Singleton.LocalClientId);
//        }
//    }


//    private void OnServerShutdown(bool isStopped)
//    {
//        OnServerExit?.Invoke(this, EventArgs.Empty);
//    }
//    public void OnPlayerQuit(ulong clientId)
//    {
        
//        if (clientId == NetworkManager.ServerClientId)
//        {
//            ulong[] clientsToDisconnect = NetworkManager.Singleton.ConnectedClientsIds.ToArray();
//            foreach (ulong id in clientsToDisconnect)
//            {
//                if (id == NetworkManager.ServerClientId) continue;
//                OnPlayerQuitServerRpc(id);
//            }
//            ShutdownNetwork();
//            SceneManager.LoadScene("MainMenuScene");
//        }
//        else
//        {
//            OnPlayerQuitServerRpc(clientId);
//        }
//    }
//    [ServerRpc(RequireOwnership = false)]
//    private void OnPlayerQuitServerRpc(ulong clientId)
//    {
//        OnKickPlayerClientRpc(clientId);
//        NetworkManager.Singleton.DisconnectClient(clientId);
//    }
//    [ClientRpc]
//    public void OnKickPlayerClientRpc(ulong clientId)
//    {
//        if(NetworkManager.Singleton.LocalClientId == clientId)
//        {
//            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
//            NetworkManager.Singleton.OnServerStopped -= OnServerShutdown;
//            Destroy(NetworkManager.Singleton.gameObject); // 销毁NetworkManager
//            Destroy(gameObject); //销毁GameMultiplayer
//            SceneManager.LoadScene("MainMenuScene");
//        }
//    }

//    public void ShutdownNetwork()
//    {
//        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnect;
//        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
//        NetworkManager.Singleton.OnServerStopped -= OnServerShutdown;
//        if (NetworkManager.Singleton != null)
//        {
//            try
//            {
//                foreach (var obj in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
//                {
//                    obj.Despawn(true);
//                }
//            }
//            catch (Exception e)
//            {

//            }
//        }
//        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
//        {
//            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
//            {
//                NetworkManager.Singleton.Shutdown(); // 关闭网络
//            }
//        }
//        Destroy(NetworkManager.Singleton.gameObject); // 销毁NetworkManager
//        Destroy(gameObject); // 销毁 GameMultiplayer
//    }
//    [ServerRpc(RequireOwnership = false)]
//    public void SpawnPlayerServerRpc(ulong clientId)
//    {

//    }

//    private void SetPlayerTransform(ulong clientId)
//    {
//        if(clientId == NetworkManager.Singleton.LocalClientId)
//        {

            
//        }
//    }

//    public Vector3 GetPlayerSpawnPosition(ulong clientId)
//    {
//        Transform Level = UnityTools.Instance.GetLevelTransform();
//        return Level.position + new Vector3(clientId * 2, 0, 0);
//    }
//    [ServerRpc(RequireOwnership = false)]
//    public void SetPlayerTransformsServerRpc(ulong clientId)
//    {
//        Transform player = PlayerTools.FindPlayerController(clientId).transform;
//        if(player != null)
//        {

//        }
//    }
//}