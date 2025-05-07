using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;
public enum VisualMode
{
    Single,
    Multiple
}
public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private Transform SinglePlayer;
    [SerializeField] private Transform[] MultiplePlayers;

    [SerializeField] private CinemachineVirtualCamera Vcamera;
    public VisualMode visualMode;

    private VisualMode lastVisualMode;
    // Start is called before the first frame update
    void Start()
    {
        visualMode = VisualMode.Single;
        SetPlayerVisual();
    }

    // Update is called once per frame
    void Update()
    {
        if(MainMenuStoredData.buttonType == ButtonType.SinglePlayer)
        {
            visualMode = VisualMode.Single;
        }
        else if(MainMenuStoredData.buttonType == ButtonType.MultiplePlayer)
        {
            visualMode = VisualMode.Multiple;
        }

        if (MainMenuStoredData.VcameraPrior)
        {
            Vcamera.Priority = 1;
        }
        else
        {
            Vcamera.Priority = -1;
        }
        if(visualMode != lastVisualMode)
        {
            SetPlayerVisual();
        }
        
        
    }
    private void SetPlayerVisual()
    {
        if (visualMode == VisualMode.Single)
        {
            SinglePlayer.gameObject.SetActive(true);
            foreach (Transform t in MultiplePlayers)
            {
                t.gameObject.SetActive(false);
            }
        }
        else
        {
            SinglePlayer.gameObject.SetActive(false);
            //随机生成MultiplePlayers个数
            Random random = new Random();
            int[] randomNumbers = Enumerable.Range(0, Enum.GetValues(typeof(PlayerVisualColor)).Length - 1)
                                            .OrderBy(x => random.Next()) // 打乱顺序
                                            .Take(MultiplePlayers.Count()) // 取前 4 个
                                            .ToArray();
            int index = 0;
            foreach (Transform t in MultiplePlayers)
            {
                t.gameObject.SetActive(true);
                t.GetComponent<PlayerVisual>().SetVisualColorFromIndex(randomNumbers[index]);
                index += 1;
            }
        }
        lastVisualMode = visualMode;
    }
}
