using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvas : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;

    private void Start()
    {
        
    }
    public void UpdateProgress(float progress, string message)
    {
        if(progress >= 0)
        {
            progressBar.value = progress;
        }
        progressText.text = message;
    }
}
