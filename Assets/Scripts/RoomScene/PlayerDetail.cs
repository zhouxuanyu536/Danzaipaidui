using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDetail : NetworkBehaviour
{
    [SerializeField] private Button Kickout;
    [SerializeField] private TextMeshProUGUI PlayerName;
    [SerializeField] private Image isReady;
    [SerializeField] private Sprite isReadyImage;
    [SerializeField] private Sprite isNotReadySprite;
    
    private ulong clientId;
    private string Name;
    private bool ready;
    private int num;

    private void Start()
    {
        Kickout.onClick.AddListener(KickoutPlayer);
    }
    private void Update()
    {
        PlayerName.text = Name;
        if (ready)
        {
            isReady.sprite = isReadyImage;
        }
        else
        {
            isReady.sprite = isNotReadySprite;
        }
    }
    public void SetClientId(ulong clientId)
    {
        this.clientId = clientId;
    }
    public void SetPlayerName(string name)
    {
        this.Name = name;
    }
    public void SetPlayerReady(bool ready)
    {
        this.ready = ready;
    }
    public void SetNum(int num)
    {
        this.num = num;
    }

    public void KickoutPlayer()
    {
        //    if(NetworkManager.Singleton.IsServer)
        //    {
        //        ulong id = GameMultiplayer.Instance.playerDataNetworkList[num].clientId;
        //        GameMultiplayer.Instance.OnPlayerQuit(id);
        //    }
    }

    public override void OnDestroy()
    {
        Kickout.onClick.RemoveListener(KickoutPlayer);
    }
}
