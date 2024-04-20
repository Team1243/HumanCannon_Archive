using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    public abstract void SetDistance(float distance);
    public abstract void Rotate(float speed);
}
