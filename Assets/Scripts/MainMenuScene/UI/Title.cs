using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    [SerializeField] private Image titleImage;
    [SerializeField] private float duration;

    private Tween fadeTween;
    // Start is called before the first frame update
    void Start()
    {
        if(duration < 0.1f)
        {
            duration = 0.3f;
        }

        // 透明度从 1 -> 0.6 -> 1 无限循环
        fadeTween = titleImage.DOFade(0.6f, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnDestroy()
    {
        fadeTween.Kill();
    }
}
