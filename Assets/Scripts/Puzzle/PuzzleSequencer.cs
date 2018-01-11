using System.Collections;
using UnityEngine;

public class PuzzleSequencer : NotifyReceiver
{
  public RabidNavigator navigator;
  public RabidTrigger trigger;

  public Transform targetBlock1A;
  public Transform targetBlock1B;
  public Transform targetBlock1C;
  public DoorOpener doorBlock1;
  public GameObject block1Help;
  private bool block1Player;
  private bool block1Rabid;

  public Transform targetBlock2A;
  public GameObject block2Checker;
  public GameObject block2Safety;

  public Transform targetBlock3A;
  public GameObject block3Safety;

  public Transform targetBlock4A;
  public GameObject block4Checker;
  public DoorOpener doorBlock4;

  public Transform targetFinal;


  // Use this for initialization
  void Start()
  {
    StartCoroutine(GoToBlock1A());
  }

  private IEnumerator GoToBlock1A()
  {
    yield return new WaitForSeconds(3);
    navigator.SetDestination(targetBlock1A.position, targetBlock1A.forward, () =>
    {
      block1Rabid = true;
      Block1Start();
    });
  }

  public override void Notify(string msg)
  {
    switch (msg)
    {
      case "block1":
        block1Player = true;
        Block1Start();
        break;

      case "block1open":
        doorBlock1.SetLocked(false);
        block1Help.SetActive(false);
        trigger.StopAiming(GoToBlock2A);
        break;

      case "block2open":
        trigger.StopAiming(GoToBlock3A);
        break;

      case "block3open":
        trigger.StopAiming(GoToBlock4A);
        break;

      case "block4open":
        trigger.StopAiming();
        doorBlock4.SetLocked(false);
        navigator.SetDestination(targetFinal.position);
        break;

      default:
        Debug.Log("Unhandled notify: " + msg);
        break;
    }
  }

  private void Block1Start()
  {
    if (block1Player && block1Rabid) StartCoroutine(Block1GetReady());
  }

  private IEnumerator Block1GetReady()
  {
    yield return new WaitForSeconds(2);

    navigator.SetDestination(targetBlock1B.position, targetBlock1B.forward, () =>
    {
      block1Help.SetActive(true);
      trigger.StartAiming();
    });
  }


  private void GoToBlock2A()
  {
    navigator.SetDestination(targetBlock1C.position, () =>
    {
      navigator.SetDestination(targetBlock2A.position, targetBlock2A.forward,
                               () =>
                               {
                                 trigger.StartAiming();
                                 block2Checker.SetActive(true);
                               });
    });
  }


  private void GoToBlock3A()
  {
    navigator.SetDestination(targetBlock3A.position, targetBlock3A.forward,
                             () =>
                             {
                               block2Safety.SetActive(false);
                               trigger.StartAiming();
                             });
  }

  private void GoToBlock4A()
  {
    block4Checker.SetActive(true);
    navigator.SetDestination(targetBlock4A.position, targetBlock4A.forward,
                             () =>
                             {
                               trigger.StartAiming();
                               block3Safety.SetActive(false);
                             });
  }
}