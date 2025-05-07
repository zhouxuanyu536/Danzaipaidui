using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeCheckDirection //指定当Elevator中心到达物体的什么位置之后开始转弯
{
    Right = 3, Left = -3, Forward = 2, Backward = -2, Up = 1, Down = -1, Circle = 0
}
public enum NodeChangeDirection //指定转弯的方向
{
    Right = 3, Left = -3, Forward = 2, Backward = -2, Up = 1, Down = -1, Circle = 0
}
public class NodeOfTrail : MonoBehaviour
{
    public NodeCheckDirection nodeCheckType;
    public NodeChangeDirection nodeChangeDirection;
    [SerializeField] private Transform TrailElevator;
    [SerializeField] private List<Transform> TrailElevatorUpAndDownTransform;
    private Transform CircleFromTransform;
    [SerializeField] private Transform CircleToTransform;
    [SerializeField] private bool isClockWise;
    [SerializeField] private bool isPlayerClockWise;
    [SerializeField] private float supposedToRotate;
    [HideInInspector] public float CircleRadius;
    private Vector3 CircleCenter;
    public bool CanChangeDirection;
    public bool isValid;
    private TrailElevatorAutoMove autoMove;
    private float rotationDuration;

    List<Tween> allTweens;

    NodeOfTrail AnotherNode;

    bool modify;

    private float StaticLastAngle;
    // Start is called before the first frame update
    void Start()
    {
        modify = false;
        StaticLastAngle = 0f;
        if (!TrailElevator.gameObject.name.Contains("TrailElevator"))
        {
        }
        autoMove = TrailElevator.GetComponent<TrailElevatorAutoMove>();
        rotationDuration = TrailElevator.GetComponent<TrailElevatorAutoMove>().rotationDuration;
        CircleFromTransform = transform;
        CircleCenter = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        isValid = true;
        if(nodeCheckType == NodeCheckDirection.Left ||  nodeCheckType == NodeCheckDirection.Right)
        {
            SetUpAndDownTransform(true);
        }
        else
        {
            SetUpAndDownTransform(false);
        }
        if (supposedToRotate == 0f)
        {
            supposedToRotate = 90;
        }
        allTweens = new List<Tween>();
    }




