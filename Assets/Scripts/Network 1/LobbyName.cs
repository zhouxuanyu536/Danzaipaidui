using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Button button;
    private Lobby lobby;
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(() =>
        {
            if(lobby != null)
                GameLobby.Instance.JoinWithId(lobby.Id);
        });
    }

    // Update is called once per frame

    public void SetLobby(Lobby lobby)
    {
        this.lobby = lobby;
        text.text = lobby.Name;
    }
    //仅限于UI
    public void SetLobbyName(string lobbyName)
    {
        text.text = lobbyName;
    }
    public string GetLobbyName()
    {
        return text.text;
    }
}
