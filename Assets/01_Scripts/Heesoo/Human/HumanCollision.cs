using UnityEngine;
using System;

public class HumanCollision : MonoBehaviour
{
    public Action OnCollisionWithTower;
    public Action OnCollisionWithObstacle;
    public Action<Vector3> OnEnterSafeArea;
    public Action<int> OnTriggerWithLevelUpObejct;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Tower")
        {
            OnCollisionWithTower?.Invoke();
        }
        if (collision.collider.CompareTag("Obstacle"))
        {
            OnCollisionWithObstacle?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {        
        if (other.tag == "SafeArea")
        {
            Vector3 targetPosition = other.transform.position;
            OnEnterSafeArea?.Invoke(targetPosition);
        }

        if (other.tag == "LevelUpObject")
        {
            if (other.TryGetComponent(out PoolableLevelUpObject levelupObject))
            {
                int levelUpAmount = levelupObject.LevelUpAmount;
                OnTriggerWithLevelUpObejct?.Invoke(levelUpAmount);
            }
        }
    }
}
