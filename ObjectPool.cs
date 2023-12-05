using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public interface IPoolable
{
    bool active { get; set; }
    void OnEnableObject();
    void OnDisableObject();
}
//T means it can be anything, but we give it constraints so it at least has to be an implementation
//of the IPoolable interface
public class ObjectPool<T> where T : IPoolable
{
    private List<T> activePool = new List<T>();
    private List<T> inActivePool = new List<T>();

    public T RequestItem()
    {
        if (inActivePool.Count > 0)
        {
            //if there's items in the pool, fish the first on out
            return ActivateItem(inActivePool[0]);
        }

        //if there aren't items in the pool, create a new one.
        return ActivateItem(AddNewItemToPool());
    }

    public T ActivateItem(T _item)
    {
        _item.OnEnableObject();
        _item.active = true;

        //remove it from the inactive objects
        if (inActivePool.Contains(_item))
        {
            inActivePool.Remove(_item);
        }


        //add it to the active objects
        activePool.Add(_item);
        return _item;
    }

    public void ReturnObjectToPool(T _item)
    {
        //remove it from the active objects
        if (activePool.Contains(_item))
        {
            activePool.Remove(_item);
        }

        _item.OnDisableObject();
        _item.active = false;

        //add it to the inactive objects
        inActivePool.Add(_item);
    }

    private T AddNewItemToPool()
    {
        //I think we use an activator because I can't create a new instance of T
        //by using the "new" keyword. 
        T instance = (T)Activator.CreateInstance(typeof(T));
        inActivePool.Add(instance);
        return instance;
    }
}
