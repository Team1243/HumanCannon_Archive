using UnityEngine;
using System;

public class BattleSystem : MonoSingleton<BattleSystem>
{
    private int chargingPoint = 100;
    public int ChargingPoint => chargingPoint;

    public Action<float> OnChargingPointChanged = null;

    [SerializeField] private HumanStatListSO humanStateListSO;
    [SerializeField] private Transform cannonTransform;

    public override void Init()
    {
        
    }

    public void PlusChargingPoint(int plusAmount)
    {
        chargingPoint = Mathf.Clamp(chargingPoint + plusAmount, 0, 100);
        OnChargingPointChanged?.Invoke(chargingPoint);
    }

    // 충전포인트 감소
    public void MinusChargingPoint(int minusAmount = 10, bool isGameOverCheck = true)
    {
        chargingPoint = Mathf.Clamp(chargingPoint - minusAmount, 0, 100);
        OnChargingPointChanged?.Invoke(chargingPoint);

        if (false == isGameOverCheck) return;

        if (chargingPoint <= 0)
        {
            GameManager.Instance.ChangeGameState(GameEventType.Over);
        }
    }

    // 전투 승패 여기서 검사하기
    // AttackHuman 생성 여기서 다 해주기
    #region Battle

    public void CreateBigAttackHuman()
    {
        int level = chargingPoint / 10;
        if (level <= 0)
        {
            level = 1;
        }

        MinusChargingPoint(chargingPoint, false);

        var stat = humanStateListSO.HumanStatList[level];
        Vector3 createPosition = cannonTransform.position + new Vector3(0, 0, 10);

        PoolableHuman attackHuman = PoolManager.Instance.Pop("BigAttackHuman_Player") as PoolableHuman;
        attackHuman.StatSetting(stat.Attack, stat.SkinColor);
        attackHuman.transform.localPosition = createPosition;
        attackHuman.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
    }

    #endregion
}
