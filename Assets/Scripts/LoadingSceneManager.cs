using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private Slider progressBar; // 拖入Slider

    private void Start()
    {
        //StartCoroutine(LoadSceneAsync("GameScene"));
    }

    //private IEnumerator LoadSceneAsync(string sceneName)
    //{
    //    AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
    //    yield return null;
    //}
}
