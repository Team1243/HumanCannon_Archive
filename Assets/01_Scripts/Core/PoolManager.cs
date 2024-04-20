using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    [SerializeField] private PoolingListSO poolingListSO;

    private Dictionary<string, Pool<PoolableMono>> pools = new ();

    public override void Init()
    {
        CreatePool();    
    }

    private void CreatePool()
    {
        var poolingList = poolingListSO.PoolList;
        foreach (var pair in poolingList)
        {
            var pool = new Pool<PoolableMono>(pair.Prefab, transform, pair.Count);
            pools.Add(pair.Prefab.Id, pool);
        }
    }

    public PoolableMono Pop(string Id)
    {
        if (false == pools.ContainsKey(Id))
        {
            Debug.LogError($"Prefab does not exist on pool : {Id}");
            return null;
        }

        PoolableMono item = pools[Id].Pop();
        return item;
    }

    public bool Push(PoolableMono prefab)
    {
        if (pools[prefab.Id].Contains(prefab)) return false;
        pools[prefab.Id].Push(prefab);
        return true;
    }
}