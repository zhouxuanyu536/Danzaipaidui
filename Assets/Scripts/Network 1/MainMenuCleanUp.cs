using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    private void Awake()
    {
        if(NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }
        if(GameMultiplayer.Instance != null)
        {
            Destroy(GameMultiplayer.Instance.gameObject);
        }
        if(GameLobby.Instance != null)
        {
            Destroy(GameLobby.Instance.gameObject);
        }
        if(Audioplay.Instance != null && Audioplay.Instance.CanDestroy)
        {
            Destroy(Audioplay.Instance.gameObject);
        }
    }
}
