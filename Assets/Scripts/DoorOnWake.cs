using UnityEngine;

public class DoorOnWake : MonoBehaviour
{
  public bool open;
  private readonly int openHash = Animator.StringToHash("open");

  void Awake()
  {
    transform.parent.gameObject.GetComponent<Animator>().SetBool(openHash, open);
  }
}