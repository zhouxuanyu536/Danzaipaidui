using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LevelGenerator : NetworkBehaviour
{
    private string LevelPath = "Prefabs/Levels/";

    private int level;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            LoadLevel();
        }
    }
    private void LoadLevel()
    {
        LoadLevelServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void LoadLevelServerRpc()
    {
        int level = GameMultiplayer.Instance.level.Value;
        if (level >= 1 && level <= 4)
        {
            this.level = level;
            LoadLevelByLevelValue(level);
        }
        else
        {
            this.level = 1;
            LoadLevelByLevelValue(1);
        }

    }

    private void LoadLevelByLevelValue(int level)
    {
        GameObject Level = Resources.Load<GameObject>(LevelPath + $"Level {level}");
        GameObject instantiatedLevel;
        if (Level != null)
        {
            instantiatedLevel = Instantiate(Level);
            instantiatedLevel.GetComponent<NetworkObject>().Spawn();
        }
        
    }
    [ServerRpc(RequireOwnership = false)]
    public void AfterLoadLevelByLevelValueServerRpc()
    {
        AfterLoadLevelByLevelValueClientRpc(level);
    }
    [ClientRpc]
    public void AfterLoadLevelByLevelValueClientRpc(int level)
    {
        LoadLevelSkyBox(level);
        Levelbgm.Instance.SetAudioClipFromLevel(level);
        RegisterWayProgressToProgressBars();
        NotifyServerChangeValueServerRpc();
    }
    private void LoadLevelSkyBox(int level)
    {
        if(level == 1)
        {
            SkyboxSettings.ChangeSkybox(SkyboxType.Day1);
        }
        else if(level == 2)
        {
            SkyboxSettings.ChangeSkybox(SkyboxType.Day2);
        }
        else if(level == 3)
        {
            SkyboxSettings.ChangeSkybox(SkyboxType.Night1);
        }
        else
        {
            SkyboxSettings.ChangeSkybox(SkyboxType.Night2);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void NotifyServerChangeValueServerRpc()
    {
        GameMultiplayer.Instance.playerLevelLoadFinishedCount.Value += 1;
    }

    private void RegisterWayProgressToProgressBars()
    {
        ProgressBars.Instance.wayProgress = FindObjectOfType<WayProgress>();
        Debug.Log("wayProgress:" + ProgressBars.Instance.wayProgress);
    }
}
