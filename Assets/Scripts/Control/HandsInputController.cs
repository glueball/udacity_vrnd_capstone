using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class HandsInputController : MonoBehaviour
{
  public HandController[] hands;

  public int locomotionMaxDistance = 20;
  public float locomotionMaxDeviation = 5f;
  
  private int locomotionIndex = -1;
  private bool locomotionValid;
  private Vector3 locomotionDestination;
  private LineRenderer locomotionLine;
  private RaycastHit hit;
  private NavMeshHit navMeshHit;

  public GameObject locomotionTargetIndicator;

  public Material highlightMaterial;
  private readonly List<Highlighted> highlighteds = new List<Highlighted>();

  public NotifyReceiver locomotionNotify;

  private struct Highlighted
  {
    public readonly GameObject gameObject;
    public readonly List<int> indexes;

    public Highlighted(GameObject gameObject)
    {
      indexes = new List<int>();
      this.gameObject = gameObject;
    }
  }

  // Use this for initialization
  void Awake()
  {
    locomotionTargetIndicator.SetActive(false);

    var i = 0;
    foreach (var hand in hands)
    {
      hand.Setup(i++, this);
    }

    locomotionLine = GetComponent<LineRenderer>();
  }

  public bool CanLocomote(int index)
  {
    if (index != locomotionIndex && locomotionIndex >= 0) return false;
    
    locomotionIndex = index;
    return true;
  }

  public void UpdateLocomotion(int index, Vector3 transformPosition, Vector3 transformForward)
  {
    Assert.AreEqual(index, locomotionIndex);

    locomotionValid = false;
    Vector3 finalPoint;

    if (Physics.Raycast(transformPosition, transformForward, out hit, locomotionMaxDistance))
    {
      finalPoint = hit.point;
      if (NavMesh.SamplePosition(hit.point, out navMeshHit, locomotionMaxDeviation, NavMesh.AllAreas))
      {
        locomotionValid = navMeshHit.position.y <= 0f;
        locomotionDestination = navMeshHit.position;
        locomotionTargetIndicator.transform.position = locomotionDestination;
      }
    }
    else
    {
      finalPoint = transformPosition + locomotionMaxDistance * transformForward;
    }

    locomotionTargetIndicator.SetActive(locomotionValid);

    var color = locomotionValid ? Color.green : Color.red;
    
    locomotionLine.SetPosition(0, transformPosition);
    locomotionLine.SetPosition(1, finalPoint);
    locomotionLine.startColor = color;
    locomotionLine.endColor = color;
    locomotionLine.enabled = true;
  }

  public bool LocomotionComplete(int index)
  {
    Assert.AreEqual(index, locomotionIndex);

    locomotionTargetIndicator.SetActive(false);
    locomotionLine.enabled = false;
    locomotionIndex = -1;

    if (!locomotionValid) return false;
    
    transform.position = locomotionDestination;
    
    if (locomotionNotify!=null) locomotionNotify.Notify("locomotion");
    
    return true;
  }

  public void GrabHighlight(int index, GameObject grabAim)
  {
    foreach (var h in highlighteds)
    {
      if (h.gameObject == grabAim)
      {
        if (!h.indexes.Contains(index)) h.indexes.Add(index);

        return;
      }
    }

    var highlighted = new Highlighted(grabAim);
    highlighted.indexes.Add(index);
    highlighteds.Add(highlighted);

    var highlighter = grabAim.GetComponent<Highlighter>();

    if (highlighter == null)
    {
      highlighter = grabAim.AddComponent<Highlighter>();
      highlighter.Init(highlightMaterial);
    }

    highlighter.Activate();
  }

  public void GrabUnHightlight(int index, GameObject exitedObject)
  {
    foreach (var h in highlighteds)
    {
      if (h.gameObject != exitedObject) continue;

      h.indexes.Remove(index);

      if (h.indexes.Count == 0)
      {
        exitedObject.GetComponent<Highlighter>().Deactivate();
        highlighteds.Remove(h);
      }

      return;
    }
  }

  public void Grab(int index, GameObject grabAim)
  {
    foreach (var h in hands)
    {
      h.Grab(index, grabAim);
    }

    var hr = grabAim.GetComponent<Highlighter>();
    if (hr != null) hr.SetGrabbed(true);
  }


  public void Release(GameObject grabAim)
  {
    var hr = grabAim.GetComponent<Highlighter>();
    if (hr != null) hr.SetGrabbed(false);
  }


  public void ForceThrowableRemove(GameObject throwable)
  {
    var col = throwable.GetComponentInChildren<Collider>();

    if (col == null) return;

    foreach (var h in hands) h.OnTriggerExit(col);
  }
}