using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InLobbyScene : NetworkBehaviour
{
    [SerializeField] private Button QuitButton;

    [SerializeField] private Button ChangeNameButton;
    [SerializeField] private ChangeName ChangeName;

    [SerializeField] private TextMeshProUGUI JoinTitleText;
    [SerializeField] private TextMeshProUGUI JoinCodeText;

    private string JoinCodeTextPrefix = "加入Code:";

    [SerializeField] private GameObject JoinDetails;
    [SerializeField] private GameObject JoinPlayer;

    [SerializeField] private Button ReadyButton;
    [SerializeField] private TextMeshProUGUI ReadyButtonText;

    [SerializeField] private TMP_Dropdown SelectColor;
    [SerializeField] private GameObject SelectLevelGameObject;
    [SerializeField] private GameObject SelectLevelDropdown;
    [SerializeField] private GameObject ShowLevelGameObject;
    [SerializeField] private GameObject ShowMemberNum;
    [SerializeField] private TMP_Dropdown SelectLevel;

    private List<Color> allColors;

    private void Awake()
    {
        SelectLevelGameObject.SetActive(false);
        SelectLevelDropdown.SetActive(false);
        ShowLevelGameObject.SetActive(true);
        ShowMemberNum.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        JoinCodeText.text = JoinCodeTextPrefix + $"<b><color=red>{GameLobby.Instance.GetLobby().LobbyCode}</color></b>";
        ReadyButtonText.text = "准备";

        QuitButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.QuitLobby();
            Loader.Load(Loader.Scene.MainMenuScene); //直接返回
        });

        ChangeNameButton.onClick.AddListener(() =>
        {
            ChangeName.Show();
        });
        ReadyButton.onClick.AddListener(() =>
        {
            GameMultiplayer.Instance.SetPlayerIsReadyServerRpc(autoAdjust: true);
            if(ReadyButtonText.text == "准备")
            {
                ReadyButtonText.text = "取消准备";
            }
            else
            {
                ReadyButtonText.text = "准备";
            }
        });

        SelectColor.onValueChanged.AddListener(OnColorSelected);
        allColors = PlayerColor.allColors;

        

        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged += InLobbyScene_OnPlayerDataNetworkListChanged;
        UpdateLobbyPlayers();

        GameMultiplayer.Instance.OnFailedToJoinGame += InLobbyScene_OnFailedToJoin;
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log("isServer");
        //删除已经Spawn过的关卡
        LevelDetails levelDetails;
        if((levelDetails = FindObjectOfType<LevelDetails>()) != null && IsServer)
        {
            if (levelDetails.IsSpawned)
            {
                levelDetails.transform.GetComponent<NetworkObject>().Despawn();
            }
            Destroy(levelDetails.gameObject);
        }
        //只有服务器才能选择关卡
        if (NetworkManager.Singleton.IsServer)
        {
            if (!GameMultiplayer.Instance.isInLobbyInitialized)
            {
                SelectLevel.value = GameMultiplayer.playSinglePlayerLevel - 1;
                GameMultiplayer.Instance.level.Value = GameMultiplayer.playSinglePlayerLevel;
                SelectLevel.RefreshShownValue();
            }
            else
            {
                SelectLevel.value = GameMultiplayer.Instance.level.Value;
                SelectLevel.RefreshShownValue();
            }
            JoinTitleText.text = $"您已创建房间{GameLobby.Instance.GetLobby().Name}！";
            ShowLevelGameObject.SetActive(false);
            ShowMemberNum.SetActive(false);
            SelectLevelDropdown.SetActive(true);
            SelectLevelGameObject.SetActive(true);
            SelectLevel.onValueChanged.AddListener(OnLevelSelected);
        } 
        else
        {
            JoinTitleText.text = $"您已进入房间{GameLobby.Instance.GetLobby().Name}！";
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void InLobbyScene_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
    {
        UpdateLobbyPlayers();
    }

    private void InLobbyScene_OnFailedToJoin(object sender,EventArgs e)
    {
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    private void UpdateLobbyPlayers()
    {
        try
        {
            foreach (Transform child in JoinDetails.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (PlayerData playerData in GameMultiplayer.Instance.GetPlayerDataNetworkList())
            {
                Debug.Log("playerDataName:" + playerData.playerName);
                Transform joinPlayer = Instantiate(JoinPlayer, JoinDetails.transform).transform;
                joinPlayer.GetComponent<JoinPlayer>().SetPlayer(playerData);
            }
        }
        catch(Exception ex) { }
        
    }
    private void OnColorSelected(int index)
    {
        if (index >= 0 && index < allColors.Count)
        {
            Debug.Log($"选择了颜色：{allColors[index]}");

            GameMultiplayer.Instance.SetPlayerColorServerRpc(allColors[index]);
        }
        else
        {
            Debug.LogError("无效的颜色索引");
        }
    }
    
    private void OnLevelSelected(int index)
    {
        GameMultiplayer.Instance.level.Value = index + 1;
        if (IsServer)
        {
            GameMultiplayer.playSinglePlayerLevel = GameMultiplayer.Instance.level.Value;
        }
        Debug.Log("Value: " + GameMultiplayer.Instance.level.Value);
    }
    private void OnDestroy()
    {
        QuitButton.onClick.RemoveAllListeners();
        if(GameManager.Instance != null)
        {
            GameMultiplayer.Instance.OnFailedToJoinGame -= InLobbyScene_OnFailedToJoin;
        }
        
    }
}
