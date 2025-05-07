using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Button quitButton;
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            settingsPanel.SetActive(true);
            quitButton.gameObject.SetActive(true);
        });
        quitButton.onClick.AddListener(() =>
        {
            settingsPanel.SetActive(false);
            quitButton.gameObject.SetActive(false);
        });
        settingsPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
