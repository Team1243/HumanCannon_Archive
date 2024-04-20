using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Curve : RoundObstacle
{
	[HideInInspector]
	public List<Transform> cubes = new List<Transform>();
	[HideInInspector]
	public float cubeAngle;
	[HideInInspector]
	public float radius;
}

public class CurveMaker : MonoBehaviour
{
	#region variable value

	[Header("References")]
    [SerializeField] private Transform cube;
	[SerializeField] private Material def;
	[SerializeField] private Material other;

	public List<Curve> curves = new List<Curve>();

	private Coroutine animationCo = null;
	private Vector3 originPos;

	public bool AnimationEnd { get; private set; }

	#endregion

	#region Create & Setting Visible
	/// <summary>
	/// CircleManager의 난이도 계산 함수에서 계산된 수치를 실체화하는 함수.
	/// </summary>
	/// <param name="range">장애물의 총 길이</param>
	/// <param name="parts">장애물을 분리할 개수</param>
	public void SetCurve(int range, int[] parts)
	{
		int calcRange = range / 36; // range를 36으로 나눈다. 최소 길이가 호의 36도 이상이 되게 하기 위함.
		int restRange = range - calcRange * 36;  // calcRange 결과의 나머지. 남는 값은 버리지 않고 나중에 더해주어 길이를 맞춘다.

		// 고리의 개수만큼 반복하여 모든 고리를 랜덤한 길이, 간격으로 분할한다
		for (int i = 0; i < parts.Length; ++i)
		{
			int calcRangeLeft = calcRange;
			int[] partSize = new int[parts[i]]; // 고리를 n개로 나눌때 n개의 부분들의 비율을 랜덤으로 지정해 partSize 변수에 저장한다.
			for (int j = 0; j < parts[i] - 1; ++j)
			{
				partSize[j] = Random.Range(1, calcRangeLeft - j - 1);
				calcRangeLeft -= partSize[j];
			}
			partSize[parts[i] - 1] = calcRangeLeft;

			int leftPart = 360 - range; // 남은 부분. 장애물과 장애물 사이 공백의 값을 정할 때 사용한다.
			int progress = 0; // 진행도. 장애물과 공백을 지정하며 360도 중에 몇 도까지 사용하였는가를 나타낸다.
			for (int j = 0; j < parts[i]; ++j)
			{
				int interval = Random.Range(30, leftPart - 30 * (parts[i] - j)); // 장애물과 쟁애물 사이의 공백의 범위
				leftPart -= interval;
				interval /= (int)curves[i].cubeAngle;
				progress += interval;

				int size = partSize[j] * 36 + (j == parts[i] - 1 ? restRange : 0); // 계산된 장애물의 길이를 다시 360도 비율로 되돌린다.
				int g = size / (int)curves[i].cubeAngle + progress; // 고리에 배치된 큐브 몇 개를 사용하여야 하는지 계산.
				//for (int k = 0; k < curves[i].cubes.Count; ++k)
				//{
				//	curves[i].cubes[k].GetComponent<MeshRenderer>().material = other;
				//}
				for (int k = progress; k < g; ++k)
				{
					curves[i].cubes[k].gameObject.SetActive(true);
					//curves[i].cubes[k].GetComponent<MeshRenderer>().material = def;
				}
				progress += g - progress;
			}
		}
	}


	public void HideCurve()
	{
		for (int i = 0; i < curves.Count; ++i)
		{
			foreach (Transform cube in curves[i].cubes)
			{
				cube.gameObject.SetActive(false);
			}
		}
	}

	/// <summary>
	/// 매개변수로 들어온 길이에 맞는 원을 만든다.
	/// </summary>
	/// <param name="radius">반지름</param>
	/// <param name="position">생성 위치</param>
	/// <returns></returns>
	public Curve CreateCubes(float radius, Vector3 position)
	{
		originPos = position;
		int cubeReq = CubeRequireByAngle(radius);
		float angle = (int)Mathf.Floor(AngleRequireByCubeSize(radius));

		Curve curve = new GameObject("Curve").AddComponent<Curve>();
		curve.transform.SetParent(transform);
		curve.transform.localPosition = new Vector3(0, -cube.localScale.y, 0);
		curve.cubeAngle = angle;
		curve.radius = radius;
		curves.Add(curve);

		Rigidbody rigid = curve.gameObject.AddComponent<Rigidbody>();
		rigid.isKinematic = true;

		for (int i = 0; i < cubeReq; ++i)
		{
			float calcAngle = angle * i;
			float rad = calcAngle * Mathf.Deg2Rad;
			float x = radius * Mathf.Cos(rad);
			float z = radius * Mathf.Sin(rad);
			Transform inst = Instantiate(cube, new Vector3(position.x + z, -cube.localScale.y, position.z + -x), Quaternion.Euler(0, -calcAngle, 0), curve.gameObject.transform);
			inst.gameObject.SetActive(false);
			curve.cubes.Add(inst);
		}

		return curve;
	}
	#endregion

	#region Animation

	public void ShowCircle(bool show, float time = 1f)
	{
		AnimationEnd = false;
		if (animationCo != null)
			StopCoroutine(animationCo);

		animationCo = StartCoroutine(ShowCircleCo(show, time));
	}

	private IEnumerator ShowCircleCo(bool show, float time)
	{
		float passed = 0, t;
		float targetY = originPos.y + (show ? cube.localScale.y * 0.5f : -cube.localScale.y);
		float originY = originPos.y + (!show ? cube.localScale.y * 0.5f : -cube.localScale.y);

		while (passed < time)
		{
			t = passed / time;
			for (int i = 0; i < curves.Count; ++i)
			{
				curves[i].transform.localPosition = new Vector3(curves[i].transform.localPosition.x, Mathf.Lerp(originY, targetY, t), curves[i].transform.localPosition.z);
			}
			passed += Time.deltaTime;
			yield return null;
		}
		for (int i = 0; i < curves.Count; ++i)
		{
			curves[i].transform.localPosition = new Vector3(0, targetY, 0);
		}
		if (!show) HideCurve();
		AnimationEnd = true;
	}

	#endregion

	#region Calculates
	public int CubeRequireByAngle(float radius)
	{
		int angleReq = (int)Mathf.Floor(AngleRequireByCubeSize(radius));
		int req = (int)Mathf.Ceil(360f / angleReq);
		return req;
	}

	public float AngleRequireByCubeSize(float radius)
	{
		float size = cube.localScale.x;
		float length = LengthOfArc(1f, radius);

		return (size / length);
	}

	public float LengthOfArc(float degree, float radius)
	{
		float length = (2 * Mathf.PI * radius) * (degree / 360);
		return length;
	}
	#endregion
}
