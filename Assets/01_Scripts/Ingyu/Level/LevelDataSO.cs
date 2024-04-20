

using UnityEngine;

[CreateAssetMenu(menuName = "SO/Level Data SO")]
public class LevelDataSO : ScriptableObject
{
    [Header("Animation Time")]
    public float appearAnimTime;
    public float disappearAnimTime;
    public float clearAnimTime;
    public float coinGainTime;

    [Header("Tower")]
    public Color towerDefaultColor;
    public Color[] towerFillColors;
    public int coinFillColorMax;
}
