using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FanAutoRotate : MonoBehaviour
{
    [SerializeField] private float RotateSpeed = 15f;
    [SerializeField] private bool isClockWise;
    private void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused && !GameManager.Instance.GameOnLoad)
        {
            return;
        }
        float angleZ = transform.rotation.eulerAngles.z + RotateSpeed * Time.deltaTime;
        if (!isClockWise)
        {
            angleZ = transform.rotation.eulerAngles.z - RotateSpeed * Time.deltaTime;
        }
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, angleZ) ;
    }
}
