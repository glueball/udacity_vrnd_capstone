using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantGrab : MonoBehaviour
{
  public GameObject replica;


  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Hand")) replica.SetActive(true);
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.CompareTag("Hand")) replica.SetActive(false);
  }
}