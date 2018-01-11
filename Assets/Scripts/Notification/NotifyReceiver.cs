using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NotifyReceiver : MonoBehaviour
{
  public abstract void Notify(string msg);
}