using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorShooter : MonoBehaviour
{
  private ObjectPool pool;

  public GameObject barrel;

  public float rocketSpeed = 20f;

  public float angle = 10f;

  void Start()
  {
    pool = GetComponent<ObjectPool>();
  }

  void Shoot()
  {
    var rocket = pool.Get();
    rocket.transform.position = barrel.transform.position + 0 * barrel.transform.forward;
    rocket.transform.rotation = barrel.transform.rotation;

    var rocketController = rocket.GetComponent<RocketController>();
    rocketController.SetPool(pool);

    rocketController.rb.velocity =
      (barrel.transform.forward * Mathf.Cos(angle) + barrel.transform.up * Mathf.Sin(angle)) *
      rocketSpeed;
    Debug.Log(rocketController.rb.velocity);
    rocketController.rb.angularVelocity = Vector3.zero;

    rocket.SetActive(true);
  }
}