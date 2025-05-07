using Unity.VisualScripting;
using UnityEngine;

public class Audioplay : MonoBehaviour
{
    public static Audioplay Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip victory;
    [SerializeField] private AudioClip defeat;

    public bool CanDestroy;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if(audioSource == null)
        {
            if(audioSource.GetComponent<AudioSource>() != null)
            {
                audioSource = transform.GetComponent<AudioSource>();
            }
            else
            {
                audioSource = transform.AddComponent<AudioSource>();
            }
        }
        CanDestroy = false;
    }
    public void PlayVictory()
    {
        UnityTools.Instance.PlayOneShot(audioSource, victory);
    }

    public void PlayDefeat()
    {
        UnityTools.Instance.PlayOneShot(audioSource, defeat);
    }
    
}
