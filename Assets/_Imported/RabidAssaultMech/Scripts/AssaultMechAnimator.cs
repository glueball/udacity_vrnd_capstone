using UnityEngine;
using System.Collections;

public enum AssaultMechAnimations {
	Idle, // 0
	Walk, // 1
	WalkBackward, // 2
	StrafeRight, // 3
	StrafeLeft, // 4
	TurnLeft, // 5
	TurnRight, // 6
	HitFromFront, // 7
	HitFromBack, // 8
	DieBackward, // 9
	DieForward, // 10
	Jump, // 11
	Attack // 12
}

public class AssaultMechAnimator : MonoBehaviour {

	public bool IsAlive = true;
	public bool StayDead = false;

	public bool UseNoMovement = false;

	public AssaultMechAnimations CurrentAnimation = AssaultMechAnimations.Idle;
	public AssaultMechAnimations DesiredAnimation = AssaultMechAnimations.Idle;
	public Animator MechAnimator;

	private Transform mechTransform;
	private Rigidbody mechRigidbody;

	public float ForwardMoveSpeed = 3f;
	public float StrafeMoveSpeed = 2f;
	public float RotationSpeed = 50f;

	private bool hit = false;
	private float hitTimerFreq = 0.1f;
	private float hitTimer = 0;

	// Use this for initialization
	void Start () {
		mechTransform = gameObject.transform;
		mechRigidbody = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		if (MechAnimator != null) {
			bool usingKeys = false;

			if (IsAlive) {
				if (Input.GetKey(KeyCode.W)) {
					DesiredAnimation = AssaultMechAnimations.Walk;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.S)) {
					DesiredAnimation = AssaultMechAnimations.WalkBackward;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.A)) {
					DesiredAnimation = AssaultMechAnimations.TurnLeft;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.D)) {
					DesiredAnimation = AssaultMechAnimations.TurnRight;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.Q)) {
					DesiredAnimation = AssaultMechAnimations.StrafeLeft;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.E)) {
					DesiredAnimation = AssaultMechAnimations.StrafeRight;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.H)) {
					DesiredAnimation = AssaultMechAnimations.HitFromFront;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.J)) {
					DesiredAnimation = AssaultMechAnimations.HitFromBack;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.Backspace)) {
					DesiredAnimation = AssaultMechAnimations.DieBackward;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.P)) {
					DesiredAnimation = AssaultMechAnimations.DieForward;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.Space)) {
					DesiredAnimation = AssaultMechAnimations.Jump;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.F)) {
					DesiredAnimation = AssaultMechAnimations.Attack;
					usingKeys = true;
				}
			
				if (hit) {
					if (hitTimer < hitTimerFreq) {
						hitTimer += Time.deltaTime;
					}
					else {
						hitTimer = 0;
						hit = false;
					}
				}
				else {
					if (!usingKeys) {
						DesiredAnimation = AssaultMechAnimations.Idle;
					}
				}
			}

			// Resurrect
			if (Input.GetKey(KeyCode.R)) {
				DesiredAnimation = AssaultMechAnimations.Idle;
				IsAlive = true;
				usingKeys = true;
			}

			MechAnimator.SetInteger("CurrentAnimation", (int)DesiredAnimation);

			if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
			{
				CurrentAnimation = AssaultMechAnimations.Idle;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
			{
				CurrentAnimation = AssaultMechAnimations.Walk;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("WalkBackward"))
			{
				CurrentAnimation = AssaultMechAnimations.WalkBackward;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("StrafeLeft"))
			{
				CurrentAnimation = AssaultMechAnimations.StrafeLeft;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("StrafeRight"))
			{
				CurrentAnimation = AssaultMechAnimations.StrafeRight;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("TurnLeft"))
			{
				CurrentAnimation = AssaultMechAnimations.TurnLeft;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("TurnRight"))
			{
				CurrentAnimation = AssaultMechAnimations.TurnRight;
			}			
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("HitFromFront"))
			{
				CurrentAnimation = AssaultMechAnimations.HitFromFront;
				hit = true;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("HitFromBack"))
			{
				CurrentAnimation = AssaultMechAnimations.HitFromBack;
				hit = true;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("DieBackward"))
			{
				CurrentAnimation = AssaultMechAnimations.DieBackward;
				if (DesiredAnimation == AssaultMechAnimations.DieBackward)
					IsAlive = false;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("DieForward"))
			{
				CurrentAnimation = AssaultMechAnimations.DieForward;
				if (DesiredAnimation == AssaultMechAnimations.DieForward)
					IsAlive = false;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
			{
				CurrentAnimation = AssaultMechAnimations.Jump;
			}

			MechAnimator.SetBool("IsAlive", IsAlive);
			MechAnimator.SetBool("Hit", hit);
		}
	}
	
	void FixedUpdate() {
		if (!UseNoMovement) {
			if (mechRigidbody != null) {
				if (CurrentAnimation == AssaultMechAnimations.Walk) {
					Vector3 velocityForward = mechTransform.forward * ForwardMoveSpeed;
					mechRigidbody.velocity = velocityForward;
				}
				else if (CurrentAnimation == AssaultMechAnimations.WalkBackward) {
					Vector3 velocityForward = mechTransform.forward * -ForwardMoveSpeed;
					mechRigidbody.velocity = velocityForward;
				}
				else if (CurrentAnimation == AssaultMechAnimations.StrafeLeft) {
					Vector3 velocityForward = mechTransform.right * -ForwardMoveSpeed;
					mechRigidbody.velocity = velocityForward;
				}
				else if (CurrentAnimation == AssaultMechAnimations.StrafeRight) {
					Vector3 velocityForward = mechTransform.right * ForwardMoveSpeed;
					mechRigidbody.velocity = velocityForward;
				}
				else if (CurrentAnimation == AssaultMechAnimations.TurnLeft) {
					Vector3 rotationVelocity = new Vector3(0, -(RotationSpeed * Time.deltaTime), 0);
					Quaternion deltaRotation = Quaternion.Euler(rotationVelocity * Time.deltaTime);
					mechRigidbody.MoveRotation(mechRigidbody.rotation * deltaRotation);
				}
				else if (CurrentAnimation == AssaultMechAnimations.TurnRight) {
					Vector3 rotationVelocity = new Vector3(0, RotationSpeed * Time.deltaTime, 0);
					Quaternion deltaRotation = Quaternion.Euler(rotationVelocity * Time.deltaTime);
					mechRigidbody.MoveRotation(mechRigidbody.rotation * deltaRotation);
				}
			}
		}
	}
}
