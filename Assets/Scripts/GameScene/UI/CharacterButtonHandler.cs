using DG.Tweening;
using Newtonsoft.Json.Linq;
using QFSW.QC.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public enum CharacterButtonHandlerType
{
    Forward,
    Backward,
    Left,
    Right, 
    Jump,
    Sprint
}
public class CharacterButtonHandler : MonoBehaviour
{
    [SerializeField] private CharacterButtonHandlerType handlerType;
    [SerializeField] private Image image;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI buttonText;
    public Key usedKey;
    private Tween tween;

    private readonly string playerPrefKeyPrefix = "Key_";
    private string playerPrefKeySuffix;
    private string playerPrefKey;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _SprintAction;

    private PlayerInputController playerInputController;
    // Start is called before the first frame update
    void Awake()
    {
        button.onClick.AddListener(DealWithInput);
        playerPrefKeySuffix = "";
        playerInputController = UnityTools.Instance.GetPlayerInputController();
        InitializeUsedKey();
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if(usedKey == Key.None)
        {
            buttonText.text = "-";
        }
    }
    void InitializeUsedKey()
    {
        switch (handlerType)
        {
            case CharacterButtonHandlerType.Forward:
                usedKey = Key.W;
                playerPrefKeySuffix += "Forward";
                break;
            case CharacterButtonHandlerType.Backward:
                usedKey = Key.S;
                playerPrefKeySuffix += "Backward";
                break;
            case CharacterButtonHandlerType.Left:
                usedKey = Key.A;
                playerPrefKeySuffix += "Left";
                break;
            case CharacterButtonHandlerType.Right:
                usedKey = Key.D;
                playerPrefKeySuffix += "Right";
                break;
            case CharacterButtonHandlerType.Jump:
                usedKey = Key.Space;
                playerPrefKeySuffix += "Jump";
                break;
            case CharacterButtonHandlerType.Sprint:
                usedKey = Key.E;
                playerPrefKeySuffix += "Sprint";
                break;
        }
        playerPrefKey = playerPrefKeyPrefix + playerPrefKeySuffix;
        Array array = Enum.GetValues(typeof(Key));
        if (PlayerPrefs.HasKey(playerPrefKey))
        {
            usedKey = (Key)array.GetValue(PlayerPrefs.GetInt(playerPrefKey));
            HandleModifyKey(usedKey);
        }
        else
        {
            for (int i = 0; i < array.Length; i++)
            {
                if ((Key)array.GetValue(i) == usedKey)
                {
                    PlayerPrefs.SetInt(playerPrefKey, i);
                    break;
                }
            }
        }
        buttonText.text = usedKey.ToString().ToUpper();
    }
    private void HandleModifyKey(Key key)
    {
        playerInputController.Player.Move.Disable();
        playerInputController.Player.Jump.Disable();
        playerInputController.Player.Sprint.Disable();
        InputBinding inputBinding = new InputBinding();

        

        switch (handlerType)
        {
            case CharacterButtonHandlerType.Forward:
                inputBinding = playerInputController.Player.Move.bindings[1];
                break;
            case CharacterButtonHandlerType.Backward:
                inputBinding = playerInputController.Player.Move.bindings[2];
                break;
            case CharacterButtonHandlerType.Left:
                inputBinding = playerInputController.Player.Move.bindings[3];
                break;
            case CharacterButtonHandlerType.Right:
                inputBinding = playerInputController.Player.Move.bindings[4];
                break;
            case CharacterButtonHandlerType.Jump:
                inputBinding = playerInputController.Player.Jump.bindings[0];
                break;
            case CharacterButtonHandlerType.Sprint:
                inputBinding = playerInputController.Player.Sprint.bindings[0];
                break;
        }
        
        string keyName = Enum.GetName(typeof(Key), key);
        if (keyName.Contains("Alpha"))
            keyName = keyName.Substring(5);
        else if (keyName.Contains("Keypad"))
            keyName = keyName.Substring(6);
        else if (keyName.Length > 1)
            keyName = keyName.Substring(0, 1).ToLower() + keyName.Substring(1);
        else 
            keyName = keyName.ToLower();

        inputBinding.overridePath = $"<keyboard>/{keyName}";

        switch (handlerType)
        {
            case CharacterButtonHandlerType.Forward:
                playerInputController.Player.Move.ApplyBindingOverride(1, inputBinding);
                break;
            case CharacterButtonHandlerType.Backward:
                playerInputController.Player.Move.ApplyBindingOverride(2, inputBinding);
                break;
            case CharacterButtonHandlerType.Left:
                playerInputController.Player.Move.ApplyBindingOverride(3, inputBinding);
                break;
            case CharacterButtonHandlerType.Right:
                playerInputController.Player.Move.ApplyBindingOverride(4, inputBinding);
                break;
            case CharacterButtonHandlerType.Jump:
                playerInputController.Player.Jump.ApplyBindingOverride(0, inputBinding);
                break;
            case CharacterButtonHandlerType.Sprint:
                playerInputController.Player.Sprint.ApplyBindingOverride(0, inputBinding);
                break;
        }
#if UNITY_EDITOR
        Debug.Log("成功绑定按键：" + inputBinding + ":from" + inputBinding.path + " to" + inputBinding.overridePath);
#endif
        playerInputController.Player.Move.Enable();
        playerInputController.Player.Jump.Enable();
        playerInputController.Player.Sprint.Enable();
    }
    private void DealWithInput()
    {
        //(newInputSystem)
        //button 闪烁 
        StartButtonFlash();
        //检测输入的按键，若没有输入，则button停止闪烁
        StartCoroutine(WaitForNewInput());
        //处理输入 usedKey
    }
    private void StartButtonFlash()
    {
        foreach(Transform tr in transform.parent)
        {
            CharacterButtonHandler handler;
            if((handler = tr.GetComponent<CharacterButtonHandler>()) != null)
            {
                handler.StopButtonFlash();
                
            }
        }
        tween = image.DOFade(0.6f, 0.5f).SetLoops(-1,LoopType.Yoyo);
    }
    private void StopButtonFlash()
    {
        tween.Kill();
        StopAllCoroutines();
        image.color = new Color(image.color.r,image.color.g,image.color.b,1);
    }
    private IEnumerator WaitForNewInput()
    {
        bool keyReceived = false;
        float startTime = 0f;
        while (!keyReceived)
        {
            foreach (var keyControl in Keyboard.current.allKeys)
            {
                if (keyControl.wasPressedThisFrame)
                {
                    usedKey = keyControl.keyCode;
                    buttonText.text = usedKey.ToString().ToUpper();
                    Debug.Log("buttonText:" + buttonText.text);
                    HandleModifyKey(usedKey);
                    keyReceived = true;
                    break;
                }
            }
            startTime += Time.deltaTime;
            if(startTime > 10f)
            {
                break;
            }
            yield return null;
        }
        foreach (Transform tr in transform.parent)
        {
            CharacterButtonHandler handler;
            if ((handler = tr.GetComponent<CharacterButtonHandler>()) != null
                && handler != this)
            {
                CheckIfButtonConflict(handler);
            }
        }

        Array array = Enum.GetValues(typeof(Key));
        for (int i = 0; i < array.Length; i++)
        {
            if ((Key)array.GetValue(i) == usedKey)
            {
                PlayerPrefs.SetInt(playerPrefKey, i);
                break;
            }
        }

        
        StopButtonFlash();
    }
    private void CheckIfButtonConflict(CharacterButtonHandler handler)
    {
        if(handler.usedKey == usedKey)
        {
            handler.usedKey = Key.None;
            Array array = Enum.GetValues(typeof(Key));
            for (int i = 0; i < array.Length; i++)
            {
                if ((Key)array.GetValue(i) == Key.None)
                {
                    PlayerPrefs.SetInt(handler.playerPrefKey, i);
                    handler.HandleModifyKey(Key.None);
                    break;
                }
            }
        }
    }
    private void OnDisable()
    {
        //button 不再闪烁 
        StopButtonFlash();
    }
}
