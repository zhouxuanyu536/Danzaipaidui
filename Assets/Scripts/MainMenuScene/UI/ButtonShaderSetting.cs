using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonShaderMode
{
    MainMenuMode,
}
public class ButtonShaderSetting : MonoBehaviour
{
    [SerializeField] private ButtonShaderMode mode;
    [SerializeField] private TextMeshProUGUI text;

    private Material material;
    private ButtonPointerHandler buttonPointerHandler;

    private float mainMenu_rotation;
    private float mainMenu_rotateDelta;
    private float mainMenu_moveSpeed;
    private float mainMenu_moveSpeedAcc; //每帧加速度
    private bool mainMenu_textAnimationStarted;
    private Tween mainMenu_textFadeTween;
    private float mainMenu_fadeDuration;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Image>().material;
        buttonPointerHandler = GetComponent<ButtonPointerHandler>();
        mainMenu_rotation = -15f;
        mainMenu_rotateDelta = 30 * Time.deltaTime;
        mainMenu_moveSpeed = 10f;
        mainMenu_moveSpeedAcc = 40 * Time.deltaTime;
        mainMenu_fadeDuration = 0.25f;
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == ButtonShaderMode.MainMenuMode)
        {
            float move;
            if (buttonPointerHandler.isPointerOver)
            {
                mainMenu_rotation = Mathf.MoveTowards(mainMenu_rotation, -30f, mainMenu_rotateDelta);
                
                mainMenu_moveSpeed = Mathf.MoveTowards(mainMenu_moveSpeed,30f,mainMenu_moveSpeedAcc);
                move = material.GetFloat("_move");
                MainMenu_StartTextAnimation();


            }
            else
            {
                mainMenu_rotation = Mathf.MoveTowards(mainMenu_rotation, -15f, mainMenu_rotateDelta);

                mainMenu_moveSpeed = Mathf.MoveTowards(mainMenu_moveSpeed, 10f, mainMenu_moveSpeedAcc);
                move = material.GetFloat("_move");
                MainMenu_StopTextAnimation();
            }
            material.SetFloat("_rotation", mainMenu_rotation); //注意大小写
            material.SetFloat("_move", move + mainMenu_moveSpeed * Time.deltaTime);

        }
    }
    private void MainMenu_StartTextAnimation()
    {
        if (!mainMenu_textAnimationStarted) 
        {
            mainMenu_textFadeTween = text.DOFade(0.7f, mainMenu_fadeDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
            mainMenu_textAnimationStarted = true;
        }
    }
    private void MainMenu_StopTextAnimation()
    {
        mainMenu_textFadeTween.Kill();
        mainMenu_textAnimationStarted = false;
        text.alpha = 1f;
    }
}
