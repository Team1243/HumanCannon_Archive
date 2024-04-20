using TMPro;

public class LevelUpButtonUI : ButtonUI
{
    public LevelUpType levelUpType;

	private TextMeshProUGUI levelTxt;
	private TextMeshProUGUI priceTxt;

	private void OnEnable()
	{
		levelTxt = transform.Find("Level_Text").GetComponent<TextMeshProUGUI>();
		priceTxt = transform.Find("Price_Text").GetComponent<TextMeshProUGUI>();

		switch (levelUpType)
		{
			case LevelUpType.HumanSpeed:
				UserDataManager.Instance.OnHumanSpeedLevelChanged += LevelChangeHandler;
				break;
			case LevelUpType.FireRate:
				UserDataManager.Instance.OnFireRateLevelChanged += LevelChangeHandler;
				break;
		}
	}

	private void OnApplicationQuit()
	{
		switch (levelUpType)
		{
			case LevelUpType.HumanSpeed:
				UserDataManager.Instance.OnHumanSpeedLevelChanged -= LevelChangeHandler;
				break;
			case LevelUpType.FireRate:
				UserDataManager.Instance.OnFireRateLevelChanged -= LevelChangeHandler;
				break;
		}
	}

	public void LevelChangeHandler(int level)
	{
		levelTxt.text = $"Level {level}";
		switch (levelUpType)
		{
			case LevelUpType.HumanSpeed:
				priceTxt.text = StatValueCalculator.HumanSpeedLevelUpPrice(level).ToString();
				break;
			case LevelUpType.FireRate:
				priceTxt.text = StatValueCalculator.FireRateLevelUpPrice(level).ToString();
				break;
		}
	}
}
