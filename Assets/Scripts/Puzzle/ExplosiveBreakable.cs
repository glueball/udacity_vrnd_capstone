using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBreakable : Breakable
{
  public float rad = 7f;

  private bool broken = false;

  public override void Break()
  {
    if (broken) return;
    broken = true;
    
    foreach (var c in Physics.OverlapSphere(transform.position, rad, -1, QueryTriggerInteraction.Collide ))
    {
      if (c.gameObject == gameObject) continue;

      var br = c.GetComponent<Breakable>();

      if (br != null) br.Break();
    }

    base.Break();
  }
}