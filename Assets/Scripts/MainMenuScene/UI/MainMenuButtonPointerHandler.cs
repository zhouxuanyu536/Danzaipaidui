using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuButtonPointerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MainMenuStoredData.VcameraPrior = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MainMenuStoredData.VcameraPrior = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        MainMenuStoredData.VcameraPrior = true;
    }
}
