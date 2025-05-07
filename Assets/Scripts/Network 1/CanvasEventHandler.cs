using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CanvasEventHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged += PlayerAllReady;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayerAllReady(object sender,EventArgs e)
    {
        bool isAllReady = true;
        foreach(PlayerData playerData in GameMultiplayer.Instance.GetPlayerDataNetworkList())
        {
            if (!playerData.isReady)
            {
                isAllReady = false;
                break;
            }
        }
        if (isAllReady)
        {
            //GameLobby.Instance.DeleteLobby();
            GameMultiplayer.Instance.SetAllPlayerNotReady();
            if (NetworkManager.Singleton.IsServer)
            {
                GameLobby.Instance.LockLobby();
                Loader.LoadNetwork(Loader.Scene.GameScene);
            }
            
        }
    }
    private void OnDestroy()
    {
        GameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= PlayerAllReady;
    }
}
