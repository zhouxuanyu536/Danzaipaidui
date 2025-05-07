using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using IEnumerator = System.Collections.IEnumerator;
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnpaused;
    public event EventHandler OnLocalPlayerReadyChanged;
    public event EventHandler OnAllPlayersReadyChanged;
    public event EventHandler OnSceneLoadCompletedEvent;
    public event EventHandler<PlayerEventArgs> OnPlayerQuitEvent;


    [SerializeField] private Transform PlayerPrefab;
    public ProgressBars progressBars;
    [SerializeField] private Button Pause;
    [SerializeField] private GameObject GamePause;
    [SerializeField] private PlayerCountDownTimer playerCountDownTimer;
    [SerializeField] private GameObject LoadingPanel;

    public NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    public NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public NetworkVariable<bool> isAllPlayersReady = new NetworkVariable<bool>(true);


    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;


    private bool localPlayerPause;

    public bool isPaused;

    public int playerFinishedRace;

    public bool GameOnLoad;


    public NetworkVariable<bool> PlayerAllSpawned = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> finalSettlement = new NetworkVariable<bool>(false); //是否进入结算
    private int playerAlreadySpawned;

    public LevelDetails levelDetails;
    public enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
    }
    private void Awake()
    {
        Instance = this;
        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPausedDictionary = new Dictionary<ulong, bool>();
        GameMultiplayer.Instance.OnFailedToJoinGame += GameManager_OnFailedToJoinGame;
        Pause.onClick.AddListener(OnPlayerPause);
        localPlayerPause = false;
        //开局等待CountToStart
        isPaused = true;

        OnPlayerQuitEvent += GameManager_OnPlayerQuit;

        playerFinishedRace = 0;

        GameMultiplayer.Instance.ServerOnClientDisconnectedEvent += DisconnectedEvent;
        GameMultiplayer.Instance.ClientOnClientDisconnectedEvent += DisconnectedEvent;

        GameOnLoad = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetCursorLock();
    }

    //public bool localPlayerIsServer()
    //{

    //}
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        bool allPlayersReady = playerPausedDictionary.Values.All(value => value == false);
        if (allPlayersReady)
        {
            isAllPlayersReady.Value = true;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerPausedServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = false;
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        isAllPlayersReady.Value = false;
        isGamePaused.Value = true;

    }
    public override void OnNetworkSpawn()
    {
        StateValueChanged(State.WaitingToStart, State.CountdownToStart);
        state.OnValueChanged += StateValueChanged;
        isGamePaused.OnValueChanged += GamePaused_OnValueChanged;
        SetGameStateServerRpc(1);
        isAllPlayersReady.OnValueChanged += isAllPlayersReady_OnValueChanged;
        if (GameMultiplayer.Instance.IsServer)
        {
            isAllPlayersReady.Value = true;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;

        }
        //注册
        Debug.Log(NetworkManager.Singleton.LocalClientId);
        SetPlayerReadyServerRpc();
    }
    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        playerAlreadySpawned = 0;
        PlayerAllSpawned.Value = false;
        finalSettlement.Value = false;
        if (sceneName == "GameScene")
        {
            OnSceneLoadCompletedEvent?.Invoke(this, EventArgs.Empty);
            StartCoroutine(LoadEventLogic());
        }


    }
    private IEnumerator LoadEventLogic()
    {
        float timeout = 10f; // 设置一个超时时间
        float startTime = Time.time;

        yield return new WaitUntil(() =>
            GameMultiplayer.Instance.playerLevelLoadFinishedCount.Value >= NetworkManager.Singleton.ConnectedClientsIds.Count ||
            Time.time - startTime > timeout
        );
        Debug.Log("playerLevelLoadFinishedCount:" + GameMultiplayer.Instance.playerLevelLoadFinishedCount.Value);
        LoadLevelDetailsClientRpc();
        //通知所有的客户端加载所有的playerGameObject 以及 对应每个客户端的 LocalPlayer
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            GameObject playerGameObject = PlayerTools.CreatePlayer();
            //同步方法
            playerGameObject.transform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
           
        }

    }
    [ClientRpc]
    private void LoadLevelDetailsClientRpc(){
        levelDetails = FindObjectOfType<LevelDetails>();
    }
    [ServerRpc(RequireOwnership = false)]
    public void NotifyServerPlayerSpawnedServerRpc()
    {
        playerAlreadySpawned += 1;
        if (playerAlreadySpawned >= NetworkManager.Singleton.ConnectedClientsIds.Count)
        {
            PlayerAllSpawned.Value = true;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void NotifyServerPlayerQuitServerRpc()
    {
        playerAlreadySpawned -= 1;
    }

    

    private void StateValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
        if(newValue == State.WaitingToStart)
        {
            playerCountDownTimer.Hide();
            isPaused = true;
            //DOTween.timeScale = 0f;
        }
        else if(newValue == State.CountdownToStart)
        {
            playerCountDownTimer.Show();
            isPaused = true;
            //DOTween.timeScale = 0f;
        }
        else
        {
            isPaused = false;
            //DOTween.timeScale = 1f;
        }
    }

    private void GamePaused_OnValueChanged(bool preValue, bool newValue)
    {
        localPlayerPause = newValue;
        if (newValue)
        {
            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
            SetPlayerPausedServerRpc();
            SetGameStateServerRpc(0);
        }
        else
        {
            OnMultiplayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        }

    }
   
    private void isAllPlayersReady_OnValueChanged(bool preValue, bool newValue)
    {
        OnAllPlayersReadyChanged?.Invoke(this, EventArgs.Empty);
        if (newValue && state.Value == State.WaitingToStart)
        {
            SetGamePausedServerRpc(false);
            SetGameStateServerRpc(1);
        }
        else if (!newValue)
        {
            SetGameStateServerRpc(0);
        }
    }

    public int GetGameState()
    {
        if (state.Value == State.WaitingToStart)
        {
            return 0;
        }
        else if(state.Value == State.CountdownToStart)
        {
            return 1;
        }
        else
        {
            return 2;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetGamePausedServerRpc(bool isPaused)
    {
        isGamePaused.Value = isPaused;
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetGameStateServerRpc(int stateNum)
    {
        if(stateNum == 0)
        {
            state.Value = State.WaitingToStart;
        }
        else if(stateNum == 1)
        {
            state.Value = State.CountdownToStart;
        }
        else
        {
            SetGameOnLoadClientRpc(false);
            state.Value = State.GamePlaying;
        }
    }
    [ClientRpc]
    public void SetGameOnLoadClientRpc(bool isOnLoad)
    {
        GameOnLoad = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeletePlayerServerRpc(ulong clientId)
    {
        //Despawn
        foreach (Transform player in PlayerTools.players)
        {
            if(player.GetComponent<NetworkObject>().OwnerClientId == clientId)
            {
                progressBars.RemovePlayerFromProgressBar(clientId);
                PlayerTools.RemovePlayerFromLists(clientId);
                InformClientsChangePlayerListClientRpc(clientId);
                player.GetComponent<NetworkObject>().Despawn(true);
                break;
            }
        }
        if (playerReadyDictionary.ContainsKey(clientId))
        {
            playerReadyDictionary.Remove(clientId);
        }
        if (playerPausedDictionary.ContainsKey(clientId))
        {
            playerPausedDictionary.Remove(clientId);
        }
       
    }

    [ClientRpc]
    public void InformClientsChangePlayerListClientRpc(ulong clientId)
    {
        PlayerTools.RemovePlayerFromLists(clientId);
    }

    private void DisconnectedEvent(object sender,PlayerEventArgs e)
    {
        NotifyServerPlayerQuitServerRpc();
        DisconnectedEventServerRpc(e.clientId);
    }

    
    [ServerRpc(RequireOwnership = false)]
    private void DisconnectedEventServerRpc(ulong clientId)
    {
        DisconnectedEventClientRpc(clientId);
    }
    [ClientRpc]
    private void DisconnectedEventClientRpc(ulong clientId)
    {
        progressBars.RemovePlayerFromProgressBar(clientId);
        PlayerTools.RemovePlayerFromLists(clientId);
    }
    public void OnPlayerQuit()
    {
        OnPlayerQuitEvent?.Invoke(this, new PlayerEventArgs(NetworkManager.Singleton.LocalClientId));
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
            GameLobby.Instance.DeleteLobby();
        }
        else
        {
            GameLobby.Instance.LeaveLobby();
            DeletePlayerServerRpc(NetworkManager.Singleton.LocalClientId);
            GameMultiplayer.Instance.KickPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }
    public void OnPlayerBackToLobby()
    {
        PlayerTools.RemoveAllPlayers();
        GameLobby.Instance.UnlockLobby(); //必定是Server
        Loader.LoadNetwork(Loader.Scene.InLobbyScene);
    }

    public void GameManager_OnPlayerQuit(object sender,PlayerEventArgs e)
    {
        PlayerTools.RemovePlayerFromLists(e.clientId);
        //UnityTools.Instance.CleanInstance();
        Debug.Log("IsQuit");
    }

    public void OnPlayerPause()
    {
        if (!localPlayerPause)
        {
            localPlayerPause = true;
            SetPlayerPausedServerRpc();
        }
        else
        {
            localPlayerPause = false;
            OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    public void GameManager_OnFailedToJoinGame(object sender,EventArgs e)
    {
        PlayerTools.RemoveAllPlayers();
        UnityTools.Instance.CleanInstance();
        Loader.Load(Loader.Scene.MainMenuScene);

    }
    public void GameManager_Server_OnClientDisconnected(object sender, EventArgs e)
    {
        if (sender is NetworkManager networkManager)
        {
            ulong clientId = networkManager.LocalClientId;
            Debug.Log("ClientId:" + clientId);
            InformClientsChangePlayerListClientRpc(clientId);
        }
    }

    private void OnDestroy()
    {
        if(NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
        }
        GameMultiplayer.Instance.OnFailedToJoinGame -= GameManager_OnFailedToJoinGame;
        GameMultiplayer.Instance.ServerOnClientDisconnectedEvent -= DisconnectedEvent;
        GameMultiplayer.Instance.ClientOnClientDisconnectedEvent -= DisconnectedEvent;
    }
    private void OnApplicationQuit()
    {
        GameLobby.Instance.QuitLobby();
    }
    private void SetCursorLock()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }
}
