using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ShowMemberNum : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI numText;
    private bool canShow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        canShow = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(canShow)
        {
            GetConnectedClientsNumServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void GetConnectedClientsNumServerRpc()
    {
        GetConnectedClientsClientRpc(NetworkManager.Singleton.ConnectedClients.Count);
    }
    [ClientRpc]
    private void GetConnectedClientsClientRpc(int count)
    {
        numText.text = $"玩家人数：{count}";
    }
}
