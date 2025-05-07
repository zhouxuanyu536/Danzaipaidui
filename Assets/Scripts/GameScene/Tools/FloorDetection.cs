using UnityEngine;

public class FloorDetection : MonoBehaviour
{
    public float raycastHeight = 2f; // 射线的长度，用来检查玩家是否在地板砖上方
    public LayerMask playerLayer; // 玩家层，设置该层来区分玩家和其他物体
    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isPaused)
        {
            return;
        }
        // 获取地板砖的位置
        Vector3 floorPosition = transform.position;

        // 从地板砖位置向上发射射线
        RaycastHit hit;
    }
}
