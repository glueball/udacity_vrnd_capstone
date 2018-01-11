using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabidPartController : MonoBehaviour
{
  public GameObject collectible;
  public GameObject builtPiece;
  public GameObject outlineReplica;
  public GameObject sparks;

  public Material outlineMaterial;

  private bool collectibleInPlace;

  private Rigidbody collectibleRb;


  public HandsInputController handsController;
  private RabidReviveController controller;


  // Use this for initialization
  private void Start()
  {
    if (builtPiece != null) builtPiece.SetActive(false);
    outlineReplica.SetActive(false);

    collectibleRb = collectible.GetComponent<Rigidbody>();

    foreach (var r in outlineReplica.GetComponentsInChildren<Renderer>())
      r.material = outlineMaterial;
  }

  private void Update()
  {
    if (collectibleInPlace && !collectibleRb.isKinematic)
    {
      handsController.ForceThrowableRemove(collectible);

      collectible.SetActive(false);
      outlineReplica.SetActive(false);
      if (sparks != null) sparks.SetActive(false);

      if (builtPiece != null) builtPiece.SetActive(true);
      collectibleInPlace = false;

      controller.PartComplete();
      enabled = false;
    }
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject == collectible)
    {
      collectibleInPlace = true;
      outlineReplica.SetActive(true);
    }
  }

  private void OnTriggerExit(Collider other)
  {
    if (other.gameObject == collectible)
    {
      collectibleInPlace = false;
      outlineReplica.SetActive(false);
    }
  }

  public void SetReviveController(RabidReviveController rabidReviveController)
  {
    controller = rabidReviveController;
  }
}