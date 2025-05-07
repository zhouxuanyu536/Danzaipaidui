using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalloffGenerator : MonoBehaviour
{
    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size,size];

        for(int i = 0;i < size; i++)
        {
            for(int j = 0;j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }
    static float Evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;
        //衰减地图游戏开始后只生成一次，这么写不会过度放慢游戏运行速度
        return Mathf.Pow(value, a) / (Mathf.Pow(value, b) + Mathf.Pow(b - b * value,a));
    }
}
