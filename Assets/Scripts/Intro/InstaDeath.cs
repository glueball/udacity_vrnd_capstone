using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstaDeath : MonoBehaviour
{
  private Animator anim;

  public int death;

  void Start()
  {
    anim = GetComponent<Animator>();

    anim.SetInteger("death", death);
  }
}