    private void SetUpAndDownTransform(bool isLeftOrRight)
    {
        if (!isLeftOrRight)
        {
            if (autoMove.FBUpAndDownTransform == null)
            {
                TrailElevatorUpAndDownTransform.Clear();
                TrailElevatorUpAndDownTransform.Add(TrailElevator.transform);
            }
            else
            {
                TrailElevatorUpAndDownTransform = autoMove.FBUpAndDownTransform;
            }
        }
        else
        {
            if (autoMove.LRUpAndDownTransform == null)
            {
                TrailElevatorUpAndDownTransform.Clear();
                TrailElevatorUpAndDownTransform.Add(TrailElevator.transform);
            }
            else
            {
                TrailElevatorUpAndDownTransform = autoMove.LRUpAndDownTransform;
            }
        }
    }
    private Vector3 CalcCircleCenter()
    {
        //顺时针 clockwise 逆时针 counterclockwise
        if(CircleFromTransform !=  null && CircleToTransform != null)
        {
            Vector3 P1 = CircleFromTransform.position;
            Vector3 P2 = CircleToTransform.position;

            Vector3 M = (P1 + P2) / 2;

            Vector3 D = P2 - P1;
            //弦长
            float chordLength = D.magnitude;

            float radius = chordLength / Mathf.Sqrt(2);

            Vector3 perpendicularDir;
            if(isClockWise)
            {
                perpendicularDir = new Vector3(D.z, 0, -D.x);
            }
            else
            {
                perpendicularDir = new Vector3(-D.z, 0, D.x);
            }

            //归一化
            perpendicularDir.Normalize();

            Vector3 circleCenter = M + perpendicularDir * chordLength / 2;

            return circleCenter;

        }
        return Vector3.zero;
    }
    private void NodeChangeDirectionCircle()
    {
        if (CircleCenter.x == float.MinValue)
        {
            CircleCenter = CalcCircleCenter();
            autoMove.CircleCenter = CircleCenter;
            CircleRadius = Vector3.Distance(CircleCenter,transform.position);
            autoMove.CircleRadius = CircleRadius;
            //对于另外一个点一样如此
            
            if ((AnotherNode = CircleToTransform.GetComponent<NodeOfTrail>()) != null)
            {
                AnotherNode.CircleCenter = CircleCenter;
                AnotherNode.CircleRadius = CircleRadius;
            }
            else
            {
               
            }
        }
        else
        {
            CalcCircleCenter();
            autoMove.CircleCenter = CircleCenter;
            autoMove.CircleRadius = CircleRadius;
        }
        Vector3 CenterToStartPos = transform.position - CircleCenter;
        Vector3 CenterToEndPos = CircleToTransform.position - CircleCenter;
        Vector3 cross = Vector3.Cross(CenterToStartPos, CenterToEndPos);
        if (cross.y < 0){
            autoMove.isCircleClockWise = false; //逆时针
        }
        else if(cross.y > 0)
        {
            autoMove.isCircleClockWise = true; //顺时针
        }
        else
        {
            autoMove.isCircleClockWise = true;
        }
        
        autoMove.SetMoveType(MoveType.Revolution, this); //绕点公转
    }
    private void OnTriggerExit(Collider obj)
    {
        if (obj.transform.name.Contains("TrailElevator"))
        {
            isValid = true;
            if ((int)nodeCheckType == -(int)nodeChangeDirection)
            {
                autoMove.GoOrReturn = !autoMove.GoOrReturn;
            }
            else if (CanChangeDirection)
            {
                int k = (int)nodeCheckType;
                int l = (int)nodeChangeDirection;
                int m = k;
                k = l;
                l = m;
                k = -k;l = -l;
                nodeCheckType = (NodeCheckDirection)k;
                nodeChangeDirection = (NodeChangeDirection)l;
            }
        }
    }
    private void ModifyPlayerSpeed(Collider obj, Vector3 center = default, bool inverse = false, Transform floorCenter = null)
    {
        float currentAngleY = obj.transform.eulerAngles.y;
        float deltaTime = Time.deltaTime;

        bool clockwise = isClockWise;

        if(center == default)
        {
            center = transform.position;
        }
        else
        {
            center = autoMove.CircleCenter;
        }
        if (autoMove.players.Count != 0)
        {
            foreach(Transform player in autoMove.players)
            {
                Vector3 playerRotateRelativePos = player.position - center;
                Vector2 playerRotateRelativePosToV2 = new Vector2(playerRotateRelativePos.x, playerRotateRelativePos.z);
                Vector3 direction;
                if (clockwise)
                {
                    direction = new Vector3(playerRotateRelativePosToV2.y, 0, -playerRotateRelativePosToV2.x).normalized;

                }
                else
                {
                    direction = new Vector3(-playerRotateRelativePosToV2.y, 0, playerRotateRelativePosToV2.x).normalized;
                }

                autoMove.moveSpeedVector = direction;
            }
            
            

        }
    }
    private void OnTriggerStay(Collider obj)
    {
        if (obj.transform != TrailElevator)
        {
            return;
        }
        if (nodeChangeDirection == NodeChangeDirection.Circle && modify == true)
        {
            
            
        }
        if (!isValid)
            return;
        Vector3 localPos;
        localPos = obj.transform.localPosition;
        switch (nodeCheckType)
        {
            case NodeCheckDirection.Left:
                
                if ((nodeChangeDirection == NodeChangeDirection.Up
                    && TrailElevatorUpAndDownTransform[0].position.x < transform.position.x)
                    || (nodeChangeDirection == NodeChangeDirection.Down 
                    && TrailElevatorUpAndDownTransform[1].position.x < transform.position.x))
                {
                    autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()),this);
                    isValid = false;
                }
                else if(nodeChangeDirection != NodeChangeDirection.Up &&
                    nodeChangeDirection != NodeChangeDirection.Down 
                    && obj.transform.localPosition.x < transform.localPosition.x)
                {
                    
                    if(autoMove.CanRotateStatic && nodeChangeDirection != NodeChangeDirection.Circle)
                    {
                        //旋转
                        autoMove.moveSpeedVector = Vector3.zero;
                        //先获取autoMove的CircleRotateSpeed旋转角度
                        if (obj.transform.localScale.x > 1 || obj.transform.localScale.z > 1)
                        {
                            autoMove.SetMoveType(MoveType.Static, null);
                            obj.transform.localPosition = new Vector3(transform.localPosition.x, obj.transform.localPosition.y, obj.transform.localPosition.z);
                            
                            if (nodeChangeDirection == NodeChangeDirection.Forward)
                            {
                                //DOTWeen 代码
                                obj.transform.DORotate(new Vector3(0, 90, 0), rotationDuration)
                                    .OnUpdate(() =>
                                    {
                                        //逆时针
                                        autoMove.isStaticRotationClockwise = false;
                                        autoMove.rotationDelta = obj.transform.rotation.eulerAngles.y - StaticLastAngle;
                                        if(Mathf.Abs(autoMove.rotationDelta) > 10)
                                        {
                                            autoMove.rotationDelta = 0f;
                                        }
                                        //获取Static时的rotationDelta
                                        StaticLastAngle = obj.transform.rotation.eulerAngles.y;
                                    })
                                    .OnComplete(() => {

                                        autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                                    });

                            }
                            else if (nodeChangeDirection == NodeChangeDirection.Backward)
                            {
                                obj.transform.DORotate(new Vector3(0, -90, 0), rotationDuration)
                                 .OnUpdate(() =>
                                 {
                                     //逆时针
                                     autoMove.isStaticRotationClockwise = true;
                                     autoMove.rotationDelta = obj.transform.rotation.eulerAngles.y - StaticLastAngle;
                                     if (Mathf.Abs(autoMove.rotationDelta) > 10)
                                     {
                                         autoMove.rotationDelta = 0f;
                                     }
                                     //获取Static时的rotationDelta
                                     StaticLastAngle = obj.transform.rotation.eulerAngles.y;
                                 })
                                .OnComplete(() =>
                                {
                                    autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                                });
                            }
                            else
                            {
                                autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                            }
                        }
                        else
                        {
                            obj.transform.localPosition = new Vector3(transform.localPosition.x, obj.transform.localPosition.y, obj.transform.localPosition.z);
                            autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                        }
                    }
                    else if(nodeChangeDirection == NodeChangeDirection.Circle)
                    {
                        
                        NodeChangeDirectionCircle();
                        if (autoMove.players.Count != 0)
                        {
                            //ModifyPlayerSpeed(obj, CircleCenter, true,
                            //autoMove.playerTransform.GetComponent<PlayerController>().playerCollideElevator.transform);
                        }
                        modify = true;
                    }
                    else
                    {
                        obj.transform.localPosition = new Vector3(transform.localPosition.x, obj.transform.localPosition.y, obj.transform.localPosition.z);
                        autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                    }
                    
                    isValid = false;
                }
                break;
                
