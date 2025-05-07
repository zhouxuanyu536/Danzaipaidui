using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Door : NetworkBehaviour
{
    bool status;
    float timeToClose;
    private float closeTime = 3f;

    private bool networkUpdate;

    private Tween tween;

    private Vector3 originalLocalPos;
    [SerializeField] private Vector3 openMove;
    [SerializeField] private float openMoveDuration;
    private float closeMoveDuration;
    // Start is called before the first frame update
    void Start()
    {
        timeToClose = 0f;
        originalLocalPos = transform.localPosition;
        closeMoveDuration = openMoveDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (status)
        {
            timeToClose += Time.deltaTime;
            if(timeToClose > closeTime)
            {
                Close();
            }
        }
    }

    public void Open()
    {
        if (!status)
        {
            status = true;
            transform.DOLocalMove(originalLocalPos + openMove, openMoveDuration);
        }
        else
        {
            clearTimeToClose();
        }
        
    }
    public void Close()
    {
        if (status)
        {
            status = false;
            clearTimeToClose();
            transform.DOLocalMove(originalLocalPos, closeMoveDuration);
        }
    }
    private void clearTimeToClose()
    {
        timeToClose = 0f;
    }
}
