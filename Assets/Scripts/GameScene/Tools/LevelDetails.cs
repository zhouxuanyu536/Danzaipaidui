using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerEventArgs : EventArgs
{
    public PlayerController playerController;
    public Transform playerTransform { get; }

    public ulong clientId;
    public PlayerEventArgs(PlayerController controller,Transform playerTransform)
    {
        playerController = controller;
        this.playerTransform = playerTransform;
    }

    public PlayerEventArgs(ulong clientId)
    {
        this.clientId = clientId;
    }
}

public class LevelDetails : NetworkBehaviour
{
    private List<PlayerController> serverPlayerControllers; //服务器端
    public Transform CheckPointsTransform;
    /// <summary>
    /// 存储玩家在检查点位置的字典。
    /// <para>PlayerController:玩家控制器对象，用于标识具体玩家。</para>
    /// <para>Transform:玩家对应的检查点位置。</para>
    /// </summary>
    private Dictionary<PlayerController, Transform> PlayersAtCheckPoints;
    [SerializeField] private float DropHeight; //坠落高度
    public CinemachineVirtualCamera cameraPreview;

    public Transform doorButtons;
    public Transform floorActive;
    // Start is called before the first frame update
    void Awake()
    {
        if(CheckPointsTransform == null)
        {
            CheckPointsTransform = transform;
        }
        if(DropHeight <= 0)
        {
            DropHeight = 120;
        }
        PlayersAtCheckPoints = new Dictionary<PlayerController, Transform>();
        UnityTools.Instance.AddPlayerEvent += AddPlayer;
        
    }


    // Update is called once per frame
    void LateUpdate()
    {
        if (GameManager.Instance == null) return;
        if (!GameManager.Instance.isPaused)
            UpdateChecks();
    }

    public override void OnNetworkSpawn()
    {
        FindObjectOfType<LevelGenerator>()?.AfterLoadLevelByLevelValueServerRpc();
    }
    void UpdateChecks()
    {
        try
        {
            CheckPlayersAtWhichCheckPoints();
            CheckPlayersFallOff();
        }
        catch(Exception e)
        {

        }
    }
    
    public void AddPlayer(object sender , PlayerEventArgs e)
    {
        PlayersAtCheckPoints.Add(e.playerController, CheckPointsTransform.GetChild(0));
    }

    public void OpenDoorByDoorButton(DoorButton doorButton)
    {
        if (doorButtons == null) return;
        int index = 0;
        foreach(Transform button in doorButtons.transform)
        {
            if(button.GetComponent<DoorButton>() == doorButton)
            {
                OpenDoorByButtonIndexServerRpc(index);
                break;
            }
            index++;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void OpenDoorByButtonIndexServerRpc(int index)
    {
        OpenDoorByButtonIndexClientRpc(index);
    }
    [ClientRpc]
    public void OpenDoorByButtonIndexClientRpc(int index)
    {
        DoorButton doorButton = doorButtons.GetChild(index).GetComponent<DoorButton>();
        if (doorButton != null)
        {
            doorButton.OpenDoor();
        }
    }

    [ServerRpc(RequireOwnership =false)]
    public void SetFloorActiveServerRpc(bool isActive)
    {
        SetFloorActiveClientRpc(isActive);
    }

    [ClientRpc]
    public void SetFloorActiveClientRpc(bool isActive) 
    {
        if (floorActive == null) return;
        foreach (GameObject floor in floorActive.GetComponent<FloorActiveHandler>().allFloors)
        {
            floor.SetActive(isActive);
        }
    }
    private void CheckPlayersFallOff()
    {

        foreach(Transform player in PlayerTools.players)
        {
            if (player == null) continue;
            PlayerController playerController = player.GetComponent<PlayerController>();
            if(playerController == null) continue;
            if (playerController.transform.position.y < transform.position.y - DropHeight)
            {
                Debug.Log("checkFallOff");
                Vector3 DropPosition = playerController.transform.position;
                Vector3 pos = PlayersAtCheckPoints[playerController].position;
                playerController.transform.GetComponent<CharacterController>().enabled = false;
                playerController.transform.position = pos;
                playerController.velocity = Vector3.zero;
                playerController.transform.rotation = Quaternion.identity;
                playerController.RelativeSpeed = Vector3.zero;
                playerController.transform.GetComponent<CharacterController>().enabled = true;
            }
        }
    }
    private void CheckPlayersAtWhichCheckPoints()
    {
        foreach(var player in PlayerTools.players)
        {
            if (player == null) continue;
            foreach (Transform checkPoint in CheckPointsTransform)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController == null) continue;
                CheckPointDetail checkPointDetail = checkPoint.GetComponent<CheckPointDetail>();
                Vector2 checkPointBounds = checkPointDetail.checkPointBounds;
                Vector2 checkPointCenterOffset = new Vector2(checkPointDetail.CenterOffset.x, checkPointDetail.CenterOffset.z);
                Vector3 playerPosition = playerController.transform.position;
                Vector2 PlayerPosProjectToXZ = new Vector2(playerPosition.x, playerPosition.z);
                Vector2 rectPositionXZ = new Vector2(checkPoint.position.x, checkPoint.position.z) + checkPointCenterOffset;
                Rect checkPointBoundsRect = new Rect(rectPositionXZ - checkPointBounds / 2, checkPointBounds);
                if (checkPointBoundsRect.Contains(PlayerPosProjectToXZ) && player.GetComponent<PlayerController>().m_PlayerAnimator.isGrounded)
                {
                    PlayersAtCheckPoints[playerController] = checkPoint;
                    Debug.Log("CheckPoint:" + checkPoint);
                }
            }
        }
        
    }
    private void OnDestroy()
    {
        CancelInvoke(nameof(UpdateChecks));
        UnityTools.Instance.AddPlayerEvent -= AddPlayer;
    }
    //[ServerRpc(RequireOwnership = false)]
    //public void GetAllControllersServerRpc(ulong clientId)
    //{
    //    serverPlayerControllers = PlayerTools.GetAllPlayerControllers();
    //    SetAllControllersClientRpc(clientId);
    //}
    //[ClientRpc]
    //public void SetAllControllersClientRpc(ulong clientId)
    //{
    //    PlayerTools.SetAllPlayerControllers(serverPlayerControllers);
    //}
    //[ServerRpc(RequireOwnership = false)]
    //public void AddPlayerControllerServerRpc(ulong clientId)
    //{
    //    PlayerController playerController = PlayerTools.FindPlayerController(clientId);
    //    if (playerController != null)
    //    {
    //        PlayerTools.AddPlayerController(playerController);
    //    }
    //    GetAllControllersServerRpc(clientId);
    //}
    //[ServerRpc(RequireOwnership = false)]
    //private void RemovePlayerControllerServerRpc(ulong clientId)
    //{
    //    PlayerController playerController = PlayerTools.FindPlayerController(clientId);
    //    if (playerController != null)
    //    {
    //        PlayerTools.RemovePlayerController(playerController);
    //    }
    //    GetAllControllersServerRpc(clientId);
    //}

    //[ServerRpc(RequireOwnership = false)]
    //public void RemoveAllPlayerControllersServerRpc()
    //{
    //    if (NetworkManager.Singleton.IsServer)
    //    {
    //        PlayerTools.ClearPlayerController();
    //    }
    //    else
    //    {
    //        RemoveAllPlayerControllersClientRpc();
    //    }

    //}
    //[ClientRpc]
    //public void RemoveAllPlayerControllersClientRpc()
    //{
    //    PlayerTools.ClearPlayerController();
    //}

}
