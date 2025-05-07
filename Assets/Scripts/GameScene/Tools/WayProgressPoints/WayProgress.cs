using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class WayProgress : MonoBehaviour
{

    private List<WayProgressPoint> points;

    private void Start()
    {
        points = new List<WayProgressPoint>();
        foreach(Transform wayProgressPoint in transform)
        {
            WayProgressPoint wPoint;
            if((wPoint = wayProgressPoint.GetComponent<WayProgressPoint>()) == null)
            {
                wPoint = wayProgressPoint.AddComponent<WayProgressPoint>();
            }
            points.Add(wPoint);
        }
    }
    private void Update()
    {
        
    }

    public float GetPlayerProgress(PlayerController playerController)
    {
        Debug.Log("points:" + points.Count);
        if (points.Count == 0) return 0f;
        else if (points.Count == 1) return 1f;
        //ProgressPoint连线，寻找Player 与 ProgressPoint 最近的点
        Vector3 closestPoint = Vector3.zero;
        float distance = Mathf.Infinity;
        Vector3 PlayerPos = playerController.transform.position;
        int MinDistancePointNum = 1;
        for (int i = 1; i < points.Count; i++) 
        {
            Vector3 PointIClosePoint;
            float dist = PointToSegmentDistance(PlayerPos, points[i].transform.position,
                points[i - 1].transform.position, out PointIClosePoint);
            if(distance > dist)
            {
                distance = dist;
                closestPoint = PointIClosePoint;
                MinDistancePointNum = i;
            }
        }
        float pointAProgress = points[MinDistancePointNum - 1].OnPointLineProgress;
        float pointBProgress = points[MinDistancePointNum].OnPointLineProgress;
        return Mathf.Lerp(pointAProgress,
            pointBProgress,
            (closestPoint - points[MinDistancePointNum - 1].transform.position).magnitude 
            / (points[MinDistancePointNum].transform.position - points[MinDistancePointNum - 1].transform.position).magnitude);

        
    }
    public static float PointToSegmentDistance(Vector3 p, Vector3 a, Vector3 b, out Vector3 closestPoint)
    {
        Vector3 ab = b - a;
        Vector3 ap = p - a;

        float abLengthSq = ab.sqrMagnitude; // |AB|^2
        float dotProduct = Vector3.Dot(ap, ab); // (P-A)·(B-A)
        float t = dotProduct / abLengthSq; // 投影比例

        if (t < 0) t = 0; // 限制在 [0,1]
        if (t > 1) t = 1;

        closestPoint = a + t * ab; // 计算投影点 Q
        return Vector3.Distance(p, closestPoint); // 计算距离
    }
}
