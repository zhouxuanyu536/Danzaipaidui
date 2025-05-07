using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoostCoolDown : MonoBehaviour
{
    private float coolDownTime;
    private Image coolDownImage;
    [SerializeField] private TextMeshProUGUI coolDownNumText;
    [SerializeField] private Button boostButton;
    private readonly float COOLDOWNTIMEAMOUNT = 5f;
    // Start is called before the first frame update
    void Start()
    {
        coolDownImage = GetComponent<Image>();
        Hide();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePanel();
        if (coolDownTime > 0)
        {
            boostButton.interactable = false;
            coolDownTime -= Time.deltaTime;
        }
        else
        {
            coolDownTime = 0;
            boostButton.interactable = true;
            Hide();
        }
    }
    public void StartCoolDown()
    {
        coolDownTime = COOLDOWNTIMEAMOUNT;
        Show();
    }

    private void UpdatePanel()
    {
        if(coolDownTime > 0)
        {
            coolDownImage.fillAmount = coolDownTime / COOLDOWNTIMEAMOUNT;
        }
        else
        {
            coolDownImage.fillAmount = 0;
        }
        coolDownNumText.text = Mathf.CeilToInt(coolDownTime).ToString();
    }

    public bool IsBoostCoolingDown()
    {
        return gameObject.activeSelf;
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
