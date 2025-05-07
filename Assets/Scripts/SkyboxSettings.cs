using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;
public enum SkyboxType
{
    Day1,
    Day2,
    Night1,
    Night2
}
public static class SkyboxSettings
{
    private static string SkyBoxRootPath = "Skybox/";
    static Material skyboxMaterial;
    public static void ChangeSkybox(SkyboxType skyType,bool isRandom = false)
    {
        if (isRandom)
        {
            SkyboxType[] skyboxTypes = (SkyboxType[])Enum.GetValues(typeof(SkyboxType));
            skyType = skyboxTypes[Random.Range(0, skyboxTypes.Length)];
        }
        switch (skyType)
        {
            case SkyboxType.Day1:
                skyboxMaterial = Resources.Load<Material>(SkyBoxRootPath + "FS000_Day_04");
                break;
            case SkyboxType.Day2:
                skyboxMaterial = Resources.Load<Material>(SkyBoxRootPath + "FS000_Day_05_Sunless");
                break;
            case SkyboxType.Night1:
                skyboxMaterial = Resources.Load<Material>(SkyBoxRootPath + "FS002_Night");
                break;
            case SkyboxType.Night2:
                skyboxMaterial = Resources.Load<Material>(SkyBoxRootPath + "FS017_Night");
                break;
        }
        RenderSettings.skybox = skyboxMaterial;
        DynamicGI.UpdateEnvironment(); // 更新全局光照
    }
}
