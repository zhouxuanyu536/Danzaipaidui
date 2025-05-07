using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;

public enum PlayMode
{
    SinglePlayer,
    MultiPlayer
}

public static class MainMenuStoredData
{
    public static bool VcameraPrior;
    public static ButtonType buttonType;
}

public class MainMenuCanvas : MonoBehaviour
{
    [SerializeField] private Button SinglePlayer;
    [SerializeField] private Button MultiPlayer;
    [SerializeField] private PlayerVisuals Visuals;
    [SerializeField] private VidPlayer videoPlayer;

    private bool oldVcameraPrior;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        DOTween.timeScale = 1f;
        SinglePlayer.onClick.AddListener(ClickSinglePlayer);
        MultiPlayer.onClick.AddListener(ClickMultiplayer);
        SkyboxSettings.ChangeSkybox(SkyboxType.Day1, isRandom:true);

    }

    // Update is called once per frame
    void Update()
    {
        if(MainMenuStoredData.VcameraPrior != oldVcameraPrior)
        {
            videoPlayerPlay();
            oldVcameraPrior = MainMenuStoredData.VcameraPrior;
        }
    }
    private void videoPlayerPlay()
    {
        if (MainMenuStoredData.VcameraPrior)
        {
            videoPlayer.PlayVideo();
        }
        else
        {
            videoPlayer.PauseVideo();
        }
    }
    private void ClickSinglePlayer()
    {
        Destroy(GameObject.Find("EventSystem"));
        //开启单人模式
        GameMultiplayer.playMultiplayer = false;
        Loader.Load(Loader.Scene.LobbyScene);
    }
    private void ClickMultiplayer()
    {
        Destroy(GameObject.Find("EventSystem"));
        GameMultiplayer.playMultiplayer = true;
        Loader.Load(Loader.Scene.LobbyScene);
    }

   
}