            case NodeCheckDirection.Right:
                if ((nodeChangeDirection == NodeChangeDirection.Up
                    && TrailElevatorUpAndDownTransform[1].position.x > transform.position.x)
                    || (nodeChangeDirection == NodeChangeDirection.Down
                    && TrailElevatorUpAndDownTransform[0].position.x > transform.position.x))
                {
                    autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                    isValid = false;
                }
                else if(nodeChangeDirection != NodeChangeDirection.Up &&
                    nodeChangeDirection != NodeChangeDirection.Down &&
                    obj.transform.localPosition.x > transform.localPosition.x)
                {
                    if(autoMove.CanRotateStatic && nodeChangeDirection != NodeChangeDirection.Circle)
                    {
                        
                        autoMove.moveSpeedVector = Vector3.zero;
                        if (obj.transform.localScale.x > 1 || obj.transform.localScale.z > 1)
                        {
                            autoMove.SetMoveType(MoveType.Static, null);
                            obj.transform.localPosition = new Vector3(transform.localPosition.x, obj.transform.localPosition.y, obj.transform.localPosition.z);

                            float targetRotationY = (nodeChangeDirection == NodeChangeDirection.Forward) ? -90 :
                                                    (nodeChangeDirection == NodeChangeDirection.Backward) ? 90 :
                                                    obj.transform.rotation.eulerAngles.y;
                            bool isClockwise = targetRotationY < 0;
                            Tween tween = obj.transform.DORotate(new Vector3(0, targetRotationY, 0), rotationDuration)
                            .OnUpdate(() =>
                            {
                                //逆时针
                                autoMove.isStaticRotationClockwise = isClockwise;
                                autoMove.rotationDelta = obj.transform.rotation.eulerAngles.y - StaticLastAngle;
                                if (Mathf.Abs(autoMove.rotationDelta) > 10)
                                {
                                    autoMove.rotationDelta = 0f;
                                }
                                //获取Static时的rotationDelta
                                StaticLastAngle = obj.transform.rotation.eulerAngles.y;
                            })
                            .OnComplete(() => {
                                autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                            });
                        }
                        else
                        {
                            obj.transform.localPosition = new Vector3(transform.localPosition.x, obj.transform.localPosition.y, obj.transform.localPosition.z);
                            autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                        }
                    }
                    else if(nodeChangeDirection == NodeChangeDirection.Circle)
                    {
                        NodeChangeDirectionCircle();
                        if(autoMove.players.Count != 0)
                        {
                            //ModifyPlayerSpeed(obj, CircleCenter, true,
                            //autoMove.playerTransform.GetComponent<PlayerController>().playerCollideElevator.transform);
                        }
                        
                        modify = true;
                    }
                    else
                    {
                        obj.transform.localPosition = new Vector3(transform.localPosition.x, obj.transform.localPosition.y, obj.transform.localPosition.z);
                        autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                    }
                    isValid = false;
                }
                
