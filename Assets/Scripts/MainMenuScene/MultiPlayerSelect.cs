using UnityEngine;
using UnityEngine.UI;

public class MultiPlayerSelect : MonoBehaviour
{
    [SerializeField] private Button StartHost;
    [SerializeField] private Button StartClient;
    [SerializeField] private Button Return;
    [SerializeField] private GameObject Main;

    private void Start()
    {
        StartHost.onClick.AddListener(OnStartHost);
        StartClient.onClick.AddListener(OnStartClient);
        Return.onClick.AddListener(OnReturn);
    }
    private void OnStartHost()
    {
        //GameMultiplayer.Instance.StartHost();
    }
    private void OnStartClient()
    {
        //GameMultiplayer.Instance.StartClient();
    }
    private void OnReturn()
    {
        gameObject.SetActive(false);
        Main.SetActive(true);
    }
}
