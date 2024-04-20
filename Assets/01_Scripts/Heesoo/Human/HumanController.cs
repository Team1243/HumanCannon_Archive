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

        // Stat�� Level1 Stat���� �����մϴ�.
        Stat = humanStatsSO.HumanStatList[level];

        MovementSetting();
        AppearanceSetting();
    }

    // ������ ����
    private void MovementSetting(float multiplier = 1)
    {
        moveSpeed = Stat.MoveSpeed * multiplier;
        humanMovement.SetMoveSpeed(moveSpeed);
        humanMovement.SetDirection(moveDir.normalized);

        float moveSpeedMultiple = Mathf.Abs(moveSpeed / 60);
        animator.SetFloat("moveSpeed", moveSpeedMultiple);
    }

    // �ܰ� ����
    private void AppearanceSetting()
    {
        skinnedMeshRenderer.material.SetColor("_BaseColor", Stat.SkinColor);
        transform.localScale = Stat.BodyScale;
    }

    #endregion

    private void OnEnable()
    {
        // ��ġ �ʱ�ȭ
        moveDir = Vector3.forward;
        transform.localPosition = Vector3.zero;
        level = 0;

        LevelUp(1);

        // Ÿ���� �浹�Ͽ��� ���� Ÿ���� �����ϰ� ������Ʈ�� �����մϴ�. 
        humanCollision.OnCollisionWithTower += () =>
        {
            GameManager.Instance.AttackTower(Stat.Attack);
            PoolManager.Instance.Push(poolableHuman);
        };

        // ��ֹ��� �浹�Ͽ��� ���� ������Ʈ�� �����ϰ�, ChargingPoint - 1 
        humanCollision.OnCollisionWithObstacle += () =>
        {
            if(PoolManager.Instance.Push(poolableHuman))
            {
                BattleSystem.Instance.MinusChargingPoint();
            }
        };

        // ���������� �����Ͽ��� ���� �ӵ��� 0.5���Ͽ� �����ϰ�, ���� ���� Ÿ���� �����մϴ�.
        humanCollision.OnEnterSafeArea += (targetPos) =>
        {
            moveDir = targetPos - transform.position;
            MovementSetting(0.7f);
        };

        // ������ ������Ʈ�� �浹�Ͽ��� �� ������ �մϴ�.
        humanCollision.OnTriggerWithLevelUpObejct += (levelUpAmount) =>
        {
            // ������ ������ 0 �̻��̸� ���������� �������ϰ�, 
            // 0 ���϶�� ������Ʈ�� �����մϴ�.
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
