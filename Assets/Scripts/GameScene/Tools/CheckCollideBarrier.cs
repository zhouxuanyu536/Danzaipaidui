using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollideBarrier : MonoBehaviour
{
    private void OnTriggerEnter(Collider obj)
    {
        var followTarget = obj.GetComponent<FollowTarget>();
        if (followTarget != null && followTarget.target.GetComponent<PlayerController>() != null &&
            followTarget.target.GetComponent<PlayerController>().isHitSpeedBlock)
        {
            followTarget.target.GetComponent<PlayerController>()
                .velocity = Vector3.zero;
            followTarget.target.GetComponent<PlayerController>()
                .isHitSpeedBlock = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
