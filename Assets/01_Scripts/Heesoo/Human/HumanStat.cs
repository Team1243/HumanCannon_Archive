using UnityEngine;

[System.Serializable]
public class HumanStat 
{
    [Header("Stat")]
    [SerializeField] private int level;
    [SerializeField] private int hp;
    [SerializeField] private int attack;
    [SerializeField] private float moveSpeed;
    
    [Space]
    [Header("Appearance")]
    [ColorUsage(true)]
    [SerializeField] private Color skinColor;
    [SerializeField] private Vector3 bodyScale;

    #region Property

    // stat
    public int Level
    {
        get { return level; }
        private set { level = value; }
    }
    public int Hp
    {
        get { return hp; }
        private set { hp = value; }
    }

    public int Attack
    {
        get { return attack; }
        private set { attack = value; }
    }
    public float MoveSpeed
    {
        get { return moveSpeed; }
        private set {  moveSpeed = value; }
    }

    // appearance
    public Color SkinColor
    {
        get { return skinColor; }
        private set { skinColor = value; }
    }
    public Vector3 BodyScale
    {
        get { return bodyScale; }
        private set { bodyScale = value; }
    }

    #endregion
}
