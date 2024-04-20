using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool isQuitting = false;

    private static T instance = null;
    public static T Instance
    {
        get
        {
            if (isQuitting)
            {
                instance = null;
            }

            if (instance == null)
            {
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    Debug.LogError($"{typeof(T).Name} is not exist");
                }
                else
                {
                    isQuitting = false;
                }
            }

            return instance;
        }
    }

    public virtual void Init() { }

    private void OnDestroy()
    {
        isQuitting = true;
        instance = null;
    }
}