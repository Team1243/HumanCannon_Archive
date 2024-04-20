using System.Collections;
using UnityEngine;

public class FireGameMode : GameModeBase
{
	private CannonController cannonController;

	public override void Init(LevelManager manager)
	{
		base.Init(manager);

		cannonController = manager.CannonController;
	}

	public override void Appear()
	{
		foreach (LevelObject level in levels)
		{
			level.Appear();
		}
		CheckAnimationEnd();
	}

	public override void Disappear()
	{
		manager.StageExist = false;

		cannonController.DeactivateShooting(); // 대포 발사 중지
		cannonController.RestHumanDestroy(); // 대포에서 발사된 모든 사람 파괴

		foreach (LevelObject level in levels)
		{
			level.Disappear();
		}
		CameraManager.Instance.CameraShake(1, 2);
		CheckAnimationEnd();
	}

	public override void Clear()
	{
		foreach (LevelObject level in levels)
		{
			level.Clear();
		}
		StartCoroutine(CoinGainTime());
	}

	private IEnumerator CoinGainTime()
	{
		yield return new WaitForSeconds(manager.levelData.coinGainTime);

		manager.ChangeState(GameLevelState.Disappear);
	}

	public override void Over()
	{
		manager.StageExist = true;
		cannonController.RestHumanDestroy();
		cannonController.DeactivateShooting();
		foreach (LevelObject level in levels)
		{
			level.Over();
		}
	}

	public override void Running()
	{
		cannonController.ActivateShooting();
		GameManager.Instance.ChangeGameState(GameEventType.Start);
	}

	public override void Ready()
	{
		manager.MoveToNextLevel();
		CreateLevel(manager.CurrentStage);
	}

	public override void CreateLevel(int stage)
	{
		foreach (LevelObject level in levels)
		{
			level.CreateLevel(stage);
		}
	}

	public override void OnAnimationEnd()
	{
		if (manager.CurrentLevelState == GameLevelState.Appear)
		{
			manager.StageExist = true;
			manager.ChangeState(GameLevelState.Running);
		}
		else if (manager.CurrentLevelState == GameLevelState.Disappear)
		{
			GameManager.Instance.ChangeGameState(GameEventType.Ready); // 장애물이 사라지고 다음 스테이지로 이동하는 동안에는 게임 상태를 Ready로 변경한다
		}
	}
}
