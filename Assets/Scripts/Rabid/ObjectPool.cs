using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
  public GameObject prefab;

  private readonly Queue<GameObject> pool = new Queue<GameObject>();

  public GameObject Get()
  {
    if (pool.Count > 0)
    {
      return pool.Dequeue();
    }
    else
    {
      return Instantiate(prefab);
    }
  }

  public void PoolObject(GameObject obj)
  {
    obj.SetActive(false);
    pool.Enqueue(obj);
  }
}