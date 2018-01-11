using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class RabidReviveController : MonoBehaviour
{
  private int nParts;
  private int nComplete;


  public Renderer[] barrels;

  public Material barrelInactive;

  public Material active;

  private bool transitioning;
  private bool currentActive;

  private Animator anim;
  public float maxDelay = 0.3f;
  public float minDelay = 0.1f;


  public AudioClip blipClip;
  public AudioClip activationClip;
  private AudioSource audioSource;

  public IntroSequencer sequencer;


  private void Start()
  {
    anim = GetComponent<Animator>();
    audioSource = GetComponent<AudioSource>();
  }


  // Use this for initialization
  void Awake()
  {
//    var parts = transform.parent.gameObject.GetComponentsInChildren<RabidPartController>();
    var parts = FindObjectsOfType(typeof(RabidPartController));
    nParts = parts.Length;

    foreach (var part in parts)
    {
      ((RabidPartController) part).SetReviveController(this);
    }
  }

  public void PartComplete()
  {
    ++nComplete;

    if (nComplete >= nParts)
    {
      StartCoroutine(Transition());
    }
  }

  private static void RandomizeArray(ref Renderer[] arr)
  {
    for (var i = arr.Length - 1; i > 0; i--)
    {
      var r = Random.Range(0, i);
      var tmp = arr[i];
      arr[i] = arr[r];
      arr[r] = tmp;
    }
  }


  private IEnumerator Transition()
  {
    var refTime = Time.time;
    var isActive = false;
    var r = barrels[0];

    audioSource.clip = blipClip;

    while (true)
    {
      yield return 0;

      float dt = Time.time - refTime;
      bool shouldActive = dt - 2.5f * Mathf.Sin(7 * dt) - 2.5 > 0;

      if (isActive && !shouldActive)
      {
        isActive = false;
        r.material = barrelInactive;
      }
      else if (!isActive && shouldActive)
      {
        isActive = true;
        r.material = active;
        audioSource.Play();
      }

      if (dt > 5f) break;
    }

    audioSource.clip = activationClip;
    audioSource.Play();

    foreach (var barrel in barrels)
    {
      barrel.material = active;

      yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
    }

    anim.SetBool("revive", true);

    var allRenderers = GetComponentsInChildren<Renderer>();
    RandomizeArray(ref allRenderers);

    foreach (var other in allRenderers)
    {
      other.material = active;

      yield return new WaitForSeconds(Random.Range(minDelay / 2, maxDelay / 2));
    }

    sequencer.RabidAlive();
    enabled = false;
  }
}