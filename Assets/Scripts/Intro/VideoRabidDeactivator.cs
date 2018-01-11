using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoRabidDeactivator : MonoBehaviour
{
  public Material barrelMaterial;
  public Material restMaterial;

  public GameObject[] barrels;

  public GameObject baseObject;
  public bool useParentAsBase = true;


  private void Awake()
  {
    var realBase = baseObject == null ? gameObject : baseObject;
    realBase = useParentAsBase ? realBase.transform.parent.gameObject : realBase;

    foreach (var r in realBase.GetComponentsInChildren<Renderer>())
    {
      r.material = restMaterial;
    }

    foreach (var barrel in barrels)
    {
      barrel.GetComponent<Renderer>().material = barrelMaterial;
    }
  }
}