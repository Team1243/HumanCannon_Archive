using System.Collections;
using UnityEngine;

public class MainScreen : PoolableMono
{
    private IUIComponent funcButtonPanelUI;
    private IUIComponent noticePanelUI;
    private IUIComponent levelUpButtonPanelUI;

    private ButtonUI playButtonUI;    

    private void Awake()
    {
        playButtonUI = transform.Find("Play_Button").GetComponent<ButtonUI>();

        funcButtonPanelUI = transform.GetChild(1).GetComponent<IUIComponent>();
        noticePanelUI = transform.GetChild(2).GetComponent<IUIComponent>();
        levelUpButtonPanelUI = transform.GetChild(3).GetComponent<IUIComponent>();
    }

    public override void Init()
    {
        funcButtonPanelUI.Show();
        noticePanelUI.Show();
        levelUpButtonPanelUI.Show();

        playButtonUI.SetClickEvent(() =>
        {
            GameManager.Instance.ChangeGameState(GameEventType.Ready);
        });
    }

	public void DestroyScreen()
    {
        playButtonUI.RemoveAllClickEvent();

        funcButtonPanelUI.Hide();
        noticePanelUI.Hide();
        levelUpButtonPanelUI.Hide();

        StartCoroutine(DestroyWithDelay(1f));
    }

    private IEnumerator DestroyWithDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        PoolManager.Instance.Push(this);
    }
}
