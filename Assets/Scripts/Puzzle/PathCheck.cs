using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PathCheck : MonoBehaviour
{
  public Transform target;
  public float checkDelay = 1f;
  public int requirePositives = 2;
  public NavMeshAgent agent;

  public NotifyReceiver notifyTo;
  public string notifyMsg;

  // Use this for initialization
  private void OnEnable()
  {
    StartCoroutine(Check());
  }

  private IEnumerator Check()
  {
    var positives = 0;

    while (true)
    {
      var path = new NavMeshPath();
      bool found = agent.CalculatePath(target.position, path);

      Debug.Log("Path: " + path.status);
      if (found && path.status == NavMeshPathStatus.PathComplete)
      {
        ++positives;

        if (positives >= requirePositives)
        {
          notifyTo.Notify(notifyMsg);
          break;
        }
      }
      else
      {
        positives = 0;
      }

      yield return new WaitForSeconds(checkDelay);
    }
  }
}