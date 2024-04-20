using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    #region Move,Attack

    [Header("Movement")]
    [SerializeField] private float moveOffset;
    [SerializeField] private float moveDurationTime;
    public float MoveOffset => moveOffset;
    public float MoveDurationTime => moveDurationTime;
    
    [Header("Shooting")]
    [SerializeField] private float shootDelay;
    private float shootDelayOrigin;

    [SerializeField] private Transform shootPosTransform;
    public float ShootSpeed => shootDelay;
    public Transform ShootPosTransform => shootPosTransform;

    #endregion

    [HideInInspector]
    public List<PoolableHuman> PoolableHumans = new ();

    [HideInInspector]
    public bool IsCanShoot = true;

    private CannonState IdleState, ShootState, MoveState;

    private CannonStateContext cannonStateContext;

    private Animator animator;
    public Animator Animator => animator;

    [HideInInspector]
    public LevelManager levelController;

    [SerializeField] private AudioClip shootAudioClip;
    public AudioClip ShootAudioClip => shootAudioClip;

    private bool notCanTouch = false;
    private float touchCooldown = 0.1f; 
    private float lastTouchTime;

    private void Start()
    {
        animator = GetComponent<Animator>();

        IdleState = gameObject.AddComponent<IdleCannonState>();
        ShootState = gameObject.AddComponent<ShootCannonState>();
        MoveState = gameObject.AddComponent<MoveCannonState>();
        levelController = FindObjectOfType<LevelManager>();

        IdleState.SetUp(this);
        ShootState.SetUp(this);
        MoveState.SetUp(this);

        cannonStateContext = new CannonStateContext(IdleState);
    }

	private void OnEnable()
	{
        shootDelayOrigin = shootDelay;
        UserDataManager.Instance.OnFireRateLevelChanged += FireRateLevelChangeHandler;
	}

	private void OnApplicationQuit()
	{
        UserDataManager.Instance.OnFireRateLevelChanged -= FireRateLevelChangeHandler;
	}

	public void FireRateLevelChangeHandler(int level)
	{
        shootDelay = shootDelayOrigin * StatValueCalculator.FireRateMultiplier(level);
	}

	private void Update()
    {
        #region Mobile Input

        // For Game Scene Test
        if (false == IsCanShoot)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began && notCanTouch) // 터치가 시작되고, 쏠 수 있는 상태인 경우
            {
                Shoot(); 
                lastTouchTime = Time.time;
                notCanTouch = false; 
            }
        }

        if (false == notCanTouch && Time.time >= lastTouchTime + touchCooldown)
        {
            notCanTouch = true; // 쏠 수 있는 상태로 전환
        }

        #endregion

        #region Keyboard Input

        if (Input.GetMouseButton(0))
        {
            Shoot();
        }
        else
        {
            Idle(); 
        }

        #endregion

        // Debug.Log(cannonStateContext.CurrentState.GetType().Name);
    }

    public void Idle()
    {
        cannonStateContext.Transition(IdleState);
    }

    public void Shoot()
    {
        cannonStateContext.Transition(ShootState);
    }

    public void Move()
    {
        cannonStateContext.Transition(MoveState);
    }

    public void ActivateShooting()
    {
        IsCanShoot = true;
    }

    public void DeactivateShooting()
    {
        IsCanShoot = false;
    }

    public void RestHumanDestroy()
    {
        foreach (PoolableHuman human in PoolableHumans)
        {
            if (human.gameObject.activeSelf)
            {
                PoolManager.Instance.Push(human);
            }
        }
        PoolableHumans.Clear();
    }
}
