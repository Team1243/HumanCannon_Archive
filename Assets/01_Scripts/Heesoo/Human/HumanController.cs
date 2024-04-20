using UnityEngine;

public class HumanController : MonoBehaviour
{
    [SerializeField] private HumanStatListSO humanStatsSO;
    public HumanStat Stat { get; private set; }

    private int level;
    private Vector3 moveDir;
    private float moveSpeed;

    private Animator animator;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    private PoolableHuman poolableHuman;
    private HumanCollision humanCollision;
    private HumanMovement humanMovement;

    private void Awake()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        skinnedMeshRenderer = transform.GetChild(0).GetChild(0).GetComponent<SkinnedMeshRenderer>();

        poolableHuman = GetComponent<PoolableHuman>();
        humanCollision = GetComponent<HumanCollision>();
        humanMovement = GetComponent<HumanMovement>();
    }

    #region Setting

    private void LevelUp(int amount)
    {
        level += amount;

        // Stat을 Level1 Stat으로 설정합니다.
        Stat = humanStatsSO.HumanStatList[level];

        MovementSetting();
        AppearanceSetting();
    }

    // 움직임 설정
    private void MovementSetting(float multiplier = 1)
    {
        moveSpeed = Stat.MoveSpeed * multiplier;
        humanMovement.SetMoveSpeed(moveSpeed);
        humanMovement.SetDirection(moveDir.normalized);

        float moveSpeedMultiple = Mathf.Abs(moveSpeed / 60);
        animator.SetFloat("moveSpeed", moveSpeedMultiple);
    }

    // 외관 설정
    private void AppearanceSetting()
    {
        skinnedMeshRenderer.material.SetColor("_BaseColor", Stat.SkinColor);
        transform.localScale = Stat.BodyScale;
    }

    #endregion

    private void OnEnable()
    {
        // 수치 초기화
        moveDir = Vector3.forward;
        transform.localPosition = Vector3.zero;
        level = 0;

        LevelUp(1);

        // 타워와 충돌하였을 때는 타워를 공격하고 오브젝트를 삭제합니다. 
        humanCollision.OnCollisionWithTower += () =>
        {
            GameManager.Instance.AttackTower(Stat.Attack);
            PoolManager.Instance.Push(poolableHuman);
        };

        // 장애물과 충돌하였을 때는 오브젝트를 삭제하고, ChargingPoint - 1 
        humanCollision.OnCollisionWithObstacle += () =>
        {
            if(PoolManager.Instance.Push(poolableHuman))
            {
                BattleSystem.Instance.MinusChargingPoint();
            }
        };

        // 안전지역에 도달하였을 때는 속도를 0.5배하여 감속하고, 진로 방향 타워로 설정합니다.
        humanCollision.OnEnterSafeArea += (targetPos) =>
        {
            moveDir = targetPos - transform.position;
            MovementSetting(0.7f);
        };

        // 레벨업 오브젝트와 충돌하였을 때 레벨업 합니다.
        humanCollision.OnTriggerWithLevelUpObejct += (levelUpAmount) =>
        {
            // 레벨업 정도가 0 이상이면 정삭적으로 레벨업하고, 
            // 0 이하라면 오브젝트를 삭제합니다.
            if (levelUpAmount > 0)
            {
                LevelUp(levelUpAmount);
            }
            else
            {
                PoolManager.Instance.Push(poolableHuman);
            }
        };
    }

    private void OnDisable()
    {
        humanCollision.OnCollisionWithTower = null;
        humanCollision.OnEnterSafeArea = null;
        humanCollision.OnTriggerWithLevelUpObejct = null;
    }

}
