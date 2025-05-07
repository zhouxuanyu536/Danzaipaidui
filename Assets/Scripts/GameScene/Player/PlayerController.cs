using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


/**
 *  FixedUpdate 对速度参数进行直接处理 controller.Move(velocity * Time.deltaTime)
 *  Move 通过OnObjectSpeed和moveInput可调节速度参数velocity 
 *  OnControllerColliderHit 调整OnObjectSpeed参数值
 **/
public class PlayerController : NetworkBehaviour
{
    //调试模式
    private bool DebugMode = true;

    //protected GameCanvasActionHandler actionHandler;

    private PlayerInputController playerInputController;
    public PlayerAnimator m_PlayerAnimator;
    public CharacterController controller;
    public Vector3 velocity;

    private Color playerColor;
    public string playerName { get; private set; }

    [SerializeField] private GameObject playerCamera;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpHeight = 50f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float drag = 1f;
    [SerializeField] private float gravity = 1f;
    private Camera mainCamera;
    //注意：所有与 speedBlock 相关的加速 必须加上 OriginalSpeedBlockSpeedMultiplier！
    [SerializeField] private float OriginalSpeedMultiplier = 1f; //下面有乘以10的代码
    [SerializeField] private Transform BarrierTransform;
    [SerializeField] private Vector3 MoveSpeedVector;
    

    private float SpeedMultiplier;
    private bool isJump;
    private bool isBoost;
    private bool CanDoubleJump = true;
    private float moveInputMultiplier = 1f;
    private float BoostMultiply = 1f;
    public bool isCollidedElasticBody { private set; get; }
    private Vector3 CollideElasticDirection;
    //碰撞速度带
    [HideInInspector]
    public bool isHitSpeedBlock = false;
    private Vector3 OnSpeedBlockSpeed = Vector3.zero;
    //碰撞齿轮
    public bool isHitRotateGear = false;
    private Vector3 OnGearSpeed = Vector3.zero;
    //碰撞传送带块
    public bool isHitTrailElevator = false;
    private Vector3 OnTrailElevatorSpeed = Vector3.zero;
    public TrailElevatorAutoMove playerCollideElevator;
    //碰撞门铃按钮
    public bool isHitDoorButton = false;

    //碰撞弹簧
    public bool isHitSpring = false;
    private bool BounceOnSpringCanDoubleJump;
    private SpringWatcher springWatcherMemory;

    //跟随物品下降速度
    private float FollowObjectDownSpeed;

    Vector3 saveInputMoveDirection;
    float targetAngle;

    //临时保存人物的碰撞法线y值
    private float hitNormalY;
    [SerializeField] private float hitNormalYThreshold; //当hitPointY大于此值时，相当于物体已经在地面上了

    public Vector2 bufferedMoveInput;

    public Vector3 RelativeSpeed; //用于Jump跳跃时储存速度
    private float RelativeSpeedSetToZero; //用于再次确认是否RelativeSpeed置零

    private bool isPlayerInit;
    private bool isPlayerEnabled;
    private GameCanvasActionHandler actionHandler;

    public Dictionary<ulong, bool> whoAreOnGround;

    public Material localHeadMaterial;
    public Material localArmAndLegMaterial;
    public Material localEyeMaterial;
    private Transform[] players;

    //玩家透明度
    private readonly float COLLIDE_PLAYER_TRANSPARENCY = 0.5f;

    //拖尾
    public TrailRenderer trailRenderer;

