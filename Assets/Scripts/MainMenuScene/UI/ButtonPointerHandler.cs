using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ButtonType
{
    SinglePlayer,
    MultiplePlayer
}

public class ButtonPointerHandler : MonoBehaviour
{
    [SerializeField] private Button button;
    private AudioSource audioSource;
    [SerializeField] private AudioClip pointerEnterAudioClip;
    [SerializeField] private AudioClip pointerDownAudioClip;
    [SerializeField] private AudioClip pointerUpAudioClip;
    [SerializeField] private AudioClip pointerExitAudioClip;
    [SerializeField] private ButtonType buttonType;
    [SerializeField] private bool pointerEnter;
    [SerializeField] private bool pointerDown;
    [SerializeField] private bool pointerUp;
    [SerializeField] private bool pointerExit;
    [SerializeField] private ButtonSoundConfig buttonSoundConfig;

    public bool isPointerOver;
    private GraphicRaycaster graphicRaycaster;
    private EventSystem eventSystem;

    private float pointerEnterTime;
    private float pointerExitTime;
    private float pointerOverTimeThreshold = 0.3f;
    void Start()
    {
        button = GetComponent<Button>();
        audioSource = gameObject.AddComponent<AudioSource>();

        // 获取 GraphicRaycaster 和 EventSystem
        graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
        eventSystem = EventSystem.current;

        pointerEnterTime = 0f;
        pointerExitTime = 0f;

        if(pointerEnterAudioClip == null)
        {
            pointerEnterAudioClip = buttonSoundConfig.pointerEnterAudioClip;
        }
        if (pointerExitAudioClip == null)
        {
            pointerExitAudioClip = buttonSoundConfig.pointerExitAudioClip;
        }
        if (pointerUpAudioClip == null)
        {
            pointerUpAudioClip = buttonSoundConfig.pointerUpAudioClip;
        }
        if (pointerDownAudioClip == null)
        {
            pointerDownAudioClip = buttonSoundConfig.pointerDownAudioClip;
        }
    }

    void Update()
    {
        if (IsPointerOverUIObject(out GameObject uiObject))
        {
            if(uiObject.GetComponent<TextMeshProUGUI>() != null)
            {
                uiObject = uiObject.transform.parent.gameObject;
            }
            if (uiObject == gameObject)
            {
                if (!isPointerOver)
                {
                    pointerExitTime = 0f;
                    pointerEnterTime += Time.deltaTime;
                    if(pointerEnterTime > pointerOverTimeThreshold)
                    {
                        pointerEnterTime = 0f;
                        OnPointerEnter();
                    }
                }

                if (Input.GetMouseButtonDown(0))
                {
                    OnPointerDown();
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    OnPointerUp();
                }
            }
            else if (isPointerOver)
            {
                pointerEnterTime = 0f;
                pointerExitTime += Time.deltaTime;
                if (pointerExitTime > pointerOverTimeThreshold)
                {
                    pointerExitTime = 0f;
                    OnPointerExit();
                }
                
            }
        }
        else if (isPointerOver)
        {
            pointerEnterTime = 0f;
            pointerExitTime += Time.deltaTime;
            if (pointerExitTime > pointerOverTimeThreshold)
            {
                pointerExitTime = 0f;
                OnPointerExit();
            }
        }
    }

    private bool IsPointerOverUIObject(out GameObject uiObject)
    {
        PointerEventData eventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, results);

        if (results.Count > 0)
        {
            uiObject = results[0].gameObject; // 获取最上层的 UI 组件
            return true;
        }

        uiObject = null;
        return false;
    }

    
    private void OnPointerEnter()
    {

        MainMenuStoredData.VcameraPrior = true;
        MainMenuStoredData.buttonType = buttonType;
        isPointerOver = true;
        if (!pointerEnter) return;
        UnityTools.Instance.PlayOneShot(audioSource, pointerEnterAudioClip);

    }

    private void OnPointerExit()
    {
        MainMenuStoredData.VcameraPrior = false;
        isPointerOver = false;
        if (!pointerExit) return;
        UnityTools.Instance.PlayOneShot(audioSource, pointerExitAudioClip);
    }

    private void OnPointerDown()
    {
        MainMenuStoredData.VcameraPrior = true;
        MainMenuStoredData.buttonType = buttonType;
        isPointerOver = true;
        if (!pointerDown) return;
        UnityTools.Instance.PlayOneShot(audioSource, pointerDownAudioClip);


    }

    private void OnPointerUp()
    {
        MainMenuStoredData.VcameraPrior = false;
        if (!pointerUp) return;
        UnityTools.Instance.PlayOneShot(audioSource, pointerUpAudioClip);

    }


}

