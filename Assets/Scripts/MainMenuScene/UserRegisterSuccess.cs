using UnityEngine;
using UnityEngine.UI;

public class UserRegisterSuccess : MonoBehaviour
{
    [SerializeField] private GameObject Login;
    [SerializeField] private Button Confirm;

    private void Start()
    {
        Confirm.onClick.AddListener(ConfirmLoginSuccess);
    }

    private void ConfirmLoginSuccess()
    {
        Login.SetActive(true);
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        Confirm.onClick.RemoveAllListeners();
    }
}
