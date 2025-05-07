using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UserLoginSuccess : MonoBehaviour
{
    [SerializeField] private GameObject Main;
    [SerializeField] private Button Confirm;
    private void Start()
    {
        Confirm.onClick.AddListener(ConfirmLoginSuccess);
    }
    
    private void ConfirmLoginSuccess()
    {
        Main.SetActive(true);
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        Confirm.onClick.RemoveAllListeners();
    }
}