                break;

            case NodeCheckDirection.Forward:
                if ((nodeChangeDirection == NodeChangeDirection.Up
                    && TrailElevatorUpAndDownTransform[0].position.z > transform.position.z)
                    || (nodeChangeDirection == NodeChangeDirection.Down
                    && TrailElevatorUpAndDownTransform[1].position.z > transform.position.z)) 
                {
                    autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                    isValid = false;
                }
                else if(nodeChangeDirection != NodeChangeDirection.Up &&
                    nodeChangeDirection != NodeChangeDirection.Down &&
                    obj.transform.localPosition.z > transform.localPosition.z)
                {
                    //根据player的position决定moveSpeedVector
                    autoMove.moveSpeedVector = Vector3.zero;
                    if (autoMove.CanRotateStatic && nodeChangeDirection != NodeChangeDirection.Circle)
                    {
                        if (obj.transform.localScale.x > 1 || obj.transform.localScale.z > 1)
                        {
                            autoMove.SetMoveType(MoveType.Static, null);
                            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, transform.localPosition.z);

                            float targetRotationY = (nodeChangeDirection == NodeChangeDirection.Left || nodeChangeDirection == NodeChangeDirection.Right) ? 0 :
                                                    obj.transform.rotation.eulerAngles.y;
                            bool isClockwise = targetRotationY < 0;
                            obj.transform.DORotate(new Vector3(0, targetRotationY, 0), rotationDuration)
                            .OnUpdate(() =>
                            {
                                //逆时针
                                autoMove.isStaticRotationClockwise = isClockwise;
                                autoMove.rotationDelta = obj.transform.rotation.eulerAngles.y - StaticLastAngle;
                                if (Mathf.Abs(autoMove.rotationDelta) > 10)
                                {
                                    autoMove.rotationDelta = 0f;
                                }
                                //获取Static时的rotationDelta
                                StaticLastAngle = obj.transform.rotation.eulerAngles.y;
                            })
                            .OnComplete(() => {
                                autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                            });
                            
                        }
                        else
                        {
                            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, transform.localPosition.z);
                            autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                            //ModifyPlayerSpeed(obj, CircleCenter,true);
                            modify = true;
                        }
                        isValid = false;
                    }
                    else if(nodeChangeDirection == NodeChangeDirection.Circle)
                    {
                        NodeChangeDirectionCircle();
                        if (autoMove.players.Count != 0)
                        {
                            //ModifyPlayerSpeed(obj, CircleCenter, true,
                            //autoMove.playerTransform.GetComponent<PlayerController>().playerCollideElevator.transform);
                        }
                        modify = true;
                    }
                    else
                    {
                        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, transform.localPosition.z);
                        autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                        
                    }
                    isValid = false;
                }
                break;

            case NodeCheckDirection.Backward:
                if ((nodeChangeDirection == NodeChangeDirection.Up
                    && TrailElevatorUpAndDownTransform[1].position.z < transform.position.z)
                    || (nodeChangeDirection == NodeChangeDirection.Down
                    && TrailElevatorUpAndDownTransform[0].position.z < transform.position.z))
                {
                    autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                    isValid = false;
                }
                else if(nodeChangeDirection != NodeChangeDirection.Up && nodeChangeDirection != NodeChangeDirection.Down 
                    && obj.transform.localPosition.z < transform.localPosition.z)
                {
                    if(autoMove.CanRotateStatic && nodeChangeDirection != NodeChangeDirection.Circle)
                    {
                        autoMove.moveSpeedVector = Vector3.zero;
                        if (obj.transform.localScale.x > 1 || obj.transform.localScale.z > 1)
                        {
                            autoMove.SetMoveType(MoveType.Static, null);
                            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, transform.localPosition.z);

                            float targetRotationY = (nodeChangeDirection == NodeChangeDirection.Left || nodeChangeDirection == NodeChangeDirection.Right) ? 0 :
                                                    obj.transform.rotation.eulerAngles.y;
                            bool isClockwise = targetRotationY < 0;
                            obj.transform.DORotate(new Vector3(0, targetRotationY, 0), rotationDuration)
                             .OnUpdate(() =>
                             {
                                 //逆时针
                                 autoMove.isStaticRotationClockwise = isClockwise;
                                 autoMove.rotationDelta = obj.transform.rotation.eulerAngles.y - StaticLastAngle;
                                 if (Mathf.Abs(autoMove.rotationDelta) > 10)
                                 {
                                     autoMove.rotationDelta = 0f;
                                 }
                                 //获取Static时的rotationDelta
                                 StaticLastAngle = obj.transform.rotation.eulerAngles.y;
                             })
                             .OnComplete(() =>
                            {
                                autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                            });
                        }
                        else
                        {
                            obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, transform.localPosition.z);
                            autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                        
                        }
                    }
                    else if (nodeChangeDirection == NodeChangeDirection.Circle)
                    {
                        NodeChangeDirectionCircle();
                        if (autoMove.players.Count != 0)
                        {
                            //ModifyPlayerSpeed(obj, CircleCenter, true,
                            //autoMove.playerTransform.GetComponent<PlayerController>().playerCollideElevator.transform);
                        }
                        modify = true;
                    }
                    else
                    {
                        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, transform.localPosition.z);
                        autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                    }

                    isValid = false;

                }
                break;
            case NodeCheckDirection.Up:
                if (obj.transform.localPosition.y >
                    transform.localPosition.y)
                {
                    obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, transform.localPosition.y, obj.transform.localPosition.z);
                    autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                    isValid = false;
                }
                break;
            case NodeCheckDirection.Down:
                if (obj.transform.localPosition.y <
                    transform.localPosition.y)
                {
                    obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, transform.localPosition.y, obj.transform.localPosition.z);
                    autoMove.SetMoveType((MoveType)Enum.Parse(typeof(MoveType), nodeChangeDirection.ToString()), this);
                    isValid = false;
                }
                break;
            default:
                
                
                if (nodeChangeDirection == NodeChangeDirection.Forward)
                {
                    if (obj.transform.position.z >
                    transform.position.z)
                    {
                        ChangeRotationAngle(obj.gameObject);
                        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, transform.localPosition.z);
                        autoMove.SetMoveType(MoveType.Forward, this);
                        isValid = false;
                    }
                }
                else if(nodeChangeDirection == NodeChangeDirection.Right)
                {
                    if (obj.transform.position.x > 
                    transform.position.x)
                    {
                        ChangeRotationAngle(obj.gameObject);
                        obj.transform.localPosition = new Vector3(transform.localPosition.x, obj.transform.localPosition.y, obj.transform.localPosition.z);
                        autoMove.SetMoveType(MoveType.Right, this);
                        isValid = false;
                    }
                    
                }
                else if(nodeChangeDirection == NodeChangeDirection.Backward)
                {
                    if (obj.transform.position.z <
                    transform.position.z)
                    {
                        ChangeRotationAngle(obj.gameObject);
                        obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, transform.localPosition.z);
                        autoMove.SetMoveType(MoveType.Backward, this);
                        isValid = false;
                    }
                    
                }
                else if(nodeChangeDirection == NodeChangeDirection.Left)
                {
                    
                    if (obj.transform.position.x <
                    transform.position.x)
                    {
                        ChangeRotationAngle(obj.gameObject);
                        obj.transform.localPosition = new Vector3(transform.localPosition.x, obj.transform.localPosition.y, obj.transform.localPosition.z);
                        autoMove.SetMoveType(MoveType.Left, this);
                        isValid = false;
                    }
                    
                }
                break;
        }
        if (CircleToTransform != null && (AnotherNode = CircleToTransform.GetComponent<NodeOfTrail>()) != null)
        {
            if (!isValid)
            {
                AnotherNode.modify = false;
            }
        }
    }
    private void ChangeRotationAngle(GameObject obj)
    {
        Vector3 angle = obj.transform.rotation.eulerAngles;
        float[] thresholds = { 0f, -90f, 90f, -180f, 180f,-270f,270f,-360f,360f };

        float closestAngle = thresholds[0];
        float minDifference = Mathf.Abs(angle.y - closestAngle);

        foreach(float threshold in thresholds)
        {
            float difference = Mathf.Abs(angle.y - threshold);
            if(difference < minDifference)
            {
                minDifference = difference;
                closestAngle = threshold;
            }
        }
        obj.transform.SetParent(obj.transform.parent.parent);
        obj.transform.rotation = Quaternion.Euler(new Vector3(angle.x, closestAngle,angle.z));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
