using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : PoolableMono
{
    private readonly Stack<T> pool = new ();
    private readonly T prefab;
    private readonly Transform parentTr;

    public Pool(T prefab, Transform parent, int count = 10)
    {
        this.prefab = prefab;
        this.parentTr = parent;

        for (int i = 0; i < count; i++)
        {
            T obj = GameObject.Instantiate(this.prefab, this.parentTr);
            obj.gameObject.name = obj.gameObject.name.Replace("(Clone)", "");
            obj.gameObject.SetActive(false);
            pool.Push(obj);
        }
    }

    public T Pop()
    {
        T obj;

        // �������� ������ �߰�
        if (pool.Count <= 0)
        {
            obj = GameObject.Instantiate(prefab, parentTr);
            obj.gameObject.name = obj.gameObject.name.Replace("(Clone)", "");
            obj.Init();
        }
        // ���������� ������
        else
        {
            obj = pool.Pop();
            obj.Init();
            obj.gameObject.SetActive(true);
        }

        return obj;
    }

    public void Push(T obj)
    {
        obj.transform.SetParent(parentTr);
        obj.gameObject.SetActive(false);
        pool.Push(obj);
    }

    public bool Contains(T obj)
	{
        return pool.Contains(obj);
	}
}