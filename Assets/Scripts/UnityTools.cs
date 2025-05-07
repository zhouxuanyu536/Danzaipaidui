using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

public class UnityTools
{
    private static UnityTools instance;
    private static PlayerInputController controller;
    public event EventHandler<PlayerEventArgs> AddPlayerEvent;
    private PlayerInput playerInput;
    private Transform localPlayerCamera;

    public static UnityTools Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new UnityTools();
            }
            if(controller == null)
            {
                controller = new PlayerInputController();
            }
            return instance;
        }
    }

    public void SetPlayerInput(PlayerInput playerInput)
    {
        this.playerInput = playerInput;
    }
    public PlayerInput GetPlayerInput()
    {
        return playerInput;
    }
    public PlayerInputController GetPlayerInputController()
    {
        return controller;
    }

    public Transform GetLevelTransform()
    {
        Transform Level = GameObject.FindObjectOfType<LevelDetails>().transform;
        return Level;
    }

    public LevelDetails GetLevelDetails()
    {
        LevelDetails details = GetLevelTransform().GetComponent<LevelDetails>();
        return details;

    }

    public void AddPlayer(PlayerController controller, Transform PlayerTransform )
    {
        AddPlayerEvent?.Invoke(this,new PlayerEventArgs(controller,PlayerTransform));
    }

    private LoadingScreen loadingScreen;
    public PlayMode playMode;
    public void SetLoadingScreen(LoadingScreen loadingScreen)
    {
        this.loadingScreen = loadingScreen;
    }
    public LoadingScreen GetLoadingScreen()
    {
        return loadingScreen;
    }
    public LoadingScreenMode loadingMode;

    public void PlayOneShot(AudioSource audioSource,AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, GlobalSettings.volume);
    }

    public void PlayAudio(AudioSource audioSource)
    {
        audioSource.volume = GlobalSettings.GetAbsoluteMusicVolume();
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        
    }
    public void PauseAudio(AudioSource audioSource)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void StopAudio(AudioSource audioSource)
    {
        audioSource.Stop();
    }

    public void PlayVideo(VideoPlayer videoPlayer,AudioSource audioSource,bool useAudioSource = false)
    {
        if(useAudioSource)
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.SetTargetAudioSource(0, audioSource);

            audioSource.volume = GlobalSettings.GetAbsoluteMusicVolume();
            videoPlayer.Play();
        }
    }
    public void PauseVideo(VideoPlayer videoPlayer)
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
    }
    public void StopVideo(VideoPlayer videoPlayer)
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
    }
    public void CleanInstance()
    {
        instance = new UnityTools();
        controller = new PlayerInputController();
    }
    public bool CheckIfLobbyNameIllegal(string lobbyName,out string illegalMessage)
    {
        if (lobbyName.Trim() == String.Empty)
        {
            illegalMessage = "房间名不能为空";
            return true;
        }
        else
        {
            illegalMessage = "";
            return false;
        }
    }

    public bool CheckIfPlayerNameIllegal(string playerName,out string illegalMessage)
    {
        if (playerName.Trim() == String.Empty)
        {
            illegalMessage = "玩家名不能为空";
            return true;
        }
        else
        {
            illegalMessage = "";
            return false;
        }
    }
    public void SetLocalPlayerCamera(Transform camera)
    {
        localPlayerCamera = camera;
    }

    public Transform GetLocalPlayerCamera()
    {
        return localPlayerCamera;
    }
}
