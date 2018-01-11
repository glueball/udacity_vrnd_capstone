using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBreakable : Breakable
{
  public int life = 3;
  private Animator anim;

  private readonly int hitHash = Animator.StringToHash("hit");

  public NotifyReceiver notifyTo;
  public string dieMsg;

  private void Start()
  {
    anim = GetComponent<Animator>();
  }

  public override void Break()
  {
    --life;

    if (life > 0)
    {
      anim.SetTrigger(hitHash);
    }
    else
    {
      anim.SetTrigger("die");
      notifyTo.Notify(dieMsg);
    }
  }
}