using UnityEngine;

public class AutoLockDoor : MonoBehaviour
{
  public DoorOpener doorOpener;

  public GameObject[] disableObjects;

  public GameObject[] enableObjects;

  public NotifyReceiver notifyTo;

  public bool requireRabid;

  public string notifyMsg;

  private void OnTriggerStay(Collider other)
  {
    if (other.CompareTag("Player") && !requireRabid)
    {
      Debug.Log("Locking: " + gameObject.name);
      if (doorOpener != null) doorOpener.SetLocked(true);

      foreach (var o in disableObjects) o.SetActive(false);
      foreach (var p in enableObjects) p.SetActive(true);

      if (notifyTo != null) notifyTo.Notify(notifyMsg);

      gameObject.SetActive(false);
    }

    if (other.CompareTag("Rabid")) requireRabid = false;
  }
}