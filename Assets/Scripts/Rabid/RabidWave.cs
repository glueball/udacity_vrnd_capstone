using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabidWave : MonoBehaviour
{
  public GameObject arm;
  private float t0;
  public float period = 1f;
  public float amplitude = 5f;

  // Use this for initialization
  private void OnEnable()
  {
    t0 = Time.time;
  }

  // Update is called once per frame
  void Update()
  {
    arm.transform.localRotation = Quaternion.Euler(0, 0,
                                                   amplitude * Mathf.Sin(2 * Mathf.PI * (Time.time - t0) / period));
  }
}