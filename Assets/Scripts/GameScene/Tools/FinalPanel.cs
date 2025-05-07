using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FinalPanel : NetworkBehaviour
{
    [SerializeField] private Button quitButton;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private Button backToLobbyButton;
    [SerializeField] private GameObject backToLobbyNotice;
    private TextMeshProUGUI backToLobbyNoticeText;

    private void Start()
    {
        quitButton.onClick.AddListener(GameManager.Instance.OnPlayerQuit);
        backToLobbyButton.onClick.AddListener(GameManager.Instance.OnPlayerBackToLobby);
        backToLobbyNoticeText = backToLobbyNotice.GetComponent<TextMeshProUGUI>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer || !GameMultiplayer.playMultiplayer)
        {
            backToLobbyButton.interactable = false;
            backToLobbyNotice.SetActive(true);
            if (!GameMultiplayer.playMultiplayer)
            {
                backToLobbyNoticeText = backToLobbyNotice.GetComponent<TextMeshProUGUI>();
                backToLobbyNoticeText.text = "单人游戏不能返回房间";
            }
            else
            {
                backToLobbyNoticeText = backToLobbyNotice.GetComponent<TextMeshProUGUI>();
                backToLobbyNoticeText.text = "只有主机可以返回房间";
            }
        }
        else
        {
            backToLobbyButton.interactable = true;
            backToLobbyNotice.SetActive(false);
        }
    }
    public void SetTitle(string titleText)
    {
        title.text = titleText;
    }
}
