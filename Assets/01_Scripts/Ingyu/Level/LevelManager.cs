using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// 각종 함수를 통해 장애물과 맵을 관리하는 함수
/// </summary>
public class LevelManager : MonoSingleton<LevelManager>, IDataObserver
{
	[Header("Variables")]
	private List<GameModeBase> gameModes;
	private List<LevelObject> levels;

	[Header("Value & Setting")]
	private GameModeBase currentGameMode;
	public GameLevelState CurrentLevelState { get; private set; }
	public int CurrentStage { get; private set; } = 1;
	[SerializeField] private float levelDistance = 100f;

	[Header("Flags")]
	[HideInInspector]
	public bool animationEnd = true;
	private bool stageExist = true;
	public bool StageExist { get => stageExist; set => stageExist = value; }

	[Header("References")]
	public LevelDataSO levelData;
	private CannonController cannonController;
	public CannonController CannonController => cannonController;
	private Tower tower;

	[Header("Event")]
	public Action<int> OnStageChanged;

	private void Awake()
	{
		cannonController = FindObjectOfType<CannonController>();
		cannonController.DeactivateShooting();
		tower = transform.Find("Tower").GetComponent<Tower>();

		levels = GetComponentsInChildren<LevelObject>().ToList();
		foreach (LevelObject level in levels)
		{
			level.manager = this;
		}

		gameModes = GetComponentsInChildren<GameModeBase>().ToList();
		foreach (GameModeBase gameMode in gameModes)
		{
			gameMode.Init(this);
		}
		currentGameMode = gameModes[0];
	}

	private void Start()
	{
		CurrentLevelState = GameLevelState.None;
		currentGameMode.CreateLevel(CurrentStage);
        OnStageChanged?.Invoke(CurrentStage);
    }

	public void Attack(int damage)
	{
		tower.Damage(damage);
	}

	#region Event Setting
	private void OnEnable()
	{
		GameEventSystem.Instance.Subscribe(this, GameEventType.Clear, OnGameEventClearHandle);
		GameEventSystem.Instance.Subscribe(this, GameEventType.Ready, OnGameEventReadyHandle);
		GameEventSystem.Instance.Subscribe(this, GameEventType.Over, OnGameEventOverHandle);
	}

	private void OnApplicationQuit()
	{
		GameEventSystem.Instance.Unsubscribe(this, GameEventType.Clear, OnGameEventClearHandle);
		GameEventSystem.Instance.Unsubscribe(this, GameEventType.Ready, OnGameEventReadyHandle);
		GameEventSystem.Instance.Unsubscribe(this, GameEventType.Over, OnGameEventOverHandle);
	}

	private void OnGameEventClearHandle() => ChangeState(GameLevelState.Clear);
	private void OnGameEventReadyHandle() => ChangeState(GameLevelState.Ready);
	private void OnGameEventOverHandle() => ChangeState(GameLevelState.Over);
	#endregion

	#region States
	public void ChangeState(GameLevelState nextState)
	{
		if (CurrentLevelState == nextState) return;

		CurrentLevelState = nextState;

		switch (nextState)
		{
			case GameLevelState.None: // 메뉴 상태 
				break;
			case GameLevelState.Running: // 게임 활성화
				Running();
				break;
			case GameLevelState.Clear: // 게임 클리어(장애물 퇴장 및 코인 획득)
				Clear();
				break;
			case GameLevelState.Ready: // 대포 움직임 등 다음 단계로 넘어가는 애니메이션
				Ready();
				break;
			case GameLevelState.Disappear: // 성 폭발 등 철거 애니메이션
				DisappearLevel();
				break;
			case GameLevelState.Appear: // 등장 애니메이션
				Appear();
				break;
			case GameLevelState.Over: // 게임 오버
				Over();
				break;
			default:
				break;
		}
	}

	// 게임을 시작
	private void Running()
	{
		currentGameMode.Running();
	}

	private void Over()
	{
		currentGameMode.Over();
	}

	private void Clear()
	{
		currentGameMode.Clear();
	}

	private void Ready()
	{
		if (stageExist) // GameEventType Menu에서 Ready로 변경하였을 시에는 등장 상태로 이동
		{
			ChangeState(GameLevelState.Appear);
			return;
		}

		++CurrentStage;
		OnStageChanged?.Invoke(CurrentStage);

		currentGameMode.Ready();
		SaveLoadManager.Instance.SaveData();
	}

	#region Animations
	public void MoveToNextLevel()
	{
		cannonController.Move();
		transform.position += new Vector3(0, 0, levelDistance);
	}

	/// <summary>
	/// 장애물 등의 등장 애니메이션을 실행
	/// </summary>
	public void Appear()
	{
		currentGameMode.Appear();
	}

	/// <summary>
	/// 타워, 장애물 등의 퇴장 애니메이션을 실행
	/// </summary>
	public void DisappearLevel()
	{
		currentGameMode.Disappear();
	}
	#endregion
	#endregion

	#region Save Load
	public void WriteData(ref SaveData data)
	{
		data.stage = CurrentStage;
	}

	public void ReadData(SaveData data)
	{
		CurrentStage = data.stage;
		OnStageChanged?.Invoke(CurrentStage);
	}
	#endregion
}
