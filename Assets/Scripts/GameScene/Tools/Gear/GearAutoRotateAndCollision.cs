using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class GearAutoRotateAndCollision : MonoBehaviour
{
    public float rotateSpeed;

    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused
            && !GameManager.Instance.GameOnLoad)
        {
            return;
        }
        float rotationZ = transform.rotation.eulerAngles.z + rotateSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,
            rotationZ);
    }
    public Vector3 GetOnGearSpeed(ControllerColliderHit hit,PlayerController playerController)
    {
        //这里的playerTransform 只调整旋转！！
        Transform player = playerController.transform;        
        Vector3 OnGearSpeed = Vector3.zero;
        if (!hit.transform.name.Contains("Gear"))
        {
            return default;
        }
        Vector3 collisionPosition = hit.point;
        Vector3 gearCenterPos = hit.transform.position;
        gearCenterPos.y = hit.point.y;
        Vector3 DirectionToGearCenter = (hit.point - gearCenterPos);
        //transform 旋转
        float rotateSpeedToRag = rotateSpeed * Mathf.PI / 180 * 2;
        Vector3 rotationAngle = transform.rotation.eulerAngles;
        float rotationDelta = rotateSpeed  * Time.deltaTime;

        transform.rotation = Quaternion.Euler(rotationAngle.x,
            rotationAngle.y + rotationDelta / 2,
            rotationAngle.z);
        //player 旋转
        Vector3 angles = player.rotation.eulerAngles;
        playerController.transform.rotation = Quaternion.Euler(angles.x, angles.y + rotationDelta * 2, angles.z);
        
        Vector3 moveDirection;
        if (rotateSpeedToRag > 0)
        {
            moveDirection = new Vector3(DirectionToGearCenter.z, 0, -DirectionToGearCenter.x).normalized;
        }
        else
        {
            moveDirection = new Vector3(-DirectionToGearCenter.z, 0, DirectionToGearCenter.x).normalized;
        }
        float distance = Vector3.Distance(collisionPosition, gearCenterPos);
        float speed = rotateSpeedToRag * distance;
        OnGearSpeed.x = Mathf.Clamp(moveDirection.x * speed, -160f, 160f);
        OnGearSpeed.z = Mathf.Clamp(moveDirection.z * speed, -160f, 160f);
        return OnGearSpeed;
    }
}
