using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    public bool IsInputOnThisPlayer;
    [SerializeField] private CinemachineFreeLook Camera; 
    [SerializeField] private PlayerCapsuleInput playerCapsuleInput;
    [SerializeField] private Renderer PlayerRenderer;
    public static Player LocalInstance { get; private set; }

    
    // Start is called before the first frame update
    void Start()
    {

    }

    public override void OnNetworkSpawn()
    {
        int index = GameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId);
        transform.position = new Vector3(index * 1.5f,0.5f,0);
        Camera.gameObject.SetActive(false);
        if (NetworkManager.Singleton.LocalClientId == GetComponent<NetworkObject>().OwnerClientId)
        {
            LocalInstance = this;
            IsInputOnThisPlayer = true;
            Camera.gameObject.SetActive(true);
        }
        base.OnNetworkSpawn();
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
    {

    }
    // Update is called once per frame
    void Update()
    {
    }
}
