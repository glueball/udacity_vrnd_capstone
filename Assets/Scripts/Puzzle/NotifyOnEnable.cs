using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotifyOnEnable : MonoBehaviour
{
  public string message;
  public NotifyReceiver notifyTo;

  private void OnEnable()
  {
    notifyTo.Notify(message);
  }
}