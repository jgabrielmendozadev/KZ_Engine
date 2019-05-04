using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectPool<T> where T : IPooleable {
    
    Func<T> _factory;
    Stack<PoolObject<T>> _objects;
    
    public ObjectPool(Func<T> factory) {
        _factory = factory;
    }

    public ObjectPool<T> InitializeObjs(int amount) {
        _objects = new Stack<PoolObject<T>>();
        for (int i = 0; i < amount; i++) CreateObj();
        return this;
    }

    void CreateObj() {
        _objects.Push(new PoolObject<T>(_factory(), this));
    }

    public T GetObject() {
        PoolObject<T> poolObj = null;
        while (poolObj == null || poolObj._object == null|| poolObj._object.IsThisNull()) {
            if (!_objects.Any()) CreateObj();
            poolObj = _objects.Pop();
        }
        return poolObj.Enable();
    }

    public void ReturnToPool(PoolObject<T> poolObj) {
        poolObj.Disable();
        _objects.Push(poolObj);
    }

    public void DestroyPool() {
        while (_objects.Any())
            _objects.Pop().DestroyPoolObject();
    }
}


[Serializable]
public class PoolObject<T> where T : IPooleable {
    public T _object { get; private set; }
    public PoolObject(T obj, ObjectPool<T> myPool) {
        _object = obj;
        Disable();
        _object.OnFinishedUsing += (p) => myPool.ReturnToPool(this);
    }

    public T Enable() {
        return (T)_object.Spawn();
    }
    public void Disable() { _object.Banish(); }
    public void DestroyPoolObject() { _object.DestroyPoolObject(); }
}



public interface IPooleable {
    IPooleable Spawn();
    void Banish();

    event Action<IPooleable> OnFinishedUsing;

    bool IsThisNull();

    void DestroyPoolObject();
    /* POSIBLE USES
    * -Not used anymore: Destroy(gameobject)
    * -Stop using this on pool:
    *        OnFinishedUsing = delegate { }
    *     OR:OnFinishedUsing = Destroy(gameobject)
    */
}


public class Pooleable<T> : IPooleable where T : UnityEngine.Object {

    #region IPOOLEABLE
    public event Action<IPooleable> OnFinishedUsing = delegate { };

    public void Banish() {
        OnBanish(this);
    }
    public IPooleable Spawn() {
        OnSpawn(this);
        return this;
    }
    public bool IsThisNull() { return this == null; }
    public void DestroyPoolObject() { UnityEngine.Object.Destroy(obj); }
    #endregion


    #region HANDLING
    public T obj { get; private set; }
    Action<Pooleable<T>> OnSpawn;
    Action<Pooleable<T>> OnBanish;

    /// <summary>Don't forget to call FinishedUsing()</summary>
    public Pooleable(T poolObject, Action<Pooleable<T>> onSpawn, Action<Pooleable<T>> onBanish) {
        obj = poolObject;
        OnSpawn = onSpawn;
        OnBanish = onBanish;
    }

    public void FinishedUsing() { OnFinishedUsing(this); }
    public T GetObject() { return obj; }
    #endregion
}