using UnityEngine;
using DG.Tweening;

public class UIManager : MonoSingleton<UIManager>
{
    private MainScreen mainScreen;

    [SerializeField] private Transform canvasTransform;

    public override void Init()
    {
        DOTween.Init();

        GameEventSystem.Instance.Subscribe(this, GameEventType.Menu, ShowMainScreen);
        GameEventSystem.Instance.Subscribe(this, GameEventType.Ready, HideMainScreen);
    }

    private void OnDisable()
    {
        GameEventSystem.Instance.Unsubscribe(this, GameEventType.Menu, ShowMainScreen);
        GameEventSystem.Instance.Unsubscribe(this, GameEventType.Ready, HideMainScreen);
    }

    public void ShowMainScreen()
    {
        mainScreen = PoolManager.Instance.Pop("MainScreen") as MainScreen;
        mainScreen.transform.SetParent(canvasTransform);
        mainScreen.transform.localPosition = Vector3.zero;
        mainScreen.transform.localScale = Vector3.one;  
    }

    public void HideMainScreen()
    {
        if (mainScreen != null && mainScreen.gameObject.activeSelf)
        {
            mainScreen.DestroyScreen();
        }
    }

    public void ShowPopUp(string poolId)
    {
        PopUpUI popUp = PoolManager.Instance.Pop(poolId) as PopUpUI;
        popUp.transform.SetParent(canvasTransform);
        popUp.transform.localPosition = Vector3.zero;
        popUp.Show();
    }

}
