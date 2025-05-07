using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
public enum SettingType
{
    GlobalVolume,
    MusicVolume,
    Bright
}
public class PlayerGlobalSetting : MonoBehaviour
{
    [SerializeField] private SettingType settingType;
    [SerializeField] private GameObject settingIcon;
    [SerializeField] private Slider settingSlider;
    [SerializeField] private TextMeshProUGUI settingNum;
    [SerializeField] private GameObject settingMessage;
    private string PlayerPrefsKey => $"Setting_{settingType}";
    // Start is called before the first frame update
    void Awake()
    {
        if(settingSlider != null && settingNum != null)
        {
            //默认值 1f
            float savedValue = PlayerPrefs.GetFloat(PlayerPrefsKey, 1f);
            settingSlider.value = savedValue;
            settingSlider.onValueChanged.AddListener(UpdateSettingNum);
            UpdateSettingNum(settingSlider.value); // 初始化 UI 显示
            settingNum.gameObject.AddComponent<EventTrigger>().triggers.Add(CreatePointerClickEvent(ShowMessage));
            ChangeProperty();
            settingSlider.onValueChanged.AddListener(value => {
                ShowMessage();
                ChangeProperty();
            });
            settingSlider.gameObject.AddComponent<EventTrigger>().triggers.Add(CreatePointerClickEvent(ShowMessage));
        }
        //当点击了SettingIcon,SettingSlider或settingNum后，出现提示
        // 监听点击事件
        if (settingIcon != null)
        {
            settingIcon.AddComponent<EventTrigger>().triggers.Add(CreatePointerClickEvent(ShowMessage));
        }
        HideMessage();
    }

    private void ChangeProperty()
    {
        if(settingType == SettingType.GlobalVolume)
        {
            GlobalSettings.volume = Mathf.Round(settingSlider.value * 100f) / 100f;
        }
        else if(settingType == SettingType.MusicVolume)
        {
            GlobalSettings.relativeMusicVolume = Mathf.Round(settingSlider.value * 100f) / 100f;
        }
        else if(settingType == SettingType.Bright)
        {
            GlobalSettings.bright = Mathf.Round(settingSlider.value * 100f) / 100f;
        }
    }

    private void UpdateSettingNum(float value)
    {
        settingNum.text = Mathf.RoundToInt(value * 100).ToString(); // 保留两位小数
        PlayerPrefs.SetFloat(PlayerPrefsKey, value); // 存储到 PlayerPrefs
        PlayerPrefs.Save(); // 立即保存
    }
    private void OnDestroy()
    {
        if (settingSlider != null)
        {
            settingSlider.onValueChanged.RemoveListener(UpdateSettingNum);
            settingSlider.onValueChanged.RemoveListener(value => ShowMessage());
        }
    }

    void Update()
    {
        if(settingMessage != null)
        {
            Vector3 pos = settingMessage.transform.position;
            settingMessage.transform.position = new Vector3(pos.x,transform.position.y,pos.z);
        }
    }

    private Entry CreatePointerClickEvent(UnityAction action)
    {
        Entry entry = new Entry { eventID = EventTriggerType.PointerClick };
        entry.callback.AddListener((eventData) => action());
        return entry;
    }
    private void ShowMessage()
    {
        if (settingMessage != null)
        {
            settingMessage.SetActive(true);
            CancelInvoke(nameof(HideMessage)); // 先取消之前的隐藏，防止闪烁
            Invoke(nameof(HideMessage), 2f); // 2秒后自动隐藏
        }
    }
    private void HideMessage()
    {
        if (settingMessage != null)
        {
            settingMessage.SetActive(false);
        }
    }
}
