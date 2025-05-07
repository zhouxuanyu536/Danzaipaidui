using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levelbgm : MonoBehaviour
{
    public static Levelbgm Instance;
    private AudioClip m_AudioClip;
    [SerializeField] private GameSoundConfig m_GameSoundConfig;

    private AudioSource m_AudioSource;
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        m_AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance != null && 
            GameManager.Instance.state.Value == GameManager.State.GamePlaying)
        {
            UnityTools.Instance.PlayAudio(m_AudioSource);
        }
        else
        {
            UnityTools.Instance.PauseAudio(m_AudioSource);
        }
    }
    public void PauseBgm()
    {
        UnityTools.Instance.StopAudio(m_AudioSource);
        m_AudioSource.clip = null;
    }
    public void SetAudioClipFromLevel(int level)
    {
        if(level == 1)
        {
            m_AudioClip = m_GameSoundConfig.Day1AudioClip;
        }
        else if (level == 2)
        {
            m_AudioClip = m_GameSoundConfig.Day2AudioClip;
        }
        else if (level == 3)
        {
            m_AudioClip = m_GameSoundConfig.Night1AudioClip;
        }
        else
        {
            m_AudioClip = m_GameSoundConfig.Night2AudioClip;
        }
        m_AudioSource.clip = m_AudioClip;
    }
    private void OnDestroy()
    {
        m_AudioSource.Stop();
    }
}
