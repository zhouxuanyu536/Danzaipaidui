using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    public Transform target;
    [SerializeField] private Vector3 offset;
    public bool followRotation;
    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isPaused)
        {
            return;
        }
        if (target != null)
        {
            transform.position = target.position + offset;
            if (followRotation)
            {
                transform.rotation = target.rotation;
            }
        }
        
    }
}
