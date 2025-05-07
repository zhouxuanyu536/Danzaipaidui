
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public static class PlayerTools
{
    public static List<Transform> players = new List<Transform>();
    public static List<ulong> connectClientsIdsIndexes = new List<ulong>();
    private static string PlayerPrefabPath = "Prefabs/Player";
    private static GameObject LevelPrefab;
    private static int LocalPlayerId = 0;
    private static int LocalPlayerIndex = 0;
    public static GameObject CreatePlayer()
    {
        GameObject playerGameObject = GameObject.Instantiate(Resources.Load(PlayerPrefabPath),
            Vector3.zero, Quaternion.identity) as GameObject;
        PlayerController playerController = playerGameObject.transform.GetComponent<PlayerController>();
        return playerGameObject;
    }
    

    public static Transform GetLocalPlayer()
    {
        if (players.Count == 0) return null;
        //设置所有player
        for(int i = 0;i < players.Count; i++)
        {
            if (players[i] == null) continue;
            if ((int)connectClientsIdsIndexes[i] == LocalPlayerId)
            {
                players[i].GetComponent<PlayerController>().OnPlayerEnabled();
                LocalPlayerIndex = i;
            }
            else
            {
                players[i].GetComponent<PlayerController>().OnPlayerDisabled();
            }
        }
        return players[LocalPlayerIndex];
    }

    public static Transform GetLocalPlayerFromClientId(ulong clientId)
    {
        for(int i = 0;i < connectClientsIdsIndexes.Count; i++)
        {
            if (connectClientsIdsIndexes[i] == clientId)
            {
                return players[i];
            }
        }
        return null;
    }
    
    public static void RemoveAllPlayers()
    {
        players = new List<Transform>();
        connectClientsIdsIndexes = new List<ulong>();
    }
    public static void RemovePlayerFromLists(ulong clientId)
    {
        for(int i = 0;i < connectClientsIdsIndexes.Count; i++)
        {
            if(clientId == connectClientsIdsIndexes[i])
            {
                players[i] = null;
                connectClientsIdsIndexes[i] = ulong.Parse("114514"); //断线
                break;
            }
        }
    }
    public static void ChangeLocalPlayer(ulong clientId)
    {
        LocalPlayerId = (int)clientId;
        GetLocalPlayer();
    }


}
