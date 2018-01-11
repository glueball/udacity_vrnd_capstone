using UnityEngine;

public class DoorOpener : MonoBehaviour
{
  public Color green;
  public Color red;

  private readonly int openParameter = Animator.StringToHash("open");
  private Animator anim;
  private Light[] lights;

  [SerializeField] private bool locked;
  [SerializeField] private bool forceOpen;
  private bool inPlayer;
  private bool inRabid;

  private bool isOpen;
  private AudioSource audioSource;

  // Use this for initialization
  void Start()
  {
    anim = GetComponent<Animator>();
    lights = GetComponentsInChildren<Light>();
    audioSource = GetComponent<AudioSource>();

    SetLocked(locked);
  }


  public void SetLocked(bool val)
  {
    locked = val;

    foreach (var l in lights)
    {
      l.color = locked ? red : green;
    }

    UpdateDoor();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      inPlayer = true;
      UpdateDoor();
    }

    if (other.CompareTag("Rabid"))
    {
      inRabid = true;
      UpdateDoor();
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      inPlayer = false;
      UpdateDoor();
    }

    if (other.CompareTag("Rabid"))
    {
      inRabid = false;
      UpdateDoor();
    }
  }


  private void UpdateDoor()
  {
    var newState = (inPlayer || inRabid || forceOpen) && !locked;

    if (newState == isOpen) return;

    isOpen = newState;
    anim.SetBool(openParameter, isOpen);
    audioSource.Play();
  }
}