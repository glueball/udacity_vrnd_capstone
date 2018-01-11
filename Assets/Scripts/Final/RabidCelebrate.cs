using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabidCelebrate : MonoBehaviour
{
  public GameObject[] cannons;
  private float angle;
  public float maxAngle = 40;
  public float increment = 1;

  // Update is called once per frame
  void Update()
  {
    angle = Mathf.Min(angle + increment, maxAngle);
    var quat = Quaternion.Euler(-angle, 0, 0);
    foreach (var cannon in cannons)
    {
      cannon.transform.localRotation = quat;
    }
  }
}