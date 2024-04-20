using UnityEngine;

public class RoundObstacle : Obstacle
{
	public override void Rotate(float speed)
	{
		transform.rotation = Quaternion.Euler(0, speed, 0);
	}

	public override void SetDistance(float distance)
	{
		// nothing
	}

	private void OnCollisionEnter(Collision collision)
	{
		// GameManager.Instance.ChangeGameState(GameEventType.Over);
	}
}
