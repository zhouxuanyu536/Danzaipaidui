using UnityEngine;
using UnityEngine.UI;

public enum NoticeType
{
    ChangeColor,
    Move
}
public class Notice : MonoBehaviour
{
    [SerializeField] private Image Yes;
    [SerializeField] private Image No;

    private bool isIt;
    [SerializeField] private CubeOperator operatingCube;
    public NoticeType noticeType;

    public void SetYesOrNoLocal()
    {
        if (isIt)
        {
            Yes.enabled = true;
            No.enabled = false;
        }
        else
        {
            Yes.enabled = false;
            No.enabled = true;
        }
    }
    private void Start()
    {
        SetYesOrNoLocal();
    }
    private void Update()
    {
        if (GameManager.Instance.GetGameState() != 2) return;
        if(Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.K))
        {
            isIt = !isIt;
            SetYesOrNoLocal();
        }

        if (Input.GetKeyDown(KeyCode.Return) && isIt)
        {
            //执行相关操作
            if(noticeType == NoticeType.ChangeColor)
            {
                operatingCube.ChangeCubeColorServerRpc();
            }
            else
            {
                operatingCube.MoveCubeServerRpc();
            }
            
        }
    }
    private void OnEnable()
    {
        isIt = true;
    }
    private void OnDisable()
    {
        isIt = false;
    }
}
