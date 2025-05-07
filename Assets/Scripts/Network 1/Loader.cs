using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenuScene, LobbyScene, InLobbyScene, GameScene,LoadingScene
    }

    public static Scene nextScene;
    public static LoadingScreenMode mode;
    public static void Load(Scene targetScene)
    {
        nextScene = targetScene;
        UnityTools.Instance.loadingMode = LoadingScreenMode.Load;
        SceneManager.LoadScene(Scene.LoadingScene.ToString(),LoadSceneMode.Additive);
    }

    public static void LoadNetwork(Scene targetScene)
    {
        nextScene = targetScene;
        UnityTools.Instance.loadingMode = LoadingScreenMode.LoadNetwork;
        SceneManager.LoadScene(Scene.LoadingScene.ToString(), LoadSceneMode.Additive);
    }

    public static void LoadCallback()
    {
        SceneManager.LoadScene(nextScene.ToString());
    }

}
