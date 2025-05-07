using Cinemachine;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCountDownTimer : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI CountDownTimer;
    private float timeLeft = 4.5f;
    [SerializeField] private GameSoundConfig gameSoundConfig;
    [SerializeField] private GameObject PauseButton;
    [SerializeField] private GameObject SettingsButton;
    [SerializeField] private GameObject Stick;
    [SerializeField] private GameObject JumpButton;
    [SerializeField] private GameObject SprintButton;
    [SerializeField] private CanvasGroup ProgressBarsCanvasGroup;
    private bool isInitialized;
    private bool isLoaded;

    private Animator animator;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }
    public override void OnNetworkSpawn()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            GetTimeCountDownStartServerRpc(NetworkManager.Singleton.LocalClientId);
        }
        CountDownTime(timeLeft);
    }
    private void OnEnable()
    {
        if(!isLoaded)
        {
            StartCoroutine(CountDownTime(timeLeft));
        }
        else
        {
            StartCoroutine(CountDownTime(3f));
        }
        isLoaded = true;
        
    }
    private IEnumerator CountDownTime(float timeLeft)
    {
        if (!isInitialized)
        {
            HideAndSetActive();
        }
        int previousTimeLeft = -1;
        HideAllCanvasComponents();
        while (timeLeft > 0)
        {
            while (!GameManager.Instance.PlayerAllSpawned.Value && !isInitialized)
            {
                yield return null;
            }
            if (!isInitialized)
            {
                LevelDetails levelDetails;
                while ((levelDetails = FindObjectOfType<LevelDetails>()) == null)
                    yield return null;
                isInitialized = true;
                CinemachineVirtualCamera cameraPreview = levelDetails.cameraPreview;
                cameraPreview.Priority = 20;
                yield return new WaitForSeconds(2f);
                cameraPreview.Priority = 0;
                //fade in out 1s
                yield return new WaitForSeconds(0.5f);
                ShowAndSetActive();
            }
            timeLeft -= Time.deltaTime;
            Debug.Log("timeLeft:" + timeLeft);
            //校准
            GetTimeCountDownStartServerRpc(NetworkManager.Singleton.LocalClientId);
            int time = Mathf.CeilToInt(timeLeft);
            CountDownTimer.color = Color.white;
            //4.5秒
            if(timeLeft < 4f)
            {
                ShowProgressBars();
            }
            if (timeLeft > 3f)
            {
                CountDownTimer.text = $"Level {GameMultiplayer.Instance.level.Value}";
            }
            else
            {
                animator.enabled = true;
                animator.speed = 1f;
                if (time != previousTimeLeft)
                {
                    if (time <= 0) break;
                    previousTimeLeft = time;
                    animator.SetTrigger("NumberPopup");
                    CountDownTimer.text = time.ToString();
                    UnityTools.Instance.PlayOneShot(audioSource, gameSoundConfig.CountDownClip);
                }

            }
            yield return null; 
        }
        ShowAllCanvasComponentsExceptProgressBars();
        if (IsServer)
        {
            GameManager.Instance.SetGameStateServerRpc(2);
        }
        Hide();
    }

    [ServerRpc(RequireOwnership = false)]
    private void GetTimeCountDownStartServerRpc(ulong clientId)
    {
        SetTimeCountDownStartClientRpc(timeLeft,clientId);
    }
    [ClientRpc]
    private void SetTimeCountDownStartClientRpc(float timeLeft,ulong clientId)
    {
        if(clientId == NetworkManager.Singleton.LocalClientId)
        {
            this.timeLeft = timeLeft;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void ShowAndSetActive()
    {
        CountDownTimer.color = new Color(0,0,0,1);
    }
    public void HideAndSetActive()
    {
        CountDownTimer.color = new Color(0, 0, 0, 0);
    }

    public void ShowAllCanvasComponentsExceptProgressBars()
    {
        PauseButton.SetActive(true);
        SettingsButton.SetActive(true);
        Stick.SetActive(true);
        JumpButton.SetActive(true);
        SprintButton.SetActive(true);
        Debug.Log("ShowButtons");
    }
    public void ShowProgressBars()
    {
        ProgressBarsCanvasGroup.alpha = 120f / 255f;
    }
    public void HideAllCanvasComponents()
    {
        PauseButton.SetActive(false);
        SettingsButton.SetActive(false);
        Stick.SetActive(false);
        JumpButton.SetActive(false);
        SprintButton.SetActive(false);
        ProgressBarsCanvasGroup.alpha = 0;
        Debug.Log("HideButtons");
    }
}
