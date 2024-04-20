using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpObjectManager : LevelObject
{
	private List<LevelUpObject> levelUpObjects = new List<LevelUpObject>();

	[SerializeField] private float hideHeight;
	private Vector3 originPos;
	private Coroutine animationCo = null;

	public override bool AnimationEnd { get; set; }

	private void Awake()
	{
		float timeOffset = 0;
		LevelUpObject[] levelUpObjectArr = GetComponentsInChildren<LevelUpObject>(true);
		for (int i = 0; i < levelUpObjectArr.Length; ++i)
		{
			timeOffset = i % 2 == 0 ? Random.Range(0f, 1f) : 1 - timeOffset;
			levelUpObjects.Add(levelUpObjectArr[i]);
			levelUpObjectArr[i].gameObject.SetActive(false);
			levelUpObjectArr[i].TimeOffset = timeOffset; 
		}
		originPos = transform.localPosition;
	}

	private void ActiveObjects(bool active)
	{
		foreach (LevelUpObject lo in levelUpObjects)
		{
			lo.gameObject.SetActive(active);
		}
	}

	private void Show(bool show, float time = 1f)
	{
		AnimationEnd = false;

		if (animationCo != null)
			StopCoroutine(animationCo);
		animationCo = StartCoroutine(ShowCo(show, time));
	}

	private IEnumerator ShowCo(bool show, float time)
	{
		if (show)
		{
			ActiveObjects(true);
		}
		else
		{
			foreach (LevelUpObject lo in levelUpObjects)
			{
				lo.isMove = false;
			}
		}

		float passed = 0, t;
		float targetY = originPos.y + (show ? 0 : -hideHeight);
		float originY = originPos.y + (!show ? 0 : -hideHeight);

		while (passed < time)
		{
			t = passed / time;
			transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(originY, targetY, t), transform.localPosition.z);
			passed += Time.deltaTime;
			yield return null;
		}
		transform.localPosition = new Vector3(0, targetY, 0);
		if (!show)
		{
			ActiveObjects(false);
		}
		else
		{
			foreach (LevelUpObject lo in levelUpObjects)
			{
				lo.isMove = true;
			}
		}
		AnimationEnd = true;
	}

	public override void Appear()
	{
	}

	public override void Clear()
	{

	}

	public override void CreateLevel(int difficult)
	{
		Show(true, 0f);
	}

	public override void Disappear()
	{
		Show(false, 1f);
	}

	public override void Over()
	{

	}
}
