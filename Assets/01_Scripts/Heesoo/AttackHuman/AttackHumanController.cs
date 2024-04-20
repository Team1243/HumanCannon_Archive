using System.Collections;
using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;

public class AttackHumanController : MonoBehaviour, IDamageable
{
    private PoolableHuman poolalbeHuman;

    private SkinnedMeshRenderer skinnedMeshRenderer;

    private StateMachine<AttackHumanController> thisStateMachine;

    private Coroutine runningDelayCoroutine = null;

    private readonly string enemyString = "Enemy";
    private readonly string playerString = "Runner";

    [SerializeField] private bool isBigHuman = false;

    #region Health

    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    public int CurrentHealth => currentHealth;
    public bool IsAlive { get; private set; }

    #endregion

    #region Move

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 60;
    public float MoveSpeed => moveSpeed;

    #endregion

    #region Attack

    [Header("Serching")]
    [SerializeField] private string targetTagString;
    private Transform target;
    public Transform Target => target;
    public Transform SearchEnemy()
    {
        target = null;

        var targets = GameObject.FindGameObjectsWithTag(targetTagString).ToList();
        if (targets.Count == 0)
        {
            Debug.LogError("Target is not exist");
            return target;
        }

        target = targets.OrderBy((target) =>
        {
            return Vector3.Distance(transform.position, target.transform.position);
        }).FirstOrDefault().transform;

        return target;
    }

    [Header("AttackValue")]
    [SerializeField] private int attackDamage = 5;
    public int AttackDamage => attackDamage;

    [SerializeField] private float attackCoolTime = 1.5f;
    public float AttackCoolTime => attackCoolTime;

    [SerializeField] private float attackRange;
    public float AttackRange => attackRange;
    public bool IsCanAttack { get; set; }

    public float GetDistanceToTarget
    {
        get
        {
            if (target == null)
            {
                Debug.LogError("Target is not exist");
                return 0;
            }

            float distance = Vector3.Distance(transform.position, target.position);
            return distance;
        }
    }
    public Vector3 GetDirectionToTarget
    {
        get
        {
            if (target == null)
            {
                Debug.LogError("Target is not exist");
                return Vector3.zero;
            }

            Vector3 direction = target.position - transform.position;
            return direction;
        }
    }



    #endregion

    public void BigHumanSetting(int attackDamage, Color bodyColor)
    {
        if (!isBigHuman) return;

        this.attackDamage = attackDamage;
        skinnedMeshRenderer.material.color = bodyColor;

        transform.DOScale(new Vector3(2, 2, 2), 1f);
    }

    #region MainLogic

    public void Init()
    {
        thisStateMachine = new StateMachine<AttackHumanController>(this, new WaitState());
        thisStateMachine.AddStateList(new IdleState());
        thisStateMachine.AddStateList(new ChaseState());
        thisStateMachine.AddStateList(new AttackState());
        thisStateMachine.AddStateList(new WaitState());

        IsAlive = true;
        IsCanAttack = true;
        currentHealth = maxHealth;
        transform.localPosition = Vector3.zero;
    }

    private void Awake()
    {
        poolalbeHuman = GetComponent<PoolableHuman>();
        skinnedMeshRenderer = transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>();    
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (IsAlive)
        {
            thisStateMachine.Update(Time.deltaTime);
        }
    }

    public void OnDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        // ui update

        if (currentHealth <= 0)
        {
            Die();
            IsAlive = false;
        }
    }

    private void Die()
    {
        Debug.Log("Die");
        Destroy(gameObject);
        thisStateMachine = null;
        // PoolManager.Instance.Push(poolalbeHuman);
    }

    #endregion

    #region UtillFunc

    public float GetAnimationClipLength(Animator _animator, string clipName)
    {
        float time = 0;

        AnimationClip[] clipList = _animator.runtimeAnimatorController.animationClips;
        AnimationClip clip = Array.Find(clipList, (c) => c.name == clipName);

        if (clip != null)
        {
            time = clip.length;
        }
        else
        {
            Debug.Log($"{clipName} animation clip is null");
        }
        return time;
    }

    public void ActionAfterCoolTime(float coolTime, Action action)
    {
        if (runningDelayCoroutine != null)
        {
            StopCoroutine(runningDelayCoroutine);
        }
        runningDelayCoroutine = StartCoroutine(CoolTimeCor(action, coolTime));
    }

    private IEnumerator CoolTimeCor(Action action, float coolTime)
    {
        yield return new WaitForSeconds(coolTime);
        action();
        runningDelayCoroutine = null;
    }

    #endregion
}
