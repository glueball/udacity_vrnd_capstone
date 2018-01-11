using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwitcher : MonoBehaviour
{
  public Material material;
  public GameObject overrideGameObject;

  public bool recursive = true;

  // Use this for initialization
  private void Awake()
  {
    var baseObject = overrideGameObject == null ? gameObject : overrideGameObject;

    if (recursive)
    {
      var renderers = baseObject.GetComponentsInChildren<Renderer>();
      foreach (var r in renderers)
      {
        r.material = material;
      }
    }
    else
    {
      baseObject.GetComponent<Renderer>().material = material;
    }
  }
}