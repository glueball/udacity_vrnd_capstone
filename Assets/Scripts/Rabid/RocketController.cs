using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
  public GameObject explosionPrefab;

  private GameObject explosion = null;
  private ObjectPool pool;
  public Rigidbody rb;

  void Start()
  {
    rb = GetComponent<Rigidbody>();
  }
  
  public void SetPool(ObjectPool p)
  {
    pool = p;
  }

  private void OnCollisionEnter(Collision other)
  {
    var p = other.contacts[0].point;
    if (explosion == null)
    {
      explosion = Instantiate(explosionPrefab, p, Quaternion.identity);
      explosion.SetActive(true);
    }
    else
    {
      explosion.SetActive(false);
      explosion.transform.position = p;
      explosion.SetActive(true);
    }

    if (pool)
      pool.PoolObject(gameObject);
    else
      gameObject.SetActive(false);

    var breakable = other.gameObject.GetComponent<Breakable>();
    if (breakable != null) breakable.Break();
  }

  private void FixedUpdate()
  {
    transform.rotation = Quaternion.LookRotation(rb.velocity);
  }
}