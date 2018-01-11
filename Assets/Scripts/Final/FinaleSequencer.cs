using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class FinaleSequencer : NotifyReceiver
{
  public RabidNavigator navigator;
  public GameObject player;

  public DoorOpener scapeDoor;
  public DoorOpener venganceDoor;

  public int doubtTime = 1;
  public Transform p0;
  public Transform p1;
  public Transform p2;
  public Transform p3;

  public GameObject[] decisionEnablers;
  public string goodEndingLevelName = "Final";


  private GameObject rabid;
  private bool lookToPlayer;
  private bool readyForDecision;

  public GameObject escapeTeleport;
  public GameObject escapeTeleportMatSwitch;
  public Transform escapeWaitPoint;
  private bool escapeWaitIn;
  public GameObject escapeWaver;
  public PlayableDirector escapeSequence;

  public GameObject warrior;
  public Transform vengancePosition;
  public GameObject warriorController;
  public GameObject killRabid;
  public GameObject rabidCelebrate;

  private void Start()
  {
    rabid = navigator.gameObject;
    Invoke("GoToP0", 3);
  }

  private void GoToP0()
  {
    navigator.SetDestination(p0.position, GoToP1);
  }

  private void GoToP1()
  {
    navigator.SetDestination(p1.position, p1.forward, () => Invoke("GoToP2", doubtTime));
  }

  private void GoToP2()
  {
    navigator.SetDestination(p2.position, p2.forward, () => Invoke("GoToP3", doubtTime));
  }

  private void GoToP3()
  {
    navigator.SetDestination(p3.position, () =>
    {
      lookToPlayer = true;
      readyForDecision = true;
      LookPlayer();

      foreach (var decisionEnabler in decisionEnablers)
      {
        decisionEnabler.SetActive(true);
      }
    });
  }

  private void LookPlayer(RabidNavigator.DoneDelegate done = null)
  {
    if (lookToPlayer)
      navigator.SetOrientation(player.transform.position - rabid.transform.position, done);
  }

  public override void Notify(string msg)
  {
    switch (msg)
    {
      case "locomotion":
        LookPlayer();
        break;

      case "scape":
        if (readyForDecision) StartCoroutine(DecidedScape());
        break;

      case "scape wait in":
        escapeWaitIn = true;
        break;

      case "scape wait out":
        escapeWaitIn = false;
        break;

      case "vengance":
        if (readyForDecision) StartCoroutine(DecidedVengance());
        break;

      case "warriorDead":
        StartCoroutine(VenganceWin());
        break;

      case "rabidDead":
        StartCoroutine(VenganceDead());
        break;

      default:
        Debug.Log("Unhandled notify: " + msg);
        break;
    }
  }

  private IEnumerator VenganceDead()
  {
    warriorController.GetComponent<WarriorController>().StopFigth();
    rabid.GetComponent<RabidTrigger>().StopAiming();

    yield return new WaitForSeconds(2f);

    killRabid.SetActive(true);

    yield return new WaitForSeconds(7f);

    SteamVR_LoadLevel.Begin(SceneManager.GetActiveScene().name, false, 3, 1f);
  }

  private IEnumerator VenganceWin()
  {
    warriorController.GetComponent<WarriorController>().StopFigth();
    rabid.GetComponent<RabidTrigger>().StopAiming();

    foreach (var c in warrior.GetComponentsInChildren<Collider>())
    {
      c.enabled = false;
    }

    warrior.GetComponent<NavMeshAgent>().enabled = false;
    warrior.GetComponent<RabidNavigator>().enabled = false;

    yield return new WaitForSeconds(3);

    navigator.SetDestination(warrior.transform.position + 5 * (rabid.transform.position - warrior.transform.position).normalized,
                             () => Invoke("VenganceWin2", 3));
  }

  private void VenganceWin2()
  {
    lookToPlayer = true;
    LookPlayer(() =>
    {
      lookToPlayer = false;
      navigator.enabled = false;
      rabid.GetComponent<Animator>().enabled = false;
      rabidCelebrate.SetActive(true);
    });
    Invoke("GoodEnding", 7);
  }

  private IEnumerator DecidedVengance()
  {
    readyForDecision = false;
    scapeDoor.SetLocked(true);

    yield return new WaitForSeconds(1f);
    lookToPlayer = false;
    yield return new WaitForSeconds(1f);

    vengancePosition.gameObject.SetActive(true);
    navigator.SetDestination(vengancePosition.transform.position, vengancePosition.transform.forward,
                             () =>
                             {
                               rabid.GetComponent<RabidTrigger>().StartAiming();
                               warriorController.SetActive(true);
                             });
  }

  private IEnumerator DecidedScape()
  {
    readyForDecision = false;
    venganceDoor.SetLocked(true);

    yield return new WaitForSeconds(1f);
    lookToPlayer = false;
    yield return new WaitForSeconds(1f);

    navigator.SetDestination(escapeWaitPoint.position, () =>
    {
      lookToPlayer = true;
      LookPlayer();
      StartCoroutine(ScapeInit());
    });
  }

  private IEnumerator ScapeInit()
  {
    int n = 0;
    while (true)
    {
      if (escapeWaitIn) ++n;
      else n = 0;

      if (n > 5)
      {
        StartCoroutine(EscapeDo());
        break;
      }

      yield return new WaitForSeconds(1f);
    }
  }

  private IEnumerator EscapeDo()
  {
    lookToPlayer = false;
    navigator.enabled = false;
    rabid.GetComponent<Animator>().enabled = false;
    escapeWaver.SetActive(true);

    yield return new WaitForSeconds(3f);

    escapeTeleport.SetActive(true);

    yield return new WaitForSeconds(1.05f);

    escapeTeleportMatSwitch.SetActive(true);

    yield return new WaitForSeconds(1.05f);

    rabid.SetActive(false);
    escapeWaver.SetActive(false);

    yield return new WaitForSeconds(.5f);

    escapeTeleport.SetActive(false);

    yield return new WaitForSeconds(2f);

    escapeSequence.Play();

    Invoke("GoodEnding", 15f);
  }


  private void GoodEnding()
  {
    SteamVR_LoadLevel.Begin(goodEndingLevelName, false, 3, 0, 1f);
  }
}