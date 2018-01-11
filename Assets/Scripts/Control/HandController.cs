using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
  private HandsInputController parent;
  private int index;

  private SteamVR_TrackedObject trackedObject;
  private SteamVR_Controller.Device device;

  private bool isLocomotionAiming;

  private bool isGrabbing;
  private GameObject grabbing;


  private readonly List<GameObject> grabAimList = new List<GameObject>();
  public float throwFactor = 1.5f;


  public void Setup(int i, HandsInputController p)
  {
    index = i;
    parent = p;
  }

  // Use this for initialization
  private void Start()
  {
    trackedObject = GetComponent<SteamVR_TrackedObject>();
  }

  // Update is called once per frame
  private void Update()
  {
    device = SteamVR_Controller.Input((int) trackedObject.index);

    if (!device.valid) return;

    // Can only initiate locomotion if had is not overlapping a throwable
    if (grabAimList.Count == 0 && device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
    {
      isLocomotionAiming = parent.CanLocomote(index);
    }

    if (isLocomotionAiming)
    {
      if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
      {
        isLocomotionAiming = false;

        if (!parent.LocomotionComplete(index) && grabAimList.Count > 0)
        {
          parent.GrabHighlight(index, grabAimList[0]);
        }
      }
      else
      {
        parent.UpdateLocomotion(index, transform.position, transform.forward);
      }
    }
    else if (!isGrabbing && device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
    {
      if (grabAimList.Count > 0)
      {
        grabbing = grabAimList[0];
        parent.Grab(index, grabbing);
      }
    }
    else if (isGrabbing && device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
    {
      parent.Release(grabbing);
      isGrabbing = false;

      var rb = grabbing.GetComponent<Rigidbody>();
      rb.isKinematic = false;
      rb.velocity = transform.parent.rotation * device.velocity * throwFactor; // Need to apply the parent rotation to get the velocity
      rb.angularVelocity = device.angularVelocity;

      grabbing.transform.parent = null;
    }
  }

  public void Grab(int i, GameObject grabAim)
  {
    if (i == index)
    {
      isGrabbing = true;
      grabbing = grabAim;

      grabbing.GetComponent<Rigidbody>().isKinematic = true;
      grabbing.transform.parent = transform;
    }
    else if (isGrabbing && grabbing == grabAim)
    {
      isGrabbing = false;
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (!other.CompareTag("Throwable")) return;

    bool empty = grabAimList.Count == 0;

    var enteredObject = other.attachedRigidbody.gameObject;

    if (!empty && grabAimList.Contains(enteredObject)) return;

    grabAimList.Add(enteredObject);

    if (empty && !isLocomotionAiming)
    {
      parent.GrabHighlight(index, grabAimList[0]);
    }
  }


  internal void OnTriggerExit(Collider other)
  {
    if (!other.CompareTag("Throwable")) return;

    var exitedObject = other.attachedRigidbody.gameObject;
    bool isFirst = grabAimList.Count > 0 && grabAimList[0] == exitedObject;

    grabAimList.Remove(exitedObject);

    if (isFirst)
    {
      parent.GrabUnHightlight(index, exitedObject);

      if (grabAimList.Count > 0)
      {
        parent.GrabHighlight(index, grabAimList[0]);
      }
    }
  }
}