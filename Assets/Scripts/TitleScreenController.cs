using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenController : MonoBehaviour
{
  public SteamVR_TrackedObject[] hands;
  private bool launched;
  public string nextScene;


  private void Update()
  {
    foreach (var hand in hands)
    {
      if (!launched && SteamVR_Controller.Input((int) hand.index).GetPress(SteamVR_Controller.ButtonMask.Trigger))
      {
        launched = true;
        SteamVR_LoadLevel.Begin(nextScene, false, 3);
      }
    }
  }
}