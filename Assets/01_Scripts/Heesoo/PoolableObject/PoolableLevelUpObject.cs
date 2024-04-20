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
            // 색깔 파란색
            // 위로 향하는 파란색 아이콘 + 숫자 텍스트
        }
        else
        {
            // 색깔 빨간색
            // 해골 아이콘
        }
    }

    /*public override void Init()
    {
        
    }*/
}
