using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBackground : MonoBehaviour
{
    private Material uiImageMaterial;
    [SerializeField] private Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        uiImageMaterial = GetComponent<Image>().material;

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 localPoint;
        RectTransform canvasRect = canvas.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            null, // Overlay 模式不需要 Camera
            out localPoint
        );

        float transformRelativePosX = transform.GetComponent<RectTransform>().localPosition.x;
        float transformRelativePosY = transform.GetComponent<RectTransform>().localPosition.y;

        // 计算屏幕中心为 (0,0)，边缘为 (-1,1)
        float normalizedX = (localPoint.x - transformRelativePosX) / (canvasRect.rect.width / 2);
        float normalizedY = (localPoint.y - transformRelativePosY) / (canvasRect.rect.height / 2);

        float speed = uiImageMaterial.GetFloat("_speed");
        float sensitivity = uiImageMaterial.GetFloat("_sensitivity");
        float speedX = speed * normalizedX;
        float speedY = speed * normalizedY;
        Vector2 offset = uiImageMaterial.GetVector("_offset");
        offset.x += speedX * sensitivity * Time.deltaTime;
        offset.y += speedY * sensitivity * Time.deltaTime;
        uiImageMaterial.SetVector("_offset", new Vector2(offset.x,offset.y));

    }
}
