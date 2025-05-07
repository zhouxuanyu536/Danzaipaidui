using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.UIElements;
public enum MoveType
{
    Right, Left, Forward,Backward,Up,Down,Static,Revolution, //Rotation 自转，Revolution 公转
    NoMove
}
public class TrailElevatorAutoMove : NetworkBehaviour
{
    public MoveType primaryMoveType;
    public float moveSpeed;
    public bool isDoubleDirection;
    public bool GoOrReturn;
    public List<Transform> FBUpAndDownTransform; //Forward and backward

    [SerializeField] private MoveType ChangedPrimaryMoveType;
    [SerializeField] private string ChangeAction;
    [HideInInspector] public bool isCircleClockWise;
    [HideInInspector] public Vector3 CircleCenter;
    [HideInInspector] public float CircleRadius;
    public float CircleRotateSpeed;
    public List<Transform> players;
    public List<Transform> LRUpAndDownTransform; //Left and right
    public bool CanRotateStatic;
    public bool CanRotateRevolution;
    public float rotationDuration; 
    public Vector3 moveSpeedVector;
    [SerializeField] private GameObject CircleCenterGameObject;

    public MoveType moveType;

    public float rotationDelta;
    public bool isStaticRotationClockwise;


    private NetworkVariable<bool> CanMove = new NetworkVariable<bool>(true);
    // Start is called before the first frame update
    void Start()
    {
        GoOrReturn = true;
        moveType = primaryMoveType;
        if(primaryMoveType == MoveType.NoMove && NetworkManager.Singleton.IsServer)
        {
            CanMove.Value = false;
        }
        moveSpeedVector = Vector3.zero;
        players = new List<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused && !GameManager.Instance.GameOnLoad)
        {
            return;
        }
        else
        {
            
        }
        Vector3 transformOffset = Vector3.zero;
        if(moveType != MoveType.Revolution && transform.parent == CircleCenterGameObject)
        {
            transform.SetParent(CircleCenterGameObject.transform.parent);
            CircleCenterGameObject.transform.rotation = Quaternion.identity;
        }
        if(moveType == MoveType.NoMove)
        {
            ExecuteChangeAction();
            return;
        }
        if(moveType == MoveType.Static)
        {
            CircleCenter = transform.position;
        }
        else if(moveType == MoveType.Left)
        {
            transformOffset = new Vector3(-moveSpeed * Time.deltaTime, 0, 0);
        }
        else if(moveType == MoveType.Right)
        {
            transformOffset = new Vector3(moveSpeed * Time.deltaTime, 0, 0);
        }
        else if(moveType == MoveType.Forward)
        {
            transformOffset = new Vector3(0, 0, moveSpeed * Time.deltaTime);
        }
        else if(moveType == MoveType.Backward)
        {
            transformOffset = new Vector3(0, 0, -moveSpeed * Time.deltaTime);
        }
        else if(moveType == MoveType.Up)
        {
            transformOffset = new Vector3(0, moveSpeed * Time.deltaTime, 0);
        }
        else if(moveType == MoveType.Down)
        {
            transformOffset = new Vector3(0, -moveSpeed * Time.deltaTime, 0);
        }
        if(players.Count != 0)
        {
            if(moveType != MoveType.Static && moveType != MoveType.Revolution)
            {
                transform.localPosition += transformOffset;
            }
            foreach(Transform player in players)
            {
                Vector3 newCircleCenter = new Vector3(CircleCenter.x, player.position.y, CircleCenter.z);
                Vector3 offset = player.position - newCircleCenter;
                Vector3 worldPosition = Vector3.zero;
                if (moveType != MoveType.Static && moveType != MoveType.Revolution)
                {
                    worldPosition = player.position + transformOffset * 20;
                }
                else if (moveType == MoveType.Revolution)
                {
                    RotateAroundCircle(false);
                    worldPosition = Quaternion.Euler(0, rotationDelta, 0) * offset + newCircleCenter;
                    if (CanRotateRevolution)
                    {
                        Vector3 angles = player.rotation.eulerAngles;
                        player.rotation = Quaternion.Euler(angles.x, angles.y + rotationDelta, angles.z);
                    }
                }
                else if (CanRotateStatic)
                {
                    worldPosition = Quaternion.Euler(0, rotationDelta * 2.5f, 0) * offset + newCircleCenter;
                    Vector3 angles = player.rotation.eulerAngles;
                    player.rotation = Quaternion.Euler(angles.x, angles.y + rotationDelta, angles.z);
                }
                moveSpeedVector = (worldPosition - player.position) * 2.5f;
                player.GetComponent<PlayerController>().SetMoveSpeedVector(moveSpeedVector);
            }
        }
        else if(moveType != MoveType.Static && moveType != MoveType.Revolution && moveType != MoveType.NoMove)
        {
            transform.localPosition += transformOffset;
        }
        else if(moveType == MoveType.Revolution)
        {
            RotateAroundCircle(false);
        }
        

    }
    public void SetMoveType(MoveType moveType,NodeOfTrail nodeOfTrail)
    {
        this.moveType = moveType;
        if(isDoubleDirection && nodeOfTrail != null && nodeOfTrail.CanChangeDirection)
        {
            nodeOfTrail.nodeCheckType = (NodeCheckDirection)(-(int)nodeOfTrail.nodeCheckType);
            nodeOfTrail.nodeChangeDirection = (NodeChangeDirection)(-(int)nodeOfTrail.nodeChangeDirection);
        }
    }
    private void RotateAroundCircle(bool isStatic = false)
    {
        if (isStatic)
        {
            CircleCenter = transform.position;
        }
        CircleCenterGameObject.transform.position = CircleCenter;
        if(transform.parent != CircleCenterGameObject)
        {
            transform.SetParent(CircleCenterGameObject.transform);
        }
        //顺 / 逆时针旋转 这里node Scale x y z 一样
        // 令Elevator在 圆环上的移动速度放慢至0.6倍
        CircleRotateSpeed = moveSpeed * 0.6f * Mathf.Rad2Deg / (CircleRadius * 0.05f);
        rotationDelta = isCircleClockWise ? CircleRotateSpeed * Time.deltaTime : -CircleRotateSpeed * Time.deltaTime;
        Vector3 circleRotation = CircleCenterGameObject.transform.rotation.eulerAngles;
        circleRotation.y += rotationDelta;
        CircleCenterGameObject.transform.rotation = Quaternion.Euler(circleRotation);
        if (!CanRotateRevolution)
        {
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.y -= rotationDelta;
            transform.rotation = Quaternion.Euler(rotation);
        }
        return;
    }
    public Vector3 GetOnTrailElevatorSpeed(float SpeedMultiplier)
    {
        return moveSpeedVector * SpeedMultiplier;
    }
    //OnPlayerOnBoard 不要删
    private void OnPlayerOnBoard()
    {
        if (CanMove.Value || players.Count != 0)
        {
            SetCanMoveServerRpc(true);
            moveType = ChangedPrimaryMoveType;
        } 
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetCanMoveServerRpc(bool canMove)
    {
        CanMove.Value = canMove;
    }
    private void ExecuteChangeAction()
    {
        if (string.IsNullOrEmpty(ChangeAction))
        {
            return;
        }
        //反射
        MethodInfo method = GetType().GetMethod(ChangeAction, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (method != null)
        {
            method.Invoke(this, null); 
        }
        else
        {

        }
        return;
    }

}
