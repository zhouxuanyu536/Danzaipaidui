using UnityEngine;

[CreateAssetMenu(fileName = "ButtonSoundConfig", menuName = "Audio/Button Sound Config")]
public class ButtonSoundConfig : ScriptableObject
{
    public AudioClip pointerEnterAudioClip;
    public AudioClip pointerDownAudioClip;
    public AudioClip pointerUpAudioClip;
    public AudioClip pointerExitAudioClip;
}

