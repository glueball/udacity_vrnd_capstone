using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : MonoBehaviour
{
  private RabidNavigator navigator;
  private Animator anim;
  private GameObject warrior;
  public Transform p0;
  public Transform[] points;
  private int index = 0;
  public GameObject rabid;
  private bool stopped;

  // Use this for initialization
  void Start()
  {
    navigator = GetComponentInParent<RabidNavigator>();
    anim = GetComponentInParent<Animator>();
    warrior = anim.gameObject;


    navigator.SetDestination(p0.position, GoToPoint);
  }

  private void GoToPoint()
  {
    if (stopped) return;

    navigator.SetDestination(points[index].position, () =>
    {
      if (stopped) return;
      navigator.SetOrientation(rabid.transform.position - warrior.transform.position, () =>
      {
        if (stopped) return;

        anim.SetTrigger("shoot");
        Invoke("GoToPoint", 4);
      });
    });
    index = (index + 1) % points.Length;
  }


  public void StopFigth()
  {
    stopped = true;
  }
}