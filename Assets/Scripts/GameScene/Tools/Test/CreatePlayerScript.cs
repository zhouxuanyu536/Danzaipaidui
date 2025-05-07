using UnityEngine;

public class CreatePlayerScript : MonoBehaviour
{
    private float offset;
    private PlayerInputController inputController;
    private void Start()
    {
        offset = 20f;
        inputController = UnityTools.Instance.GetPlayerInputController();
    }
    private void Update()
    {
        //点击p键，增加玩家并放置到指定位置
        //点击u键，可以切换玩家视角
        
    }
}
