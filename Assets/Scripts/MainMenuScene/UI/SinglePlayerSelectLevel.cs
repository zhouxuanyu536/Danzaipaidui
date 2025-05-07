using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SinglePlayerSelectLevel : MonoBehaviour
{
    private TMP_Dropdown selectLevelDropDown;
    // Start is called before the first frame update
    void Start()
    {
        selectLevelDropDown = GetComponent<TMP_Dropdown>();
        if(GameMultiplayer.playSinglePlayerLevel > 1)
        {
            selectLevelDropDown.value = GameMultiplayer.playSinglePlayerLevel - 1;
            selectLevelDropDown.RefreshShownValue();
        }
        else
        {
            GameMultiplayer.playSinglePlayerLevel = 1;
        }
        selectLevelDropDown.onValueChanged.AddListener(selectLevelDropDown_OnValueChanged);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void selectLevelDropDown_OnValueChanged(int index)
    {
        GameMultiplayer.playSinglePlayerLevel = index + 1;
    }
}
