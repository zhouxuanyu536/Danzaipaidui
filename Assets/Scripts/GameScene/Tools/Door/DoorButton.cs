using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    [SerializeField] private Door door;

    [SerializeField] private Vector3 buttonDownDist;

    private Vector3 originalLocalPos;
    private bool isButtonDown;
    private float timeToButtonUp;
    [SerializeField] private float buttonDownTime;
    private float buttonUpTime;

    private Tween buttonTween;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private LevelDetails levelDetails;
    private void Start()
    {
        if(buttonDownDist == Vector3.zero)
        {
            buttonDownDist = new Vector3(0, -0.07f, 0);
        }
        originalLocalPos = transform.localPosition;
        timeToButtonUp = 0f;
        buttonUpTime = buttonDownTime;
    }
    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.isPaused) 
        {
            buttonTween.Pause();
            return;
        }
        else
        {
            buttonTween.Play();
        }
        if (isButtonDown)
        {
            timeToButtonUp += Time.deltaTime;
            if(timeToButtonUp > buttonDownTime)
            {
                isButtonDown = false;
                ButtonUpAuto();
                ClearTimeToButtonUp();
            }
        }
        Debug.Log("isButtonDown:" + isButtonDown);
    }
    public void OpenDoor()
    {
        door.Open();
        if (!isButtonDown)
        {
            buttonTween.Kill();
            buttonTween = transform.DOLocalMove(originalLocalPos + buttonDownDist, buttonDownTime);
        }
        else
        {
            ClearTimeToButtonUp();
        }
        isButtonDown = true;
    }

    public void ButtonUpAuto()
    {
         ButtonUpAutoClientRpc();
    }
    [ClientRpc]
    private void ButtonUpAutoClientRpc()
    {
        buttonTween.Kill();
        buttonTween = transform.DOLocalMove(originalLocalPos, buttonUpTime);
    }
    [ClientRpc]
    private void CloseDoorClientRpc()
    {
        door.Close();
        transform.DOLocalMove(originalLocalPos, buttonDownTime);
    }
    private void ClearTimeToButtonUp()
    {
        timeToButtonUp = 0f;
    }

}
