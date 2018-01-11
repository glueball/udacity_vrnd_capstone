using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayOnSpace : MonoBehaviour
{
  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      GetComponent<PlayableDirector>().Play();
      enabled = false;
    }
  }
}