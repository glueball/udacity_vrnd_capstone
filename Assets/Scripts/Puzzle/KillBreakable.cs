using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBreakable : Breakable
{
  public AudioSource wilhelm;

  public override void Break()
  {
    wilhelm.Play();
    StartCoroutine(Reload());
  }

  private IEnumerator Reload()
  {
    yield return new WaitForSeconds(1);
    SteamVR_LoadLevel.Begin("Puzzle", false, 0.5f, 1);
  }
}