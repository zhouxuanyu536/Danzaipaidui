using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class GlobalSettings
{
    public static float volume = PlayerPrefs.GetFloat("Setting_" + SettingType.GlobalVolume.ToString(),1f);
    public static float relativeMusicVolume = PlayerPrefs.GetFloat("Setting_" + SettingType.MusicVolume.ToString(), 1f);
    public static float bright = PlayerPrefs.GetFloat("Setting_" + SettingType.Bright.ToString(), 1f);

    public static float GetAbsoluteMusicVolume()
    {
        return volume * relativeMusicVolume;
    }
}
