
public class LevelUpButtonPanelUI : ButtonPanelUI
{
    private ButtonUI speedUpButtonUI;
    private ButtonUI firerateUpButtonUI;

    protected override void Awake()
    {
        base.Awake();
        speedUpButtonUI = transform.Find("SpeedUp_Button").GetComponent<ButtonUI>();
        firerateUpButtonUI = transform.Find("FirerateUp_Button").GetComponent<ButtonUI>();
    }

	public override void Show()
	{
		base.Show();
		speedUpButtonUI.SetClickEvent(() =>
		{
			UserDataManager.Instance.UpgradeHumanSpeed(1);
		});
		firerateUpButtonUI.SetClickEvent(() => 
		{
			UserDataManager.Instance.UpgradeFireRate(1);
		});
	}

	public override void Hide()
	{
		base.Hide();
		speedUpButtonUI.RemoveAllClickEvent();
		firerateUpButtonUI.RemoveAllClickEvent();
	}
}
