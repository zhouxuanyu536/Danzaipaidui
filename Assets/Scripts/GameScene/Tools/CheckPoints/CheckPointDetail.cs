using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointDetail : MonoBehaviour 
{
    public Vector2 checkPointBounds;
    public Vector3 CenterOffset;
    [HideInInspector] public Vector3 Center;
    private void OnDrawGizmos()
    {
        Vector3 TopLeft = (transform.position + CenterOffset) + new Vector3(
            -checkPointBounds.x / 2, 0, checkPointBounds.y / 2);
        Vector3 TopRight = (transform.position + CenterOffset) + new Vector3(
            checkPointBounds.x / 2, 0, checkPointBounds.y / 2);
        Vector3 BottomLeft = (transform.position + CenterOffset) + new Vector3(
            -checkPointBounds.x / 2, 0, -checkPointBounds.y / 2);
        Vector3 BottomRight = (transform.position + CenterOffset) + new Vector3(
            checkPointBounds.x / 2, 0, -checkPointBounds.y / 2);
        Vector3[] lineSegments = new Vector3[]
        {
            TopLeft, TopRight,  // 左上 → 右上
            TopRight, BottomRight, // 右上 → 右下
            BottomRight, BottomLeft, // 右下 → 左下
            BottomLeft, TopLeft // 左下 → 左上
        };

        Gizmos.color = Color.red;
       
        Gizmos.DrawLineList(lineSegments);
    }

    // Update is called once per frame
    void Update()
    {
        Center = transform.position + CenterOffset;
    }
}
