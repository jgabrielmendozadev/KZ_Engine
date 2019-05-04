using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class PoolManager {

    static Dictionary<string, object> _pools = new Dictionary<string, object>();

    public static void CreatePool<T>(string poolName, Func<T> factory, int initialSize = 0) where T : IPooleable {
        if (_pools.ContainsKey(poolName))
            Debug.LogWarning("PoolManager already has a pool named \"" + poolName + "\"");
        else
            _pools[poolName] = new ObjectPool<T>(factory).InitializeObjs(initialSize);
    }

    public static T GetObject<T>(string poolName) where T : IPooleable {
        if (!_pools.ContainsKey(poolName))
            throw new Exception("ObjectPool for \"" + poolName + "\" not found");
        if (_pools[poolName] == null)
            throw new Exception("No objects in pool");
        
        ObjectPool<T> p = _pools[poolName] as ObjectPool<T>;
        if (p == null)
            throw new Exception("Wrong type of pool for \"" + poolName + "\"");

        return p.GetObject();
    }

    public static void DestroyPool<T>(string poolName) where T : IPooleable {
        ((ObjectPool<T>)_pools[poolName]).DestroyPool();
        _pools.Remove("name");
    }

    public static void DestroyAllPools() {
        _pools.Values.ToList().ForEach(p => {
            ObjectPool<IPooleable> pool = p as ObjectPool<IPooleable>;
            if (pool != null) pool.DestroyPool();
        });
    }
}