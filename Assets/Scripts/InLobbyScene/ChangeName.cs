using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChangeName : NetworkBehaviour
{
    [SerializeField] private TMP_InputField InputField;
    [SerializeField] private Button confirm;
    [SerializeField] private Button cancel;
    [SerializeField] private TextMeshProUGUI hintMessage; //提示
    private bool isSpawned;
    // Start is called before the first frame update
    private void Awake()
    {
        //不能在OnNetworkSpawn之前Hide()!
    }
    public override void OnNetworkSpawn()
    {
        isSpawned = true;
        Hide();
        confirm.onClick.AddListener(OnConfirm);
        cancel.onClick.AddListener(OnCancel);
    }
    void OnEnable()
    {
        if (!isSpawned) return;
        if (!GameMultiplayer.Instance.isSpawned) return;
        InputField.text = GameMultiplayer.Instance.
            GetPlayerDataFromNetworkList(NetworkManager.Singleton.LocalClientId).playerName.ToString();
    }

    private void OnConfirm()
    {
        if(UnityTools.Instance.CheckIfPlayerNameIllegal(InputField.text,out string illegalMessage))
        {
            hintMessage.text = illegalMessage;
            InputField.text = "";
        }
        else
        {
            hintMessage.text = "";
            GameMultiplayer.Instance.SetPlayerName(InputField.text);
            GameMultiplayer.Instance.SetPlayerNameServerRpc(InputField.text);
            Hide();
        }
        
    }

    private void OnCancel()
    {
        hintMessage.text = "";
        Hide();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        confirm.onClick.RemoveAllListeners();
        cancel.onClick.RemoveAllListeners();
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        hintMessage.text = "";
        gameObject.SetActive(false);
    }
}
