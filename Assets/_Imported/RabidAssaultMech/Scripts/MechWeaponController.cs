using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MechWeaponController : MonoBehaviour {

	private Transform myTransform;

	public GameObject CannonWeaponPrefab;
	public ParticleSystem WeaponFiringParticles;
	public ParticleSystem LaserBoltParticles;

	public int NumberOfBarrels = 0;
	public Transform[] BarrelTransforms;
	public Transform[] BarrelFirePoints;
	public List<bool> barrelsFired;
	public List<Vector3> localCurrentPositions;
	public List<Vector3> localNormPositions;
	public List<Vector3> localFiredPositions;

	public float BarrelKickSpeed = 1f;
	private float ZBackMovement = -45f;

	public bool TestFire = false;
	public float LaserSpeed = 50f;
	public float LaserSize = 0.1f;
	public float LaserLifetime = 4f;
	public float FiringEffectSize = 2f;
	public float FiringEffectLife = 0.25f;
	private float FiringFreq = 0.5f;
	private float firingTimer = 0;
	public int currentBarrelNumber = 0;

	// Use this for initialization
	void Start () {
		myTransform = gameObject.transform;

		if (BarrelTransforms.Length > 0) {
			NumberOfBarrels = BarrelTransforms.Length;

			barrelsFired = new List<bool>();
			localCurrentPositions = new List<Vector3>();
			localNormPositions = new List<Vector3>();
			localFiredPositions = new List<Vector3>();

			// Setup Local Positions for Normal and Standard
			for (int i = 0; i < BarrelTransforms.Length; i++) {
				Vector3 newBarrelNormPosition = Vector3.zero;
				newBarrelNormPosition = BarrelTransforms[i].localPosition;
				localCurrentPositions.Add(newBarrelNormPosition);
				barrelsFired.Add(false);
				localNormPositions.Add(newBarrelNormPosition);
			}
			for (int i = 0; i < BarrelTransforms.Length; i++) {
				Vector3 newBarrelFiredPosition = Vector3.zero;
				newBarrelFiredPosition = BarrelTransforms[i].localPosition;
				newBarrelFiredPosition.z += ZBackMovement;
				localFiredPositions.Add(newBarrelFiredPosition);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (TestFire) {
			if (firingTimer < FiringFreq) {
				firingTimer += Time.deltaTime;
			}
			else {
				FireBarrel(currentBarrelNumber);
				// Move to Next Barrel
				// End Firing If Barrels All Finished Firing
				if (currentBarrelNumber + 1 == BarrelTransforms.Length) {
					currentBarrelNumber = 0;
					TestFire = false;
				}
				else {
					currentBarrelNumber++;
				}
				firingTimer = 0;
			}
		}

		// Handle Barrel Firing
		for (int i = 0; i < BarrelTransforms.Length; i++) {
			if (barrelsFired[i]) {
				// Barrel Is Firing
				if (Vector3.Distance(localCurrentPositions[i], localFiredPositions[i]) > 0.1f) {
					localCurrentPositions[i] = Vector3.Lerp(localCurrentPositions[i], localFiredPositions[i], BarrelKickSpeed * Time.deltaTime);
				}
				else {
					barrelsFired[i] = false;
				}
			}
			else {
				// Barrel Is Firing
				if (Vector3.Distance(localCurrentPositions[i], localNormPositions[i]) > 0.1f) {
					localCurrentPositions[i] = Vector3.Lerp(localCurrentPositions[i], localNormPositions[i], BarrelKickSpeed * Time.deltaTime);
				}
			}
		}

		// Update Barrel Positions
		for (int i = 0; i < BarrelTransforms.Length; i++) {
			BarrelTransforms[i].localPosition = localCurrentPositions[i];
		}
	}

	private void FireBarrel(int barrelNumber) {
		// Fire Cannon Barrel
		if (WeaponFiringParticles != null) {
			Vector3 firingEffectVelocity = Vector3.zero;
			WeaponFiringParticles.Emit(BarrelFirePoints[barrelNumber].position, firingEffectVelocity, FiringEffectSize, FiringEffectLife, Color.white);
		}
		if (CannonWeaponPrefab) {
			GameObject newLaserBolt = GameObject.Instantiate(CannonWeaponPrefab, BarrelFirePoints[barrelNumber].position, myTransform.rotation) as GameObject;
			newLaserBolt.name = "CannonLaserBolt";
		}

		barrelsFired[barrelNumber] = true;
	}


}
