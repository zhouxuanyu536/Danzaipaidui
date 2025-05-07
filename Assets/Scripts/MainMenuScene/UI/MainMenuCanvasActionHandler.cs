using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuCanvasActionHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject Main;
    private Button UserButton;
    private GameObject Title;
    [SerializeField] private TextMeshProUGUI UserText;
    private Button SinglePlayer;
    private Button MultiPlayer;
    private GameObject Login;
    private GameObject LoginSuccess;
    private GameObject LoginFailed;
    private GameObject Register;
    private GameObject RegisterSuccess;
    private GameObject RegisterFailed;
    private GameObject MultiPlayerSelect;
    void Start()
    {
        
        
        Transform MainTransform = transform.Find("Main");

        Main = MainTransform.gameObject;
        Login = transform.Find("Login").gameObject;
        LoginSuccess = transform.Find("LoginSuccess").gameObject;
        LoginFailed = transform.Find("LoginFailed").gameObject;
        Register = transform.Find("Register").gameObject;
        RegisterSuccess = transform.Find("RegisterSuccess").gameObject;
        RegisterFailed = transform.Find("RegisterFailed").gameObject;
        MultiPlayerSelect = transform.Find("MultiPlayerSelect").gameObject;
        Main.SetActive(true);
        Login.SetActive(false);
        LoginSuccess.SetActive(false);
        LoginFailed.SetActive(false);
        Register.SetActive(false);
        RegisterSuccess.SetActive(false);
        RegisterFailed.SetActive(false);
        MultiPlayerSelect.SetActive(false);
        Title = MainTransform.Find("Title").gameObject;
        UserButton = MainTransform.Find("UserName").GetComponent<Button>();
        SinglePlayer = MainTransform.Find("SinglePlayer").GetComponent<Button>();
        MultiPlayer = MainTransform.Find("MultiPlayer").GetComponent<Button>();
        UserButton.onClick.AddListener(UserLogin);
        SinglePlayer.onClick.AddListener(OnSinglePlayerClicked);
        MultiPlayer.onClick.AddListener(OnMultiPlayerClicked);
    }

    private void UserLogin()
    {
        Login.SetActive(true);
        Main.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnSinglePlayerClicked()
    {
        SceneManager.LoadScene("GameScene");
    }
    private void OnMultiPlayerClicked()
    {
        MultiPlayerSelect.SetActive(true);
        Main.SetActive(false);
    }
    private void OnDestroy()
    {
        UserButton.onClick.RemoveListener(UserLogin);
        SinglePlayer.onClick.RemoveListener(OnSinglePlayerClicked);
    }
}
