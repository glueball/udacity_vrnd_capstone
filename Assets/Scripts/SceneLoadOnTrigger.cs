using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadOnTrigger : MonoBehaviour
{
  public String sceneName;

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      SteamVR_LoadLevel.Begin(sceneName);
    }
  }
}
