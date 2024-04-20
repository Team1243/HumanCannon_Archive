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
	/// CircleManager�� ���̵� ��� �Լ����� ���� ��ġ�� ��üȭ�ϴ� �Լ�.
	/// </summary>
	/// <param name="range">��ֹ��� �� ����</param>
	/// <param name="parts">��ֹ��� �и��� ����</param>
	public void SetCurve(int range, int[] parts)
	{
		int calcRange = range / 36; // range�� 36���� ������. �ּ� ���̰� ȣ�� 36�� �̻��� �ǰ� �ϱ� ����.
		int restRange = range - calcRange * 36;  // calcRange ����� ������. ���� ���� ������ �ʰ� ���߿� �����־� ���̸� �����.

		// ���� ������ŭ �ݺ��Ͽ� ��� ���� ������ ����, �������� �����Ѵ�
		for (int i = 0; i < parts.Length; ++i)
		{
			int calcRangeLeft = calcRange;
			int[] partSize = new int[parts[i]]; // ���� n���� ������ n���� �κе��� ������ �������� ������ partSize ������ �����Ѵ�.
			for (int j = 0; j < parts[i] - 1; ++j)
			{
				partSize[j] = Random.Range(1, calcRangeLeft - j - 1);
				calcRangeLeft -= partSize[j];
			}
			partSize[parts[i] - 1] = calcRangeLeft;

			int leftPart = 360 - range; // ���� �κ�. ��ֹ��� ��ֹ� ���� ������ ���� ���� �� ����Ѵ�.
			int progress = 0; // ���൵. ��ֹ��� ������ �����ϸ� 360�� �߿� �� ������ ����Ͽ��°��� ��Ÿ����.
			for (int j = 0; j < parts[i]; ++j)
			{
				int interval = Random.Range(30, leftPart - 30 * (parts[i] - j)); // ��ֹ��� ��ֹ� ������ ������ ����
				leftPart -= interval;
				interval /= (int)curves[i].cubeAngle;
				progress += interval;

				int size = partSize[j] * 36 + (j == parts[i] - 1 ? restRange : 0); // ���� ��ֹ��� ���̸� �ٽ� 360�� ������ �ǵ�����.
				int g = size / (int)curves[i].cubeAngle + progress; // ���� ��ġ�� ť�� �� ���� ����Ͽ��� �ϴ��� ���.
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
	/// �Ű������� ���� ���̿� �´� ���� �����.
	/// </summary>
	/// <param name="radius">������</param>
	/// <param name="position">���� ��ġ</param>
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
