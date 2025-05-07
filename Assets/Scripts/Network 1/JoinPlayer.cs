using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoinPlayer : MonoBehaviour
{
    [SerializeField] private Button Kick;
    [SerializeField] private Image KickSprite;
    [SerializeField] private Sprite CanKick;
    [SerializeField] private Sprite CanNotKick;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI isReady;
    [SerializeField] private Image JoinedPlayerBg;
    
    private PlayerData playerData;
    // Start is called before the first frame update
    void Start()
    {
        if (GameLobby.Instance.IsLobbyHost() && !GameLobby.Instance.IsServerOfLobby(playerData))  
        {
            KickSprite.sprite = CanKick;
            Kick.onClick.AddListener(() =>
            {
                GameLobby.Instance.KickPlayer(playerData.playerId.ToString());
                GameMultiplayer.Instance.KickPlayerServerRpc(playerData.clientId);
            });
        }
        else
        {
            KickSprite.sprite = CanNotKick;
            Kick.interactable = false;
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayer(PlayerData playerData)
    {
        this.playerData = playerData;
        playerName.text = playerData.playerName.ToString();
        JoinedPlayerBg.color = playerData.playerColor;
        JoinedPlayerBg.color = new Color(
            playerData.playerColor.r,
            playerData.playerColor.g,
            playerData.playerColor.b,
            0.2f);
        if (playerData.isReady)
        {
            isReady.text = "准备好了";
        }
        else
        {
            isReady.text = null;
        }
    }
}
