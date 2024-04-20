using UnityEngine;

public class PoolableHuman : PoolableMono
{
    private AttackHumanController attackHumanController;

    private void Awake()
    {
        if(TryGetComponent(out AttackHumanController controller))
        {
            attackHumanController = controller;
        }
    }

    public override void Init()
    {
        attackHumanController?.Init();        
    }

    public void StatSetting(int attackDamage, Color bodyColor)
    {
        attackHumanController.BigHumanSetting(attackDamage, bodyColor);
    }

}
