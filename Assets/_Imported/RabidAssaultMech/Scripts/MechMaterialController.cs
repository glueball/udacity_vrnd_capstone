using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MechnaridMaterialTypes {
	SciFiOriginal,
	DesertMaterial,
	SnowCammoMaterial,
	GreenCammoMaterial,
	RedBlackMaterial
}

public class MechMaterialController : MonoBehaviour {

	public Material MechnaridBodyMaterial;
	public Material DesertBodyMaterial;
	public Material SnowCammoBodyMaterial;
	public Material GreenCammoBodyMaterial;
	public Material RedBlackBodyMaterial;

	private List<Renderer> BodyRenderers;

	public MechnaridMaterialTypes ActiveMaterialType = MechnaridMaterialTypes.SciFiOriginal;
	public MechnaridMaterialTypes DesiredMaterialType = MechnaridMaterialTypes.SciFiOriginal;

	// Use this for initialization
	void Start () {
		// Find all Leg and Body Renderers
		Renderer[] allRenderers = gameObject.GetComponentsInChildren<Renderer>();
//		Debug.Log(allRenderers.Length.ToString() + " Renderers Found Total.");

		// Initialize Lists
		BodyRenderers = new List<Renderer>();

		// Seperate Renderers
		if (allRenderers.Length > 0) {
			for (int i = 0; i < allRenderers.Length; i++) {
				if (allRenderers[i].sharedMaterial.name == MechnaridBodyMaterial.name) {
					BodyRenderers.Add(allRenderers[i]);
				}
			}
		}
		allRenderers = null;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyUp(KeyCode.Tab)) {
			if (DesiredMaterialType == MechnaridMaterialTypes.SciFiOriginal)
				DesiredMaterialType = MechnaridMaterialTypes.DesertMaterial;
			else if (DesiredMaterialType == MechnaridMaterialTypes.DesertMaterial)
				DesiredMaterialType = MechnaridMaterialTypes.SnowCammoMaterial;
			else if (DesiredMaterialType == MechnaridMaterialTypes.SnowCammoMaterial)
				DesiredMaterialType = MechnaridMaterialTypes.GreenCammoMaterial;
			else if (DesiredMaterialType == MechnaridMaterialTypes.GreenCammoMaterial)
				DesiredMaterialType = MechnaridMaterialTypes.RedBlackMaterial;
			else if (DesiredMaterialType == MechnaridMaterialTypes.RedBlackMaterial)
				DesiredMaterialType = MechnaridMaterialTypes.SciFiOriginal;
		}

		if (DesiredMaterialType != ActiveMaterialType) {
			UpdateMaterials();
		}
	}

	private void UpdateMaterials() {		
		ActiveMaterialType = DesiredMaterialType;

		if (ActiveMaterialType == MechnaridMaterialTypes.SciFiOriginal) {
			UpdateBodyMaterials(MechnaridBodyMaterial);
		}
		else if (ActiveMaterialType == MechnaridMaterialTypes.DesertMaterial) {
			UpdateBodyMaterials(DesertBodyMaterial);
		}
		else if (ActiveMaterialType == MechnaridMaterialTypes.SnowCammoMaterial) {
			UpdateBodyMaterials(SnowCammoBodyMaterial);
		}
		else if (ActiveMaterialType == MechnaridMaterialTypes.GreenCammoMaterial) {
			UpdateBodyMaterials(GreenCammoBodyMaterial);
		}
		else if (ActiveMaterialType == MechnaridMaterialTypes.RedBlackMaterial) {
			UpdateBodyMaterials(RedBlackBodyMaterial);
		}
	}

	private void UpdateBodyMaterials(Material materialToUse) {
		if (BodyRenderers.Count > 0) {
			for (int i = 0; i < BodyRenderers.Count; i++) {
				BodyRenderers[i].material = materialToUse;
			}
		}
	}

}