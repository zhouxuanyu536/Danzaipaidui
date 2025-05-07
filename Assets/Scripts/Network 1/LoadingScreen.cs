using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum LoadingScreenMode
{
    Load,
    LoadNetwork

}
public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;


    // Start is called before the first frame update
    void Start()
    {
        //注册
        UnityTools.Instance.SetLoadingScreen(this);
        StartCoroutine(LoadAsync());
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoadAsync()
    {
        AsyncOperation operation;
        if (UnityTools.Instance.loadingMode == LoadingScreenMode.Load)
        {
            operation = SceneManager.LoadSceneAsync(Loader.nextScene.ToString());
            operation.allowSceneActivation = false;
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                progressBar.value = progress;
                progressText.text = $"加载中...({progress * 100:F0}%)";

                if (operation.progress >= 0.9f) //加载完成：0.9f
                {
                    yield return new WaitForSeconds(0.8f);

                    operation.allowSceneActivation = true;
                }
                yield return null;
            }
        }
        else
        {
            progressBar.value = 100;
            progressText.text = "完成...(100%)";
            //由于没有AsyncOperation,还是直接100%吧...
            NetworkManager.Singleton.SceneManager.LoadScene(Loader.nextScene.ToString(), LoadSceneMode.Single);
        }
        
    }

}
