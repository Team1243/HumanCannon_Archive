using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// ���� �Լ��� ���� ��ֹ��� ���� �����ϴ� �Լ�
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
			case GameLevelState.None: // �޴� ���� 
				break;
			case GameLevelState.Running: // ���� Ȱ��ȭ
				Running();
				break;
			case GameLevelState.Clear: // ���� Ŭ����(��ֹ� ���� �� ���� ȹ��)
				Clear();
				break;
			case GameLevelState.Ready: // ���� ������ �� ���� �ܰ�� �Ѿ�� �ִϸ��̼�
				Ready();
				break;
			case GameLevelState.Disappear: // �� ���� �� ö�� �ִϸ��̼�
				DisappearLevel();
				break;
			case GameLevelState.Appear: // ���� �ִϸ��̼�
				Appear();
				break;
			case GameLevelState.Over: // ���� ����
				Over();
				break;
			default:
				break;
		}
	}

	// ������ ����
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
		if (stageExist) // GameEventType Menu���� Ready�� �����Ͽ��� �ÿ��� ���� ���·� �̵�
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
	/// ��ֹ� ���� ���� �ִϸ��̼��� ����
	/// </summary>
	public void Appear()
	{
		currentGameMode.Appear();
	}

	/// <summary>
	/// Ÿ��, ��ֹ� ���� ���� �ִϸ��̼��� ����
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
