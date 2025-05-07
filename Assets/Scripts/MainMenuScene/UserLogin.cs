using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserLogin : MonoBehaviour
{
    [SerializeField] private GameObject Main;
    [SerializeField] private TMP_InputField UserNameInputField;
    [SerializeField] private TMP_InputField PasswordInputField;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject LoginSuccess;
    [SerializeField] private GameObject LoginFailed;
    [SerializeField] private GameObject RegisterGameObject;
    [SerializeField] private Button Exit;
    [SerializeField] private Button Login;
    [SerializeField] private Button Register;
    // Start is called before the first frame update
    void Start()
    {
        Exit.onClick.AddListener(OnLoginExit);
        Login.onClick.AddListener(OnLogin);
        Register.onClick.AddListener(OnRegister);
        LoginSuccess.SetActive(false);
        LoginFailed.SetActive(false);
    }
    void ClearInputField()
    {
        UserNameInputField.text = "";
        PasswordInputField.text = "";
    }
    void OnLoginExit()
    {
        ClearInputField();
        Main.SetActive(true);
        gameObject.SetActive(false);
    }
    void OnLoginSuccess()
    {
        //清空InputField
        ClearInputField();
        LoginSuccess.SetActive(true);
        gameObject.SetActive(false);
    }
    void OnLoginFailed()
    {
        LoginFailed.SetActive(true);
        gameObject.SetActive(false);
    }
    void OnLogin()
    {
        bool LoginSuccess = false;
        //与数据库比对

        //开挂
        if(UserNameInputField.text == "zxy921" 
            && PasswordInputField.text == "12345678zxy")
        {
            LoginSuccess = true;
        }

        if (LoginSuccess)
        {
            Debug.Log("登录成功！");
            text.text = $"用户名：{UserNameInputField.text}";
            OnLoginSuccess();
        }
        else
        {
            Debug.Log("用户名或密码错误！");
            OnLoginFailed();
        }

        
    }
    void OnRegister()
    {
        RegisterGameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDestroy()
    {
        Exit.onClick.RemoveListener(OnLoginExit);
        Login.onClick.RemoveListener(OnLogin);
    }
}
