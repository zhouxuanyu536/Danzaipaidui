using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CubeFollowGunTail : MonoBehaviour
{
    [SerializeField] private GameObject gunTail;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update()
    {
        if(gunTail != null)
        {
            transform.position = gunTail.transform.position;
        }
        
    }


}
