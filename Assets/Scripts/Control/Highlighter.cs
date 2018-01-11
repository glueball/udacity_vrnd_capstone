using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class Highlighter : MonoBehaviour
{
  private GameObject replica;

  private bool grabbed;

  private bool active = true;

  public void Init(Material mat)
  {
    replica = new GameObject(gameObject.name + " - Replica");

    var mf = GetComponentInChildren<MeshFilter>();
    if (mf != null)
    {
      var rmf = CloneComponent(mf, replica) as MeshFilter;
      Debug.Assert(rmf != null, "rmf != null");
      rmf.mesh = mf.mesh;
    }

    var mr = GetComponentInChildren<MeshRenderer>();
    if (mr != null)
    {
      var rmr = CloneComponent(mr, replica) as MeshRenderer;
      Debug.Assert(rmr != null, "rmr != null");
      rmr.material = mat;
    }

    replica.transform.position = transform.position;
    replica.transform.rotation = transform.rotation;
//    replica.transform.localScale = transform.localScale; 
    replica.transform.parent = transform;
    replica.transform.localScale = Vector3.one; 
  }

  public void Activate()
  {
    active = true;
    replica.SetActive(active && !grabbed);
  }

  public void Deactivate()
  {
    active = false;
    replica.SetActive(false);
  }

  public void SetGrabbed(bool grab)
  {
    grabbed = grab;
    replica.SetActive(active && !grabbed);
  }


  private static Component CloneComponent(Component source, GameObject destination)
  {
    Component tmpComponent = destination.gameObject.AddComponent(source.GetType());

    foreach (var f in source.GetType().GetFields())
    {
      f.SetValue(tmpComponent, f.GetValue(source));
    }

    return tmpComponent;
  }
}