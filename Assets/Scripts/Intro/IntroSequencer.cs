using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class IntroSequencer : MonoBehaviour
{
  public RabidNavigator rabidNavigator;

  public GameObject disableWhenRevived;

  public Transform projectorLocation;
  public GameObject projector;
  public VideoPlayer projectorPlayer;

  public DoorOpener exitDoor;

  public Transform exitLocation;

  public void RabidAlive()
  {
    disableWhenRevived.SetActive(false);
    StartCoroutine(MoveToProyector());
  }

  private IEnumerator MoveToProyector()
  {
    rabidNavigator.enabled = true;

    yield return new WaitForSeconds(1);

    rabidNavigator.SetDestination(projectorLocation.position);
    yield return new WaitForSeconds(3);
    rabidNavigator.SetDestination(projectorLocation.position, projectorLocation.right, () => StartCoroutine(StartVideo()));
  }

  private IEnumerator StartVideo()
  {
    projectorPlayer.loopPointReached += e => StartCoroutine(VideoPlayed());
    var projectorObject = projectorPlayer.gameObject;
    projectorObject.SetActive(false);
    yield return new WaitForSeconds(2);
    projector.SetActive(true);
    projectorObject.SetActive(true);
  }

  private IEnumerator VideoPlayed()
  {
    exitDoor.SetLocked(false);
    projectorPlayer.gameObject.SetActive(false);
    yield return new WaitForSeconds(2);
    projector.SetActive(false);
    yield return new WaitForSeconds(2);

    rabidNavigator.SetDestination(exitLocation.position, exitLocation.forward);
  }
}