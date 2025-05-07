using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TorusAutoRotate : MonoBehaviour
{
    //沿y轴顺时针旋转
    // Start is called before the first frame update
    [SerializeField] private float degreesSpeed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 angles = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(angles.x, angles.y + degreesSpeed * Time.deltaTime, angles.z);
    }
}
