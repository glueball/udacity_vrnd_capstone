using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
  public GameObject complete;
  public GameObject fragments;

  public virtual void Break()
  {
    complete.SetActive(false);
    fragments.transform.parent = null;
    fragments.SetActive(true);
    
    Destroy(gameObject);
  }
}