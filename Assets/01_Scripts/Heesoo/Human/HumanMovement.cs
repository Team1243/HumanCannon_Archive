using UnityEngine;

public class HumanMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private Vector3 moveDirection;

    private void Update()
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.Self);
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetDirection(Vector3 dir)
    {
        moveDirection = dir;
        Quaternion.LookRotation(dir);
    }
}
