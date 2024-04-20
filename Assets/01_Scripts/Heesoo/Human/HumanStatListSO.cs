using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HumanStatsSO", menuName = "SO/HumanStatList")]
public class HumanStatListSO : ScriptableObject
{
    // [ArrayElementTitle("level")]
    public List<HumanStat> HumanStatList = new ();
    public float speedMul;
}
