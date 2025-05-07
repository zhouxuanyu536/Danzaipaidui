using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.AudioSettings;

public class CameraAdjust : MonoBehaviour
{
    private CinemachineFreeLook freeLookCamera;


    [SerializeField] private float rigMinRadius;
    [SerializeField] private float rigMaxRadius;

    [SerializeField] private float co; //系数

    private PlayerInputController playerInputController;
    private void Start()
    {
        freeLookCamera = GetComponent<CinemachineFreeLook>();
        rigMinRadius = 16f;
        rigMaxRadius = 25f;
        co = 0.5f;
        playerInputController = UnityTools.Instance.GetPlayerInputController();
        playerInputController.Player.Enable();
        freeLookCamera.m_YAxis.Value = 0.5f;
    }
    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isPaused && !GameManager.Instance.GameOnLoad)
        {
            //相机停止转动
            freeLookCamera.m_XAxis.m_MaxSpeed = 0f; // 停止水平旋转
            freeLookCamera.m_YAxis.m_MaxSpeed = 0f; // 停止垂直旋转
            return;
        }
        else
        {
            //相机恢复转动
            freeLookCamera.m_XAxis.m_MaxSpeed = 800f;
            freeLookCamera.m_YAxis.m_MaxSpeed = 4f;
        }
        
        
        bool isMobile = Application.isMobilePlatform;
        if (isMobile || Touchscreen.current != null)
        {
            freeLookCamera.m_XAxis.m_InputAxisName = "";
            freeLookCamera.m_YAxis.m_InputAxisName = "";
            Debug.Log("运行在手机/平板上");
            CheckTouchMove();
        }
        else
        {
            freeLookCamera.m_XAxis.m_InputAxisName = "Mouse X";
            freeLookCamera.m_YAxis.m_InputAxisName = "Mouse Y";
            Debug.Log("运行在电脑上（PC/Mac/WebGL）");
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll < 0f)
            {
                co += 0.04f;
            }
            else if (scroll > 0f)
            {
                co -= 0.04f;
            }
        }
        if (Gamepad.current != null)
        {
            CheckGamepadRightStickMove(); 
            CheckGamepadButtonWest(); //拉近视角
            CheckGamepadButtonEast(); //拉远视角
        }
        // 限制 co 在 0~1 之间
        co = Mathf.Clamp01(co);

        // Lerp 插值计算
        freeLookCamera.m_Orbits[0].m_Radius = freeLookCamera.m_Orbits[1].m_Radius = freeLookCamera.m_Orbits[2].m_Radius = Mathf.Lerp(rigMinRadius, rigMaxRadius, co);

    }

    private Vector2 t;
    private Touch touchZero;
    private Touch touchOne;
    private void CheckTouchMove()
    {
        List<int> touchList = new List<int>();
        //检测有多少手指
        if(Input.touchCount >= 1)
        {
            for(int i = 0;i < Input.touchCount;i++) 
            {
                Touch touch = Input.GetTouch(0);
                //左侧1/4往右可控制
                Debug.Log("screenWidth:" + Screen.width / 2);
                if (touch.position.y < Screen.height * 3 / 8)
                {
                    if (touch.position.x < Screen.width / 4 || touch.position.x > Screen.width / 2)
                    {
                        continue;
                    }
                }
                else if (touch.position.y > Screen.height * 3 / 4)
                {
                    if (touch.position.x < Screen.width / 4)
                    {

                        continue;
                    }
                }
                touchList.Add(i);
            }
        }
        if(touchList.Count >= 2)
        {
            //缩放
            touchZero = Input.GetTouch(touchList[0]);
            touchOne = Input.GetTouch(touchList[1]);
            float lastChange = 0;
           
            lastChange = (touchOne.position - touchZero.position).magnitude - 
            ((touchOne.position - touchOne.deltaPosition) - (touchZero.position - touchZero.deltaPosition)).magnitude;
            
            if (lastChange > 0)
            {
                co -= 0.04f * (lastChange / 100f);
            }
            else
            {
                co += 0.04f * (lastChange / 100f);
            }
        }
        else if (touchList.Count == 1)
        {
            Touch touch = Input.GetTouch(touchList[0]);
            Debug.Log("touchPosition" + touch.position);
            freeLookCamera.m_XAxis.m_InputAxisValue = (touch.deltaPosition).x / 25f;
            freeLookCamera.m_YAxis.m_InputAxisValue = (touch.deltaPosition).y / 25f;
           
        }
        else
        {
            freeLookCamera.m_XAxis.m_InputAxisValue = 0;
            freeLookCamera.m_YAxis.m_InputAxisValue = 0;
        }
    }
    
    private void CheckGamepadRightStickMove()
    {
        Vector2 gamepadAdjustCameraValue = playerInputController.Player.Gamepad_AdjustCameraHV.ReadValue<Vector2>();
        if(gamepadAdjustCameraValue.magnitude > 0)
        {
            freeLookCamera.m_XAxis.m_InputAxisName = "";
            freeLookCamera.m_YAxis.m_InputAxisName = "";
            freeLookCamera.m_XAxis.m_InputAxisValue = gamepadAdjustCameraValue.x / 5f;
            freeLookCamera.m_YAxis.m_InputAxisValue = gamepadAdjustCameraValue.y / 5f;
        }
    }
    private void CheckGamepadButtonWest()
    {
        float isButtonWestPressed = playerInputController.Player.Gamepad_AdjustCameraN.ReadValue<float>();
        Debug.Log("isButtonWestPressed:" + isButtonWestPressed);
        if(isButtonWestPressed > 0)
        {
            if (Application.isMobilePlatform || Touchscreen.current != null)
            {
                co -= 0.01f;
            }
            else
            {
                co -= 0.002f;
            }
        }
    }
    private void CheckGamepadButtonEast()
    {
        float isButtonEastPressed = playerInputController.Player.Gamepad_AdjustCameraF.ReadValue<float>();
        if (isButtonEastPressed > 0)
        {
            if (Application.isMobilePlatform || Touchscreen.current != null)
            {
                co += 0.01f;
            }
            else
            {
                co += 0.002f;
            }
        }
    }
}
