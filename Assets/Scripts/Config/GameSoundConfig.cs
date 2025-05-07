using UnityEngine;

[CreateAssetMenu(fileName = "GameSoundConfig", menuName = "Audio/Game Sound Config")]
public class GameSoundConfig : ScriptableObject
{
    public AudioClip Day1AudioClip;
    public AudioClip Day2AudioClip;
    public AudioClip Night1AudioClip;
    public AudioClip Night2AudioClip;

    public AudioClip CountDownClip;
}

