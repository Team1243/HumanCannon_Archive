using UnityEngine;

public class StatValueCalculator
{
    public static int StageClearCoin(int stage)
	{
		int calc = (int)Mathf.Log(stage + 1) * 40;
		return calc;
	}

	///<summary>
	/// 현재 레벨이 level일때 다음 레벨까지 필요한 비용을 반환
	///</summary>
	public static int FireRateLevelUpPrice(int level)
	{
		if (level < 1) return 0;

		int calc = level < 20 ? Mathf.FloorToInt(Mathf.Log(level) * 50 + 100) : (level * 2 + 125);
		return calc;
	}

	///<summary>
	/// 현재 레벨이 level일때 다음 레벨까지 필요한 비용을 반환
	///</summary>
	public static int HumanSpeedLevelUpPrice(int level)
	{
		if (level < 1) return 0;

		int calc = level < 20 ? Mathf.FloorToInt(Mathf.Log(level) * 50 + 30) : (level * 2 + 75);
		return calc;
	}

	public static float FireRateMultiplier(int level)
	{
		float ret = 1f - level * 0.02f;
		ret = Mathf.Clamp(ret, 0.5f, 1f);
		return ret;
	}

	public static float HumanSpeedMultiplier(int level)
	{
		float ret = 1f + level * 0.02f;
		ret = Mathf.Clamp(ret, 1f, 2.5f);
		return ret;
	}
}
