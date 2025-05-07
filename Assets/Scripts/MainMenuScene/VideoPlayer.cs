using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class VidPlayer: MonoBehaviour
{
    public string videoName; // 只需要文件名，如 "video.mp4"
    private VideoPlayer videoPlayer;
    [SerializeField] private AudioSource audioSource;

    private bool isPlaying;
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        isPlaying = false;
    }

    public void PlayVideo()
    {
        if(!isPlaying)
        {
            StartCoroutine(LoadAndPlayVideo());
            isPlaying = true;
        }
        else
        {
            UnityTools.Instance.PlayVideo(videoPlayer, audioSource, true);
        }
        
    }

    private IEnumerator LoadAndPlayVideo()
    {
        string videoPath = Path.Combine(Application.streamingAssetsPath, videoName);

        // Android 特殊处理 忽略
        if (Application.platform == RuntimePlatform.Android)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(videoPath))
            {
                yield return request;
                if (request.result == UnityWebRequest.Result.Success)
                {
                    string tempPath = Path.Combine(Application.persistentDataPath, videoName);
                    File.WriteAllBytes(tempPath, request.downloadHandler.data);
                    videoPath = tempPath;
                }
                else
                {
                    Debug.LogError("Failed to load video: " + request.error);
                    yield break;
                }
            }
        }

        videoPlayer.url = videoPath;
        UnityTools.Instance.PlayVideo(videoPlayer, audioSource,true);
    }

    public void PauseVideo()
    { 
        UnityTools.Instance.PauseVideo(videoPlayer);
    }

    public void StopVideo()
    {
        UnityTools.Instance.StopVideo(videoPlayer);
    }
}
