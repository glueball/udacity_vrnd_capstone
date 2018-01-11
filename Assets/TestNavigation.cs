using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNavigation : MonoBehaviour
{
  public Transform target;

  // Use this for initialization
  void Start()
  {
    GetComponent<Animator>().SetBool("revive", true);
  }

  // Update is called once per frame
  void Update()
  {
    if (Input.GetKeyDown(KeyCode.A))
    {
      GetComponent<RabidNavigator>().SetDestination(target.position, target.forward);
    }
  }
}