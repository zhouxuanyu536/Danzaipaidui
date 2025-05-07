using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputSettingsButtonHandler : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private GameObject VolumeSetting;
    [SerializeField] private GameObject MusicSetting;
    [SerializeField] private GameObject BrightSetting;
    [SerializeField] private GameObject CharacterForward;
    [SerializeField] private GameObject CharacterBackward;
    [SerializeField] private GameObject CharacterLeft;
    [SerializeField] private GameObject CharacterRight;
    [SerializeField] private GameObject CharacterJump;
    [SerializeField] private GameObject CharacterSprint;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private GameObject SettingsPanelMessage;
    private Button inputButton;
    private bool status;
    // Start is called before the first frame update
    void Start()
    {
        inputButton = GetComponent<Button>();
        ChangeStatus();
        inputButton.onClick.AddListener(ClickChangeStatus);
    }
    private void OnEnable()
    {
        status = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void ClickChangeStatus()
    {
        status = !status;
        ChangeStatus();
    }
    private void ChangeStatus()
    {
        if(status)
        {
            titleText.text = "按键设置";
            VolumeSetting.SetActive(false);
            MusicSetting.SetActive(false);
            BrightSetting.SetActive(false);
            CharacterForward.SetActive(true);
            CharacterBackward.SetActive(true);
            CharacterLeft.SetActive(true);
            CharacterRight.SetActive(true);
            CharacterJump.SetActive(true);
            CharacterSprint.SetActive(true);
            SettingsPanelMessage.SetActive(false);
            buttonText.text = "返回";
        }
        else
        {
            titleText.text = "设置";
            VolumeSetting.SetActive(true);
            MusicSetting.SetActive(true);
            BrightSetting.SetActive(true);
            CharacterForward.SetActive(false);
            CharacterBackward.SetActive(false);
            CharacterLeft.SetActive(false);
            CharacterRight.SetActive(false);
            CharacterJump.SetActive(false);
            CharacterSprint.SetActive(false);
            SettingsPanelMessage.SetActive(true);
            buttonText.text = "按键设置";
        }
    }
}