    private Coroutine jumpCoroutine;
    private void Awake()
    {
        isJump = false;

        isPlayerInit = false;
        isPlayerEnabled = false;

        m_PlayerAnimator = GetComponent<PlayerAnimator>();
        controller = GetComponent<CharacterController>();
        GameInstance.SetPlayerController(this);
        MoveSpeedVector = Vector3.zero;
        targetAngle = -181f;
        hitNormalY = 0;
        hitNormalYThreshold = 0.707f; //45度
        FollowObjectDownSpeed = 0;
        BounceOnSpringCanDoubleJump = true;
        playerInputController = UnityTools.Instance.GetPlayerInputController();
        playerInputController.Player.Jump.started += ctx => Jump();
        playerInputController.Player.Sprint.started += ctx => Sprint();
        playerInputController.Player.Enable();
        UnityTools.Instance.AddPlayer(this, transform);

        actionHandler = FindObjectOfType<GameCanvasActionHandler>();

        whoAreOnGround = new Dictionary<ulong, bool>();

        players = new Transform[1] { transform };
        trailRenderer.gameObject.SetActive(false);
        
    }


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (transform.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
        {
            UnityTools.Instance.SetPlayerInput(transform.GetComponent<PlayerInput>());
            UnityTools.Instance.SetLocalPlayerCamera(playerCamera.transform);
        }
        InformAllPlayersToUpdatePlayersListServerRpc();
        ChangeLocalPlayerServerRpc();
        Vector3 testBornPos = GameObject.Find("TestBornPos").transform.position;
        Vector3 offset = Vector3.zero;
        switch (GameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId))
        {
            case 0:
                offset = Vector3.zero;
                break;
            case 1:
                offset = new Vector3(15, 0, 0);
                break;
            case 2:
                offset = new Vector3(0, 0, -15);
                break;
            case 3:
                offset = new Vector3(15, 0, -15);
                break;
        }
        SetPlayerBornPositionServerRpc(testBornPos + offset);
        SetPlayerName(GameMultiplayer.Instance.GetPlayerDataFromNetworkList(OwnerClientId).playerName.ToString());
        SetPlayerColorClientRpc(GameMultiplayer.Instance.GetPlayerDataFromNetworkList(OwnerClientId).playerColor);
        GameManager.Instance.NotifyServerPlayerSpawnedServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void ChangeLocalPlayerServerRpc()
    {
        ChangeLocalPlayerClientRpc();
    }
    [ClientRpc]
    private void ChangeLocalPlayerClientRpc()
    {
        PlayerTools.ChangeLocalPlayer(NetworkManager.Singleton.LocalClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void InformAllPlayersToUpdatePlayersListServerRpc()
    {
        InformAllPlayersToUpdatePlayersListClientRpc();
    }
    [ClientRpc]
    private void InformAllPlayersToUpdatePlayersListClientRpc()
    {
        players = FindObjectsOfType<PlayerController>()
            .Select(x => x.transform).ToArray();
        PlayerTools.players.Clear();
        PlayerTools.connectClientsIdsIndexes.Clear();
        GameManager.Instance.progressBars.RemoveAllProgressBars();
        foreach (Transform player in players)
        {
            PlayerTools.players.Add(player);
            PlayerTools.connectClientsIdsIndexes.Add(player.GetComponent<NetworkObject>().OwnerClientId);
            //Update Progress Bar

            GameManager.Instance.progressBars.AddPlayerToProgressBar(player.GetComponent<NetworkObject>().OwnerClientId);

        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TellSprintServerRpc(bool isSprint)
    {
        TellSprintClientRpc(isSprint);
    }
    [ClientRpc]
    private void TellSprintClientRpc(bool isSprint)
    {
        try
        {
            trailRenderer.gameObject.SetActive(isSprint);
        }
        catch { }
    }
    /// <summary>
    /// 角色根据输入移动，并让FreeLookCamera跟随该目标
    /// </summary>
    public void OnPlayerEnabled()
    {
        if (isPlayerEnabled && isPlayerInit) return;
        playerCamera.SetActive(true);
        isPlayerInit=true;
        isPlayerEnabled = true;
    }
    public void OnPlayerDisabled()
    {
        if(!isPlayerEnabled && isPlayerInit) return; 
        playerCamera.SetActive(false);
        isPlayerInit = true;
        isPlayerEnabled = false;
    }
// Update is called once per frame
    void FixedUpdate()
    {
        if (!IsOwner)
        {
            CheckCharacterCollisions();
        }
        else
        {
            isPlayerInit = true;
            isPlayerEnabled = true;
        }
        Transform localPlayerCamera;
        if((localPlayerCamera = UnityTools.Instance.GetLocalPlayerCamera()) != null)
        {
            playerNameText.transform.LookAt(localPlayerCamera);
            playerNameText.transform.rotation = Quaternion.Euler(0, playerNameText.transform.rotation.eulerAngles.y + 180, 0);
        }
        else
        {
            //playerNameText.transform.rotation = Quaternion.identity;
        }

        if (mainCamera == null)
        {
            mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }

        if (GameManager.Instance != null && GameManager.Instance.isPaused && !GameManager.Instance.GameOnLoad)
        {
            m_PlayerAnimator.animatorSpeed = 0f;
            return;
        }
        else if(GameManager.Instance.isPaused && GameManager.Instance.GameOnLoad)
        {
            m_PlayerAnimator.animatorSpeed = 1f;
            return;
        }
        else
        {
            m_PlayerAnimator.animatorSpeed = 1f;
        }
        if (IsOwner)
        {
            isBoost = actionHandler.isBoost;
            TellSprintServerRpc(isBoost);
        }
        if (isBoost)
        {
            BoostMultiply = 1.5f;
            m_PlayerAnimator.animatorSpeed = 1.5f;
        }
        else
        {
            BoostMultiply = 1f;
            m_PlayerAnimator.animatorSpeed = 1f;
        }
        
        isCollidedElasticBody = false;

        if (!isCollidedElasticBody)
        {
            if (isPlayerEnabled)
            {
                Vector2 moveInput = playerInputController.Player.Move.ReadValue<Vector2>();
                //施加缓冲
                bufferedMoveInput = Vector2.MoveTowards(bufferedMoveInput, moveInput, 5f * Time.deltaTime);
            }
            HandleMovement(bufferedMoveInput);
        }
        else
        {
            m_PlayerAnimator.blendValue = 0;
            if (controller.isGrounded)
            {
                velocity.x = velocity.x * 0.98f;
                velocity.z = velocity.z * 0.98f;
                if (Mathf.Abs(velocity.x) < 0.01f) velocity.x = 0f;
                if (Mathf.Abs(velocity.z) < 0.01f) velocity.z = 0f;
            }
            if (controller.isGrounded &&
                new Vector2(controller.velocity.x, controller.velocity.z).magnitude <= 1.3f)
            {
                isCollidedElasticBody = false;
            }
        }
        
        





    }
    private void HandleMovement(Vector2 bufferedMoveInput)
    {
        if (gameObject.IsDestroyed()) return;
        Vector3 cameraRotation = mainCamera.transform.rotation.eulerAngles;
        SpeedMultiplier = 20f * OriginalSpeedMultiplier;
        Quaternion rotation = Quaternion.Euler(0, cameraRotation.y, 0);
        Vector3 input = Vector3.zero;
        if (controller.isGrounded || isHitTrailElevator)
        {
            input = new Vector3(bufferedMoveInput.x, 0, bufferedMoveInput.y);
            input = rotation * input * moveInputMultiplier;
        }
        else
        {
            //调试模式 DebugMode
            if (DebugMode)
            {
                input = new Vector3(bufferedMoveInput.x, 0, bufferedMoveInput.y);
                input = rotation * input * moveInputMultiplier;
            }

        }
        float blendValue = input.magnitude;
        if (blendValue > 0)
        {
            Quaternion targetRotation = Quaternion.Euler(0, Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            float targetVx = input.x * moveSpeed * transform.lossyScale.x * BoostMultiply;
            float targetVz = input.z * moveSpeed * transform.lossyScale.z * BoostMultiply;
            //优先级:传送带块 > 速度带 > 齿轮 > 普通地面
            if (isHitTrailElevator)
            {
                if(playerCollideElevator != null)
                { 
                    OnTrailElevatorSpeed = playerCollideElevator.GetOnTrailElevatorSpeed(SpeedMultiplier);
                }

                RelativeSpeed = OnTrailElevatorSpeed;
                AutoSpeedUpOrDown(new Vector3Int(1, 0, 1), new Vector3(RelativeSpeed.x + targetVx,
                        velocity.y, RelativeSpeed.z + targetVz));
                FollowObjectDownSpeed = OnTrailElevatorSpeed.y;
                if (playerCollideElevator != null)
                {
                    SetTrailElevatorPlayerRot(playerCollideElevator.transform, input, moveSpeed);
                }
            }
            if (isHitSpeedBlock)
            {
                RelativeSpeed = OnSpeedBlockSpeed;
                FollowObjectDownSpeed = OnSpeedBlockSpeed.y;
                AutoSpeedUpOrDown(new Vector3Int(2, 0, 2), new Vector3(RelativeSpeed.x + targetVx,
                        velocity.y, RelativeSpeed.z + targetVz));

            }
            else if(isHitRotateGear) {
                RelativeSpeed = OnGearSpeed;
                AutoSpeedUpOrDown(new Vector3Int(2, 0, 2), new Vector3(RelativeSpeed.x + targetVx,
                        velocity.y, RelativeSpeed.z + targetVz));
            }
            else
            {
                if (isJump)
                {
                    if (input.magnitude > 0.1f)
                    {
                        RelativeSpeed.x = Mathf.MoveTowards(RelativeSpeed.x, 0, drag * Time.deltaTime);
                        RelativeSpeed.z = Mathf.MoveTowards(RelativeSpeed.z, 0, drag * Time.deltaTime);
                    }
                    AutoSpeedUpOrDown(new Vector3Int(2, 0, 2), new Vector3(RelativeSpeed.x + targetVx, 
                        velocity.y, RelativeSpeed.z + targetVz));
                }
                else
                {
                    if (RelativeSpeedSetToZero < 2)
                    {
                        AutoSpeedUpOrDown(new Vector3Int(2, 0, 2), new Vector3(RelativeSpeed.x + targetVx,
                            velocity.y, RelativeSpeed.z + targetVz));
                        RelativeSpeedSetToZero += 1;
                    }
                    else
                    {
                        AutoSpeedUpOrDown(new Vector3Int(1, 0, 1), new Vector3(targetVx, velocity.y, targetVz));
                        RelativeSpeed = Vector3.zero;
                        RelativeSpeedSetToZero = 0;
                    }
                }
            }
        }
        else
        {
            if (isHitTrailElevator)
            {
                if (localPos != Vector3.zero)
                {
                    transform.position = playerCollideElevator.transform.TransformPoint(new Vector3(5 * localPos.x / 2,5 * localPos.y / 2,5 * localPos.z / 2));
                }
                //匀加速
                if (playerCollideElevator != null)
                {
                    OnTrailElevatorSpeed = playerCollideElevator.GetOnTrailElevatorSpeed(SpeedMultiplier);
                    RelativeSpeed = OnTrailElevatorSpeed;
                    FollowObjectDownSpeed = OnTrailElevatorSpeed.y;
     
                    AutoSpeedUpOrDown(new Vector3Int(2, 0, 2), OnTrailElevatorSpeed); //x轴和z轴
                    velocity.y += Physics.gravity.y * transform.lossyScale.y * Time.deltaTime * gravity;
                }
            }
            else if (isHitSpeedBlock)
            {
                AutoSpeedUpOrDown(new Vector3Int(2, 0, 2), OnSpeedBlockSpeed);
                RelativeSpeed = OnSpeedBlockSpeed;
            }
            else if (isHitRotateGear)
            {
                AutoSpeedUpOrDown(new Vector3Int(2, 0, 2), OnGearSpeed);
                RelativeSpeed = OnGearSpeed;
            }
            else
            {
                
                if (!isJump)
                {
                    if (RelativeSpeedSetToZero < 2)
                    {
                        RelativeSpeedSetToZero += 1;
                    }
                    else
                    {
                        RelativeSpeed = Vector3.zero;
                        RelativeSpeedSetToZero = 0;
                    }
                }
                if(input.magnitude > 0.1f)
                {
                    RelativeSpeed.x = Mathf.MoveTowards(RelativeSpeed.x, 0, drag * Time.deltaTime);
                    RelativeSpeed.z = Mathf.MoveTowards(RelativeSpeed.z, 0, drag * Time.deltaTime);
                }
                AutoSpeedUpOrDown(new Vector3Int(1, 0, 1), RelativeSpeed);
            }
        }
        m_PlayerAnimator.blendValue = blendValue;
        if (isHitSpring)
        {
            if(springWatcherMemory != null)
            {
                velocity.y = Mathf.Sqrt(springWatcherMemory.springJumpHeight * 2f * Constant.GRAVITY * transform.lossyScale.y * transform.lossyScale.y);
            }
            else
            {
                velocity.y = Mathf.Sqrt(30 * 2f * Constant.GRAVITY * transform.lossyScale.y * transform.lossyScale.y);
            }
            
            isJump = true;

            CanDoubleJump = BounceOnSpringCanDoubleJump;
            isHitSpring = false;
        }

        //增添重力
        velocity.y += Physics.gravity.y * transform.lossyScale.y * Time.deltaTime * gravity;

        if (hitNormalY > hitNormalYThreshold)
        {
            OnGround();
        }
        else
        {
            if (IsOwner)
            {
                m_PlayerAnimator.isGrounded = false;
            }
            //时间大于0.25s,则isJump为true
            jumpCoroutine = StartCoroutine(SetJumpCoroutine());
            isHitRotateGear = false;
            isHitSpeedBlock = false;
            isHitTrailElevator = false;
            isHitSpring = false;
            velocity.y = Mathf.Max(velocity.y, -Constant.GRAVITY * transform.lossyScale.y * gravity * 3f);
        }
        if (velocity.y <
            Physics.gravity.y * transform.lossyScale.y * gravity * 0.2f)
        {
            playerFreeFall();
        }
        controller.Move(velocity * Time.deltaTime);
        controller.Move(new Vector3(0, FollowObjectDownSpeed, 0) * Time.deltaTime);
        Debug.Log("isGrounded:" + controller.isGrounded);
        FollowObjectDownSpeed = 0f;
    }
    private IEnumerator SetJumpCoroutine()
    {
        yield return new WaitForSeconds(0.25f);
        isJump = true;
    }
    public float AutoSpeedUpOrDown(Vector3Int axis,Vector3 normalSpeed = default) //默认值 Vector3.zero
    {
        if(axis.x != 0)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, normalSpeed.x, drag * axis.x * Time.deltaTime);
        }
        if(axis.y != 0)
        {
            velocity.y = Mathf.MoveTowards(velocity.y, normalSpeed.y, drag * axis.y * Time.deltaTime);
        }
        if(axis.z != 0)
        {
            velocity.z = Mathf.MoveTowards(velocity.z, normalSpeed.z, drag * axis.z * Time.deltaTime);
        }
        
        Vector3 speedToInput = new Vector3((velocity.x - normalSpeed.x) / moveSpeed * transform.lossyScale.x,
            0, (velocity.z - normalSpeed.z) / moveSpeed * transform.lossyScale.z);
        return speedToInput.magnitude;

    }
    public void OnGround()
    {
        if (IsOwner)
        {
            m_PlayerAnimator.isFreeFall = false;
            m_PlayerAnimator.isJump = false;
            m_PlayerAnimator.isGrounded = true;
        }
        isJump = false;
        CanDoubleJump = true;
        moveInputMultiplier = 1f;
        hitNormalY = 0;
    }
    public void Jump()
    {
        if (!isPlayerEnabled) return;
        if (!isJump)
        {
            isJump = true;
            m_PlayerAnimator.isJump = true;
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * Constant.GRAVITY * transform.lossyScale.y * transform.lossyScale.y);
            moveInputMultiplier = 1f;
            FollowObjectDownSpeed = 0f;
        }
        else if(CanDoubleJump)
        {
            CanDoubleJump = false;
            m_PlayerAnimator.isJump = true;
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * Constant.GRAVITY * transform.lossyScale.y * transform.lossyScale.y);
            moveInputMultiplier = 0.8f;
            BounceOnSpringCanDoubleJump = false;
            FollowObjectDownSpeed = 0f;
        }
    }
    private void Sprint()
    {
        if(actionHandler != null)
        {
            actionHandler.SetBoost();
        }
    }
    Vector3 localPos;
    void CheckCharacterCollisions()
    {
        Transform player = transform;
        if (player == null || player == PlayerTools.GetLocalPlayer()) return;
        try
        {
            //如果与localPlayer相距太近，则隐身
            if (Mathf.Abs(PlayerTools.GetLocalPlayer().position.y - player.position.y) < 4)
            {
                
                if (Mathf.Abs(PlayerTools.GetLocalPlayer().position.x - player.position.x) < 3
                    && Mathf.Abs(PlayerTools.GetLocalPlayer().position.z - player.position.z) < 3)
                {
                    SetPlayerVisibility(player, COLLIDE_PLAYER_TRANSPARENCY);
                }
                else
                {
                    SetPlayerVisibility(player, 1);
                }
            }
            else
            {
                SetPlayerVisibility(player, 1);
            }
        }
        catch { }
        
    }
    
    private void SetPlayerVisibility(Transform player,float alpha)
    {
        if(player != null)
        {
            player.GetComponent<PlayerComp>().SetAlpha(alpha);
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormalY = hit.normal.y;
        
        //检测碰撞
        TrailElevatorAutoMove trailElevatorCollide = hit.transform.GetComponent<TrailElevatorAutoMove>();
        SpeedBlock speedBlockCollide = hit.transform.GetComponent<SpeedBlock>();
        GearAutoRotateAndCollision gearCollide = hit.transform.GetComponent<GearAutoRotateAndCollision>();
        SpringWatcher springWatcher = hit.transform.GetComponent<SpringWatcher>();
        DoorButton doorButton = hit.transform.GetComponent<DoorButton>();
        //碰撞传送带块
        if(trailElevatorCollide != null && hitNormalY > hitNormalYThreshold)
        {
            if (localPos == Vector3.zero)
            {
                localPos = transform.InverseTransformPoint(transform.position);
            }
            
            playerCollideElevator = trailElevatorCollide;
            if (!playerCollideElevator.GetComponent<TrailElevatorAutoMove>().players.Contains(transform))
            {
                playerCollideElevator.GetComponent<TrailElevatorAutoMove>().players.Add(transform);
            }
            
            isHitTrailElevator = true;
            OnTrailElevatorSpeed = trailElevatorCollide.GetOnTrailElevatorSpeed(SpeedMultiplier);
        }
        else
        {
            if(playerCollideElevator != null)
            {
                playerCollideElevator.GetComponent<TrailElevatorAutoMove>().players.Remove(transform);
            }
            isHitTrailElevator = false;

        }
        //碰撞速度带
        if (speedBlockCollide != null && hitNormalY > hitNormalYThreshold)
        {
            isHitSpeedBlock = true;
            float speed = 1f;
            SpeedBlockScaleWatcherSpeed scaleWatcherSpeed = speedBlockCollide.GetComponent<SpeedBlockScaleWatcherSpeed>();
            if(scaleWatcherSpeed != null)
            {
                speed = scaleWatcherSpeed.speed;
            }
            OnSpeedBlockSpeed = speedBlockCollide.GetOnSpeedBlockSpeed(this,speed, SpeedMultiplier);
        }
        else
        {
            isHitSpeedBlock = false;
        }

        //碰撞齿轮
        if (gearCollide != null && hitNormalY > hitNormalYThreshold)
        {
            OnGearSpeed = gearCollide.GetOnGearSpeed(hit,this);
            isHitRotateGear = true;
        }
        else
        {
            isHitRotateGear = false;
        }

        //碰撞弹簧
        if (springWatcher != null && hitNormalY > hitNormalYThreshold)
        {
            isHitSpring = true;
            springWatcherMemory = springWatcher;
        }
        else
        {
            isHitSpring = false;
            BounceOnSpringCanDoubleJump = true;
            springWatcherMemory = null;
        }
        //碰撞按钮
        if(doorButton != null)
        {
            isHitDoorButton = true;
            Debug.Log("doorButton");
            GameManager.Instance.levelDetails.OpenDoorByDoorButton(doorButton);
        }
        else
        {
            isHitDoorButton = false;
        }
        var detailBrick = hit.gameObject.GetComponent<DetailBrick>();
        if (detailBrick != null && detailBrick.GetIsElastic())
        {
            //若碰到弹性物体，则回弹
            isCollidedElasticBody = true;
            CollideElasticDirection = hit.normal;
            //令该方向的velocity为0
            float velocityAlongNormal = Vector3.Dot(velocity, CollideElasticDirection);
            velocity = velocity - velocityAlongNormal * CollideElasticDirection;
            float reboundStrength = 1.5f; // 反弹强度
            velocity += CollideElasticDirection * reboundStrength;
        }
        else
        {
            CollideElasticDirection = hit.normal;
            //令该方向的velocity为0
            float velocityAlongNormal = Vector3.Dot(velocity, CollideElasticDirection);
            velocity = velocity - velocityAlongNormal * CollideElasticDirection;
        }

    }
    private void playerFreeFall()
    {
        if (IsOwner)
        {
            m_PlayerAnimator.isGrounded = false;
            m_PlayerAnimator.isFreeFall = true;
        }
        
    }

    public void SetIsCollidedElasticBody(bool isCollidedElasticBody,Vector3 collideDirection)
    {
        this.isCollidedElasticBody = isCollidedElasticBody;
    }

    
    public void SetTrailElevatorPlayerRot(Transform elevator, Vector3 inputMoveDirection, float moveSpeed)
    {
        Quaternion newRotation = elevator.rotation;
        if (inputMoveDirection != Vector3.zero)
        {
            saveInputMoveDirection = inputMoveDirection;
            targetAngle = -181f;
        }
        else
        {
            if(targetAngle == -181f)
            {
                targetAngle = Mathf.Atan2(saveInputMoveDirection.x, saveInputMoveDirection.z) * Mathf.Rad2Deg
                    - newRotation.eulerAngles.y;
            }
            transform.rotation = newRotation
                    * Quaternion.Euler(0, targetAngle, 0);
        }
    }

    public void SetMoveSpeedVector(Vector3 MoveSpeedVector)
    {
        this.MoveSpeedVector = MoveSpeedVector;
    }

    public void SetPlayerName(string playerName)
    {
        playerNameText.text = playerName;
        this.playerName = playerName;
    }
    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerBornPositionServerRpc(Vector3 bornPos)
    {
        controller.Move(bornPos - transform.position);
    }
    //Material,仅在下面功能使用
    [SerializeField] private Material headMaterial;
    [SerializeField] private Material ArmAndLegMaterial;
    [SerializeField] private Material EyeMaterial;

 
    [ClientRpc]
    public void SetPlayerColorClientRpc(Color color)
    {
        // 实例化材质，防止共享材质被修改
        PlayerComp comp = transform.GetComponent<PlayerComp>(); 
        comp.Head.GetComponent<Renderer>().material = headMaterial;
        comp.Head.GetComponent<Renderer>().material.SetColor("_Color", color);
        trailRenderer.sharedMaterial = comp.Head.GetComponent<Renderer>().material;
        foreach (Transform c in comp.ArmsAndLegs)
        {
            c.GetComponent<Renderer>().material = ArmAndLegMaterial;
            c.GetComponent<Renderer>().material.color = color;
        }
        foreach(Transform e in comp.Eyes)
        {
            e.GetComponent<Renderer>().material = EyeMaterial;
            if(color.r > 1)
            {
                color.r = color.r / 255;
                color.g = color.g / 255;
                color.b = color.b / 255;
            }
            if (color == Color.red || color == Color.blue)
                e.GetComponent<Renderer>().material.color = Color.white;
            else
                e.GetComponent<Renderer>().material.color = Color.black;
        }
        playerNameText.color = color;
        if (playerNameText.color == new Color(1, 1, 0, 1))
        {
            playerNameText.color = new Color(191f / 255f, 191f / 255f, 0, 1);
        }
        
    }

    public void OnDestroy()
    {
        isPlayerInit = false;
        isPlayerEnabled = false;
        
        UnityTools.Instance.SetLocalPlayerCamera(null);
    }
}
