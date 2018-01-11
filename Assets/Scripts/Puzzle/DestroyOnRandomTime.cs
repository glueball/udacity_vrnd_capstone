using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnRandomTime : MonoBehaviour
{
  public float minTime = 5f;
  public float maxTime = 10f;

  private void OnEnable()
  {
    Destroy(gameObject, Random.Range(minTime, maxTime));
  }
}