using UnityEngine;

public abstract class LevelObject : MonoBehaviour
{
    [HideInInspector] public LevelManager manager;

    public abstract bool AnimationEnd{ get; set; }
    public abstract void CreateLevel(int difficult);
    public abstract void Clear();
    public abstract void Appear();
    public abstract void Disappear();
    public abstract void Over();
}
