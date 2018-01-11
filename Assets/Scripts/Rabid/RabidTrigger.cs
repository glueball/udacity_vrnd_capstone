using System.Collections;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class RabidTrigger : MonoBehaviour
{
  public GameObject[] launchPoints;
  public GameObject rayCastOrigin;

  private LineRenderer laser;

  private bool aiming;
  private RabidNavigator navigator;
  private readonly Vector3[] limits = new Vector3[2];
  private int index;
  private Vector3 originalOrientation;

  private ObjectPool pool;
  public float rocketSpeed = 50f;

  private AudioSource launchSound;

  private KeywordRecognizer recognizer;

  private AudioSource shootAudio;
  public float shootUtterPeriod = 10f;
  private float nextShootUtter;

  // Use this for initialization
  void Start()
  {
    pool = GetComponent<ObjectPool>();
    laser = GetComponent<LineRenderer>();
    navigator = GetComponent<RabidNavigator>();
    launchSound = rayCastOrigin.GetComponent<AudioSource>();
    shootAudio = GetComponent<AudioSource>();

    recognizer = new KeywordRecognizer(new[] {"shoot"}, ConfidenceLevel.Low);
    recognizer.OnPhraseRecognized += args => Shoot();
    recognizer.Start();
  }


  private void OnDestroy()
  {
    recognizer.Stop();
    recognizer.Dispose();
  }

  public SteamVR_TrackedObject[] hands;
  private float lastShoot;
  public float triggerCooldown = 5f;

  private void Update()
  {
    if (!aiming) return;

    Debug.Log(lastShoot + triggerCooldown + " < " + Time.time);
    if (lastShoot + triggerCooldown < Time.time)
    {
      foreach (var hand in hands)
      {
        var triggerHandIndex = (int) hand.index;
        if (triggerHandIndex > 0
            && SteamVR_Controller.Input(triggerHandIndex).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
          lastShoot = Time.time;
          Shoot();
          break;
        }
      }
    }

    if (nextShootUtter < Time.time)
    {
      Debug.Log("Utter");
      shootAudio.Play();
      nextShootUtter = Time.time + shootUtterPeriod;
    }
  }

  public void StartAiming()
  {
    var forward = transform.forward;
    forward.y = 0;
    originalOrientation = forward;

    aiming = true;
    index = 0;
    laser.enabled = true;
    limits[0] = Quaternion.Euler(0, 45, 0) * forward;
    limits[1] = Quaternion.Euler(0, -45, 0) * forward;

    StartCoroutine(Aim());
  }

  public void StopAiming(RabidNavigator.DoneDelegate done = null)
  {
    aiming = false;
    navigator.SetOrientation(originalOrientation, done);
    laser.enabled = false;
  }

  private IEnumerator Aim(bool nextStep = true)
  {
    yield return new WaitForSeconds(0.5f);

    if (aiming)
    {
      navigator.SetOrientation(limits[index], () => StartCoroutine(Aim()));
      if (nextStep) index = (index + 1) % limits.Length;
    }
  }

  private RaycastHit hit;

  private void Shoot()
  {
    if (!aiming) return;
    launchSound.Play();
    nextShootUtter = Time.time + shootUtterPeriod;

    foreach (var barrel in launchPoints)
    {
      float angle = Mathf.PI / 4;
      if (Physics.Raycast(barrel.transform.position, barrel.transform.forward, out hit, 250))
      {
        var d = Vector3.Distance(rayCastOrigin.transform.position, hit.point);
        angle = 0.5f * Mathf.Asin(Physics.gravity.magnitude * d / rocketSpeed / rocketSpeed);
      }

      var rocket = pool.Get();
      rocket.transform.position = barrel.transform.position;
      rocket.transform.rotation = barrel.transform.rotation;

      var rocketController = rocket.GetComponent<RocketController>();
      rocketController.SetPool(pool);

      rocketController.rb.velocity =
        (barrel.transform.forward * Mathf.Cos(angle) + barrel.transform.up * Mathf.Sin(angle)) *
        rocketSpeed;
      rocketController.rb.angularVelocity = Vector3.zero;

      rocket.SetActive(true);
    }
  }
}