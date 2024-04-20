using UnityEngine;
using TMPro;

public class PoolableLevelUpObject : MonoBehaviour
{
    [SerializeField] private int levelUpAmount;
    public int LevelUpAmount => levelUpAmount;

    private TextMeshPro doorText;

    private void Awake()
    {
        doorText = transform.Find("Doors Renderers").Find("DoorText").GetComponent<TextMeshPro>();  
    }

    private void OnEnable()
    {
        doorText.text = "+" + levelUpAmount.ToString();     
    }

    public void Setting(int amount)
    {
        levelUpAmount = amount;
        if (levelUpAmount > 0)
        {
            // ���� �Ķ���
            // ���� ���ϴ� �Ķ��� ������ + ���� �ؽ�Ʈ
        }
        else
        {
            // ���� ������
            // �ذ� ������
        }
    }

    /*public override void Init()
    {
        
    }*/
}
