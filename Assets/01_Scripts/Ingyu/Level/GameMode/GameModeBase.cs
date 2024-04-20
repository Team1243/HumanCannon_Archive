using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameModeBase : MonoBehaviour
{
    protected List<LevelObject> levels;
    [HideInInspector]
    public LevelManager manager;

    protected bool animationEnd;
    protected Coroutine animationEndCo;

    public virtual void Init(LevelManager manager)
	{
        this.manager = manager;

        levels = GetComponentsInChildren<LevelObject>().ToList();
        foreach (LevelObject level in levels)
        {
            level.manager = manager;
        }
	}

	public abstract void Running();
    public abstract void Appear();
    public abstract void Disappear();
    public abstract void Ready();
    public abstract void Clear();
    public abstract void Over();
    public abstract void OnAnimationEnd();

    public abstract void CreateLevel(int stage);

    protected virtual void CheckAnimationEnd()
	{
        animationEnd = false;
        if (animationEndCo != null)
            StopCoroutine(animationEndCo);
        animationEndCo = StartCoroutine(CheckAnimationEndCo());
	}

    protected virtual IEnumerator CheckAnimationEndCo()
	{
        bool notEnd = true;
        while (notEnd)
        {
            notEnd = false;
            foreach (LevelObject level in levels)
            {
                if (!level.AnimationEnd)
                {
                    notEnd = true;
                    break;
                }
            }
            yield return null;
        }
        animationEnd = true;
        OnAnimationEnd();
    }
}
