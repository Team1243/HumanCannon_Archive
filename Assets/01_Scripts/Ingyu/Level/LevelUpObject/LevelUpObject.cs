using UnityEngine;

public class LevelUpObject : MonoBehaviour
{
    [SerializeField] private float moveRange = 10f;
    [SerializeField] private float moveTime = 10f;

	public bool isMove = true;

	private float timeOffset;
	public float TimeOffset { 
		get => timeOffset;
		set {
			timeOffset = value;
			time += timeOffset;
		}
	}
	private float time = 0;
	private Vector3 originPos;

	private void Start()
	{
		originPos = transform.localPosition;
	}

	private void Update()
	{
		Move();
	}

	private void Move()
	{
		if (!isMove) return;

		time += Time.deltaTime / moveTime;
		time %= 1f;
		float x = Mathf.Sin(time * Mathf.PI * 2) * moveRange;
		transform.localPosition = new Vector3(originPos.x + x, originPos.y, originPos.z);
	}
}
