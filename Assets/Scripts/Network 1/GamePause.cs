using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePause : NetworkBehaviour
{
    [SerializeField] private Button Quit;
    [SerializeField] private Button BackToLobby;
    [SerializeField] private GameObject BackToLobbyNotice;
    private TextMeshProUGUI backToLobbyNoticeText;
    [SerializeField] private GameObject PauseUI;
    [SerializeField] private GameObject WaitingForUnpause;
    [SerializeField] private Image PauseImage;
    [SerializeField] private Sprite PlaySprite;
    [SerializeField] private Sprite PauseSprite;
    [SerializeField] private Button Settings;
    [SerializeField] private Button SettingsQuitButton;
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private GameObject SettingsPanelMessage;
    // Start is called before the first frame update
    void Start()
    {
        PauseUI.SetActive(false);
        WaitingForUnpause.SetActive(false);
        Quit.onClick.AddListener(GameManager.Instance.OnPlayerQuit);
        BackToLobby.onClick.AddListener(GameManager.Instance.OnPlayerBackToLobby);
        
        GameManager.Instance.OnMultiplayerGamePaused += OnPlayerPause;
        GameManager.Instance.OnAllPlayersReadyChanged += OnPlayerAllReady;
        GameManager.Instance.OnLocalGameUnpaused += OnPlayerContinue;
        Settings.onClick.AddListener(OnPlayerHitSettingsButton);
        SettingsQuitButton.onClick.AddListener(OnPlayerHitSettingsQuitButton);
        Settings.gameObject.SetActive(false);
        SettingsPanel.SetActive(false);
        SettingsQuitButton.gameObject.SetActive(false);

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer || !GameMultiplayer.playMultiplayer)
        {
            BackToLobby.interactable = false;
            BackToLobbyNotice.SetActive(true);
            if (!GameMultiplayer.playMultiplayer)
            {
                backToLobbyNoticeText = BackToLobbyNotice.GetComponent<TextMeshProUGUI>();
                backToLobbyNoticeText.text = "单人游戏不能返回房间";
            }
            else
            {
                backToLobbyNoticeText = BackToLobbyNotice.GetComponent<TextMeshProUGUI>();
                backToLobbyNoticeText.text = "只有主机可以返回房间";
            }
        }
        else
        {
            BackToLobby.interactable = true;
            BackToLobbyNotice.SetActive(false);
        }
    }
    private void OnPlayerPause(object sender,EventArgs e)
    {
        PauseUI.SetActive(true);
        WaitingForUnpause.SetActive(false);
        PauseImage.sprite = PlaySprite;
        Settings.gameObject.SetActive(true);
        SettingsPanel.SetActive(false);
        SettingsPanelMessage.SetActive(false);
    }
    private void OnPlayerContinue(object sender, EventArgs e)
    {
        GameManager.Instance.SetPlayerReadyServerRpc();
        PauseUI.SetActive(false);
        if (!GameManager.Instance.isAllPlayersReady.Value)
        {
            WaitingForUnpause.SetActive(true);
        }
        PauseImage.sprite = PauseSprite;
        Settings.gameObject.SetActive(false);
        SettingsPanel.SetActive(false);
        SettingsQuitButton.gameObject.SetActive(false);
    }

    private void OnPlayerAllReady(object sender, EventArgs e)
    {
        if (GameManager.Instance.isAllPlayersReady.Value)
        {
            PauseUI.SetActive(false);
            WaitingForUnpause.SetActive(false);
        }
    }

    private void OnPlayerHitSettingsButton()
    {
        SettingsPanelMessage.SetActive(true);
        SettingsPanel.SetActive(true);
        SettingsQuitButton.gameObject.SetActive(true);
    }
    private void OnPlayerHitSettingsQuitButton()
    {
        SettingsPanelMessage.SetActive(false);
        SettingsPanel.SetActive(false);
        SettingsQuitButton.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
