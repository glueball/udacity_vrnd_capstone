using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InOutNotify : MonoBehaviour
{
  public NotifyReceiver notifyTo;
  public string inMsg;
  public string outMsg;

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player")) notifyTo.Notify(inMsg);
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.CompareTag("Player")) notifyTo.Notify(outMsg);
  }
}