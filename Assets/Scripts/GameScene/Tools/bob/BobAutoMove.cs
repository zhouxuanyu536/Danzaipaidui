using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobAutoMove : MonoBehaviour
{
    [SerializeField] private Transform root;
    [SerializeField] private bool ClockWiseFirst;
    [SerializeField] private bool isDoubleDirection;
    [SerializeField] private bool isStopped;
    [SerializeField, Range(0f, 90f)] private float rotateAngle;
    private Sequence sequence;
    
    // Start is called before the first frame update
    void Start()
    {
        if (rotateAngle == 0f) rotateAngle = 45;
        sequence = DOTween.Sequence();
        // 第一次 Loop：旋转到 30° 并 Yoyo 无限循环
        sequence.Append(root.transform.DORotate(new Vector3(0, 0, rotateAngle), 1f)
            .SetLoops(2, LoopType.Yoyo)); // 播放 2 次（1次正向 + 1次反向 = 来回1次）

        // 第二次 Loop：旋转到 -30° 并 Yoyo 无限循环
        sequence.Append(root.transform.DORotate(new Vector3(0, 0, -rotateAngle), 1f)
            .SetLoops(2, LoopType.Yoyo)); // 再来回 1 次


        float totalDuration = sequence.Duration();

        
        float randomTime = Random.Range(0, totalDuration);

        // 直接跳到随机时间点
        sequence.Goto(randomTime, true);

        sequence.SetLoops(-1); // 无限循环
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance != null 
            && GameManager.Instance.state.Value != GameManager.State.GamePlaying
            && !GameManager.Instance.GameOnLoad)
        {
            sequence.timeScale = 0f;
        }
        else
        {
            sequence.timeScale = 1f;
        }
    }
    private void OnDestroy()
    {
        sequence.Kill();
    }
}
