using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserRegister : MonoBehaviour
{
    [SerializeField] private TMP_InputField UserNameInputField;
    [SerializeField] private TMP_InputField PasswordInputField;
    [SerializeField] private GameObject Login;
    [SerializeField] private Button Exit;
    [SerializeField] private Button Register;
    [SerializeField] private GameObject RegisterSuccess;
    [SerializeField] private GameObject RegisterFailed;
    private void Start()
    {
        Exit.onClick.AddListener(OnRegisterExit);
        Register.onClick.AddListener(OnRegister);
        RegisterSuccess.SetActive(false);
        RegisterFailed.SetActive(false);
    }
    private void OnDestroy()
    {
        Exit.onClick.RemoveAllListeners();
        Register.onClick.RemoveAllListeners();
    }
    private void OnRegisterExit()
    {
        gameObject.SetActive(false);
        Login.gameObject.SetActive(true);
    }
    private void OnRegisterSuccess()
    {
        UserNameInputField.text = "";
        PasswordInputField.text = "Password";
        gameObject.SetActive(false);
        RegisterSuccess.SetActive(true);
    }
    private void OnRegisterFailed()
    {
        gameObject.SetActive(false);
        RegisterFailed.SetActive(true);
    }
    private void OnRegister()
    {
        gameObject.SetActive(false);
        //如果输入的信息合法，则弹出RegisterSuccess
        //假设
        var user = UserNameInputField.text;
        var password = PasswordInputField.text;
        bool NotAllowed1 = user.Trim() == "zxy921";
        bool NotAllowed2 = user.Trim().Length == 0 || password.Trim().Length == 0;
        if (NotAllowed1 || NotAllowed2)
        {
            //注册失败
            OnRegisterFailed();
        }
        else
        {
            //注册成功
            OnRegisterSuccess();
        }
    }
}
