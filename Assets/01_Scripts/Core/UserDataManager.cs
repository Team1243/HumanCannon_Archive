

using System;
using UnityEngine;

public struct UserData
{
	public int coin;
	public int fireRateLevel;
	public int humanSpeedLevel;
}

public class UserDataManager : MonoSingleton<UserDataManager>, IDataObserver
{
	public UserData userData = new UserData() { coin = 0, fireRateLevel = 1, humanSpeedLevel = 1 };

	[SerializeField] private HumanStatListSO humanStatsSO;

	public event Action<int> OnCoinChanged;
	public event Action<int> OnFireRateLevelChanged;
	public event Action<int> OnHumanSpeedLevelChanged;

	#region Level Up

	public void UpgradeFireRate(int up = 1)
	{
		int req = 0;
		for (int i = 0; i < up; ++i)
		{
			req += StatValueCalculator.FireRateLevelUpPrice(userData.fireRateLevel + i);
		}
		if (CanReduceCoin(req))
		{
			userData.fireRateLevel += up;
			ReduceCoin(req);
			OnFireRateLevelChanged?.Invoke(userData.fireRateLevel);
		}
	}

	public void UpgradeHumanSpeed(int up = 1)
	{
		int req = 0;
		for (int i = 0; i < up; ++i)
		{
			req += StatValueCalculator.HumanSpeedLevelUpPrice(userData.humanSpeedLevel + i);
		}
		if (CanReduceCoin(req))
		{
			userData.humanSpeedLevel += up;
			ReduceCoin(req);
			OnHumanSpeedLevelChanged?.Invoke(userData.humanSpeedLevel);
			humanStatsSO.speedMul = StatValueCalculator.HumanSpeedMultiplier(userData.humanSpeedLevel);
		}
	}

	#endregion

	#region Coin

	public bool CanReduceCoin(int value) => userData.coin >= value; // 코인 사용 가능 여부 반환
	public void AddCoin(int value) => ModifyCoin(value); // 코인 증가
	public void ReduceCoin(int value) => ModifyCoin(-value); // 코인 감소

	private void ModifyCoin(int change)
	{
		userData.coin += change;
		OnCoinChanged?.Invoke(userData.coin);
	}

	private void Start()
	{
		OnCoinChanged?.Invoke(userData.coin);
		OnFireRateLevelChanged?.Invoke(userData.fireRateLevel);
		OnHumanSpeedLevelChanged?.Invoke(userData.humanSpeedLevel);
		humanStatsSO.speedMul = StatValueCalculator.HumanSpeedMultiplier(userData.humanSpeedLevel);
	}

	#endregion

	#region Save Load

	public void WriteData(ref SaveData data)
	{
		data.coin = userData.coin;
		data.fireRateLevel = userData.fireRateLevel;
		data.humanSpeedLevel = userData.humanSpeedLevel;
	}

	public void ReadData(SaveData data)
	{
		ModifyCoin(data.coin);
		userData.fireRateLevel = UnityEngine.Mathf.Max(data.fireRateLevel, 1);
		userData.humanSpeedLevel = UnityEngine.Mathf.Max(data.humanSpeedLevel, 1);
	}

	#endregion
}
