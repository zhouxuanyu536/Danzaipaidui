using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UserRegisterFailed : MonoBehaviour
{
    [SerializeField] private GameObject Login;
    [SerializeField] private Button Confirm;
    private void Start()
    {
        Confirm.onClick.AddListener(ConfirmLoginFailed);
    }

    private void ConfirmLoginFailed()
    {
        Login.SetActive(true);
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        Confirm.onClick.RemoveAllListeners();
    }
}
