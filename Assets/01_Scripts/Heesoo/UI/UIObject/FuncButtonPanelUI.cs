using UnityEngine;

public class FuncButtonPanelUI : MonoBehaviour, IUIComponent
{
    private ButtonPanelUI leftPanelUI;

    private ButtonUI shareButtonUI;
    private ButtonUI settingButtonUI;

    private void Awake()
    {
        leftPanelUI = transform.GetChild(0).GetComponent<ButtonPanelUI>();
        shareButtonUI = leftPanelUI.transform.Find("Share_Button").GetComponent<ButtonUI>();
        settingButtonUI = leftPanelUI.transform.Find("Setting_Button").GetComponent<ButtonUI>();
    }

    public void Show()
    {
        shareButtonUI.SetClickEvent(() =>
        {
            GameManager.Instance.ShareInAnroid();
        });
        settingButtonUI.SetClickEvent(() =>
        {
            UIManager.Instance.ShowPopUp("SettingPopUp");
        });

        leftPanelUI.Show();
    }

    public void Hide()
    {
        shareButtonUI.RemoveAllClickEvent();
        settingButtonUI.RemoveAllClickEvent();

        leftPanelUI.Hide();
    }           
}
