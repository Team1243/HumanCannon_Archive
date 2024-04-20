using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Circle
{
	public float speed;
	public float distance;
	public Obstacle obstacle;
	public AnimationCurve curve;

	private float offsetTime;
	private float graphArea;
	private float rotateTime;

	public Circle(float speed, float distance, Obstacle obstacle, AnimationCurve curve)
	{
		this.speed = speed;
		this.distance = distance;
		this.obstacle = obstacle;
		this.curve = curve;

		offsetTime = Random.Range(0, 1f);
		rotateTime = offsetTime;
		rotateTime %= 1f;
		graphArea = MathfExtension.IntegrateCurve(curve, 0, 1f, 101);
		float angle = MathfExtension.IntegrateCurve(curve, 0, rotateTime, 101) / graphArea * 360;
		obstacle.Rotate(angle);
	}

	public void ApplyNewCurve(AnimationCurve curve)
	{
		graphArea = MathfExtension.IntegrateCurve(curve, 0, 1f, 101);
		this.curve = curve;
		float angle = MathfExtension.IntegrateCurve(curve, 0, rotateTime, 101) / graphArea * 360;
		obstacle.Rotate(angle);
	}

	public void Update()
	{
		rotateTime += Time.deltaTime * speed / 360f;
		rotateTime %= 1f;
		float angle = MathfExtension.IntegrateCurve(curve, 0, rotateTime, 101) / graphArea * 360;
		obstacle.Rotate(angle);
	}
}

public class CircleManager : LevelObject
{
	[Header("Setting Values")]
	[SerializeField] private int difficult;
    [SerializeField] private float minimumRange = 10f;
	[SerializeField] private float interval = 5f;

	[Header("References")]
	[SerializeField] private Transform position;
	private CurveMaker curveMaker;
	public CurveMaker CurveMaker => curveMaker;

	public override bool AnimationEnd { get; set; }
	private bool isDead = false;

	private List<Circle> obstacles = new List<Circle>();

	private void Awake()
	{
		curveMaker = GetComponent<CurveMaker>();
		CreateCircle();
	}

	private void Update()
	{
		if (!isDead)
		{
			for (int i = 0; i < obstacles.Count; ++i)
			{
				obstacles[i].Update();
			}
		}
	}

	public void CreateCircle()
	{
		ClearCircle();
		
		for (int i = 0; i < 3; ++i)
		{
			float distance = minimumRange + interval * i;
			Curve curve = curveMaker.CreateCubes(distance, position.position);
			Circle circle = new Circle(Random.Range(50f, 100f), distance, curve, CreateRandomSpeedCurve());
			obstacles.Add(circle);
		}
	}

	public void ClearCircle()
	{
		for (int i = 0; i < obstacles.Count; ++i)
		{
			Destroy(obstacles[i].obstacle.gameObject);
		}
		obstacles.Clear();
	}

	public AnimationCurve CreateRandomSpeedCurve()
	{
		AnimationCurve curve = new AnimationCurve();

		float height = Random.Range(0f, 1f);

		curve.AddKey(new Keyframe(0, height));
		curve.AddKey(new Keyframe(1f, height));

		int time = Random.Range(1, 4);


		for (int i = 0; i < time; ++i)
		{
			float x = Random.Range(0.1f, 0.9f);
			float y = Random.Range(0f, 1f);
			curve.AddKey(new Keyframe(x, y));
		}

		return curve;
	}

	/// <summary>
	/// ���̵� ���� ���� ��ġ�� ����ϴ� �Լ�. ���� ��ġ�� Curve Maker Ŭ������ ���� ��üȭ�Ѵ�.
	/// </summary>
	/// <param name="difficult"></param>
	private void DifficultCalculator(int difficult)
	{
		// circle = ���� ����. (���̵� / 50)�� �ø��� ���̴�. �ּ� 1 �ִ� 2.
		int circle = (int)Mathf.Ceil(difficult / 50f);
		circle = Mathf.Clamp(circle, 1, 2);

		// range = ��ֹ��� ����, ȣ�� ����Ͽ� ��������� ������ ��������. (���̵� + 30). �ּ� 30�� �ִ� 120��.
		int range = 30 + difficult;
		range = Mathf.Clamp(range, 30, 100);

		// parts = �� ���� ��ֹ��� �� ���� �κ����� ������ ���Ѵ�. 50�� ���� 1 �κо� �� ���� �� �ִ�. 1 ~ (���� / 50) �������� �������� ����. �ּ� 1 �ִ� 3.
		int[] parts = new int[circle];
		for (int i = 0; i < circle; ++i)
		{
			parts[i] = Random.Range(1, (range / 50) + 2);
		}

		// ���� range, parts�� CurveMaker Ŭ������ ������ ��ֹ��� ����������� �Ѵ�.
		curveMaker.SetCurve(range, parts);
	}

	#region Abstract Methods
	public override void CreateLevel(int difficult)
	{
		this.difficult = difficult;
		//curveMaker.HideCurve();
		isDead = true;
		for (int i = 0; i < obstacles.Count; ++i)
		{
			obstacles[i].ApplyNewCurve(CreateRandomSpeedCurve());
		}
	}

	// ����
	public override void Appear()
	{
		AnimationEnd = false;
		DifficultCalculator(difficult);
		curveMaker.ShowCircle(true, manager.levelData.appearAnimTime);
		StopAllCoroutines();
		StartCoroutine(AnimationEndCheckCo());
	}

	public override void Disappear()
	{
	}

	// Ŭ���� �ÿ� ��ֹ� �����
	public override void Clear()
	{
		isDead = true;
		AnimationEnd = false;
		curveMaker.ShowCircle(false, 0f);
		StopAllCoroutines();
		StartCoroutine(AnimationEndCheckCo());
	}

	private IEnumerator AnimationEndCheckCo()
	{
		while (true)
		{
			yield return null;
			if (curveMaker.AnimationEnd)
				break;
		}
		AnimationEnd = true;
		isDead = false;
	}

	public override void Over()
	{
		isDead = true;
		curveMaker.ShowCircle(false, 0f);
	}
	#endregion
}
