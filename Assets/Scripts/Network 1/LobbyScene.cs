using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyScene : MonoBehaviour
{
    [SerializeField] private Button GoToMainMenu;

    [SerializeField] private GameObject Slope;
    [SerializeField] private Button CreateLobby;
    [SerializeField] private Button QuickJoin;
    [SerializeField] private GameObject Lobbies;
    [SerializeField] private GameObject LobbyNamePrefab;

    [SerializeField] private GameObject Scope;
    [SerializeField] private Button Confirm;
    [SerializeField] private Button Cancel;
    [SerializeField] private TMP_InputField InputLobbyName;
    [SerializeField] private GameObject MessageBox;
    [SerializeField] private TextMeshProUGUI MessageText;
    [SerializeField] private Button MessageBox_Confirm;
    [SerializeField] private Button MessageBox_Cancel;

    [SerializeField] private Button JoinByCode;
    [SerializeField] private TMP_InputField JoinCode;
    private void Start()
    {
        InputLobbyName.text = null;
        Slope.SetActive(true);
        GoToMainMenu.onClick.AddListener(() =>
        {
            GameLobby.Instance.LeaveLobby();
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        CreateLobby.onClick.AddListener(OnLobbyCreate);
        QuickJoin.onClick.AddListener(OnQuickJoin);

        Scope.SetActive(false);
        Confirm.onClick.AddListener(OnLobbyCreate_Confirmed);
        Cancel.onClick.AddListener(OnLobbyCreate_Cancelled);
        MessageBox.SetActive(false);
        MessageBox_Confirm.onClick.AddListener(OnLobbyCreated_ConfirmedMessageBox_Confirmed);
        MessageBox_Cancel.onClick.AddListener(OnLobbyCreated_ConfirmedMessageBox_Cancelled);

        GameLobby.Instance.OnLobbyListChanged += LobbyScene_OnLobbyListChanged;

        UpdateLobbyList(new List<Lobby>());
        JoinByCode.onClick.AddListener(JoinWithCode);
    }

    private void OnLobbyCreate()
    {
        Slope.SetActive(false);
        Scope.SetActive(true);
    }

    private void LobbyScene_OnLobbyListChanged(object sender, GameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        try
        {
            foreach (Transform child in Lobbies.transform)
            {
                Destroy(child.gameObject);
            }
            foreach (Lobby lobby in lobbyList)
            {
                Transform lobbyTransform = Instantiate(LobbyNamePrefab, Lobbies.transform).transform;
                lobbyTransform.gameObject.SetActive(true);
                lobbyTransform.GetComponent<LobbyName>().SetLobby(lobby);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
        
    }
    private void OnLobbyCreate_Confirmed()
    {
        //假设不合法字符为xxkx
        if(UnityTools.Instance.CheckIfLobbyNameIllegal(InputLobbyName.text, out string illegalMessageText))
        {
            MessageText.text = illegalMessageText;
            InputLobbyName.text = null;
            MessageBox.SetActive(true);
        }
        else
        {
            GameObject LobbyDetail = Instantiate(LobbyNamePrefab, Lobbies.transform);
            LobbyDetail.transform.GetComponent<LobbyName>().SetLobbyName(InputLobbyName.text);
            //Public
            GameLobby.Instance.CreateLobby(InputLobbyName.text, false);

            //Private
            //GameLobby.Instance.CreateLobby(InputLobbyName.text, true);
            //Slope.SetActive(true);
            //Scope.SetActive(false);
        }
    }

    private void OnLobbyCreate_Cancelled()
    {
        InputLobbyName.text = null;
        Slope.SetActive(true);
        Scope.SetActive(false);
    }
    private void OnLobbyCreated_ConfirmedMessageBox_Confirmed()
    {
        MessageBox.SetActive(false);
    }

    private void OnLobbyCreated_ConfirmedMessageBox_Cancelled()
    {
        MessageBox.SetActive(false);
    }

    public void OnQuickJoin()
    {
        GameLobby.Instance.QuickJoin();
    }
    public void JoinWithCode()
    {
        string code = JoinCode.text;
        GameLobby.Instance.JoinWithCode(code);
    }
    
}
