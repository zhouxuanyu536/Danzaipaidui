using UnityEngine;

public class PlayerRotateCenter : MonoBehaviour
{
    public Transform Player;
    private PlayerController playerController;
    private FollowTarget followTarget;

    
    void Start()
    {
        if (Player == null)
        {
            Debug.LogError("Player 不能为空！");
        }
        if(Player.GetComponent<PlayerController>() == null)
        {
            Debug.LogError("Player Controller不能为空！");
        }

        playerController = Player.GetComponent<PlayerController>();
        followTarget = GetComponent<FollowTarget>();
        followTarget.followRotation = true; //跟随旋转
    }
    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isPaused)
        {
            return;
        }
        if (playerController.isHitTrailElevator) {
            followTarget.target = playerController.playerCollideElevator.transform;
            Player.SetParent(transform);
        }
        else
        {
            followTarget.target = null;
            Player.SetParent(transform.parent);
        }
    }
    
    
}
