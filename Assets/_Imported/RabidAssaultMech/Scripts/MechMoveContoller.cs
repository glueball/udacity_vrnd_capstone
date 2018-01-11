using UnityEngine;
using System.Collections;

public enum MechAnimStates {
	Unknown,
	Idle,
	WalkForward,
	WalkBackward,
	JumpForward,
	TurnRight,
	TurnLeft,
	EnterAttackMode,
	AttackMode,
	ExitAttackMode,
	Death
}

public enum AnimationControlTypes {
	Idle, // 0
	Walk, // 1
	WalkBackward, // 2
	JumpForward, // 3
	TurnRight, // 4
	TurnLeft, // 5
	EnterAttackMode, // 6
	AttackMode, // 7
	ExitAttackMode, // 8
	Death // 9
}

public class MechMoveContoller : MonoBehaviour {

	public bool UseNoMovement = false;

	public bool MechAlive = true;
	public bool MechAttacking = false;
	public int MechHealth = 100;

	public MechAnimStates CurrentAnimState = MechAnimStates.Unknown;
	public AnimationControlTypes CurrentAnimControl = AnimationControlTypes.Idle;
	public bool ControlManually = false;
	public AnimationControlTypes ManualAnimControl = AnimationControlTypes.Idle;

	public BoxCollider MainGroundCollider;
	private MechWeaponController weaponController;

	private Transform myTransform;
	private Rigidbody myRigidbody;
	public Animator MechAnimator;
	private AnimatorStateInfo currentBaseState;

	public float RotationSpeed = 10f;
	public float ForwardMoveSpeed = 1f;

	public bool IsOnGround = false;
	public bool Jumping = false;
	private Vector3 jumpForcePosition;
	public float JumpingForce = 10;
	public float distToGround = 0;
	public Transform JumpFromTrans;

	// Use this for initialization
	void Start () {
		myTransform = gameObject.transform;
		myRigidbody = gameObject.GetComponent<Rigidbody>();
		weaponController = gameObject.GetComponent<MechWeaponController>();
		if (MainGroundCollider != null) {
			distToGround = MainGroundCollider.bounds.extents.y + 1;
		}
	}

	private bool IsGrounded() {
		return Physics.Raycast(MainGroundCollider.center, -Vector3.up, distToGround + 0.3f);
	}

	// Update is called once per frame
	void Update () {
		if (myRigidbody != null) {

			IsOnGround = IsGrounded();

			// Check for Grounded
			if (Jumping) {
				if (IsGrounded()) {
					CurrentAnimControl = AnimationControlTypes.Idle;
					Jumping = false;
				}
			}

			bool usingKeys = false;

			// Control Mech Speed
			if (CurrentAnimControl != AnimationControlTypes.JumpForward && CurrentAnimControl != AnimationControlTypes.Death
			    && CurrentAnimControl != AnimationControlTypes.EnterAttackMode && CurrentAnimControl != AnimationControlTypes.AttackMode) {

				if (Input.GetKey(KeyCode.W)) {
					CurrentAnimControl = AnimationControlTypes.Walk;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.S)) {
					CurrentAnimControl = AnimationControlTypes.WalkBackward;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.A)) {
					CurrentAnimControl = AnimationControlTypes.TurnLeft;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.D)) {
					CurrentAnimControl = AnimationControlTypes.TurnRight;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.Space)) {
					CurrentAnimControl = AnimationControlTypes.JumpForward;
					usingKeys = true;
				}				
				if (Input.GetKeyDown(KeyCode.F)) {
					CurrentAnimControl = AnimationControlTypes.EnterAttackMode;
					usingKeys = true;
				}
				if (Input.GetKey(KeyCode.Backspace)) {
					CurrentAnimControl = AnimationControlTypes.Death;
					MechAnimator.SetInteger("AnimationControl", (int)9);
					MechHealth = 0;
					usingKeys = true;
				}

				// Stop Movement
				if (!usingKeys) {
					CurrentAnimControl = AnimationControlTypes.Idle;
				}
			}
			if (CurrentAnimState == MechAnimStates.Death) {
				if (!MechAlive) {
					if (MechHealth == 0) {
						if (Input.GetKey(KeyCode.Backspace)) {
							MechHealth = 100;
							MechAlive = true;
							CurrentAnimControl = AnimationControlTypes.Idle;
						}
					}
				}
			}
			if (CurrentAnimControl == AnimationControlTypes.EnterAttackMode) {
				if (Input.GetKeyUp(KeyCode.F)) {
					CurrentAnimControl = AnimationControlTypes.ExitAttackMode;
					MechAnimator.SetBool("MechAttacking", (bool)false);
					MechAttacking = false;
				}
			}
			if (CurrentAnimControl == AnimationControlTypes.ExitAttackMode) {
				if (CurrentAnimState == MechAnimStates.Idle) {
					CurrentAnimControl = AnimationControlTypes.Idle;
				}
			}
			if (MechHealth > 0) {
				if (CurrentAnimState == MechAnimStates.Idle) {
					if (!MechAlive) {
						MechAlive = true;
					}
				}
			}
			if (ControlManually) {
				if (ManualAnimControl == AnimationControlTypes.Death) {
					MechHealth = 0;
				}
				MechAnimator.SetInteger("AnimationControl", (int)ManualAnimControl);
			}
			else {
				MechAnimator.SetInteger("AnimationControl", (int)CurrentAnimControl);
			}
			MechAnimator.SetInteger("MechHealth", (int)MechHealth);

			if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle01"))
			{
				CurrentAnimState = MechAnimStates.Idle;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("WalkCycle01"))
			{
				CurrentAnimState = MechAnimStates.WalkForward;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("WalkBackward01"))
			{
				CurrentAnimState = MechAnimStates.WalkBackward;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("JumpForward"))
			{
				CurrentAnimState = MechAnimStates.JumpForward;
				Jumping = true;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("TurnLeft"))
			{
				CurrentAnimState = MechAnimStates.TurnLeft;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("TurnRight"))
			{
				CurrentAnimState = MechAnimStates.TurnRight;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("EnterAttackMode"))
			{
				CurrentAnimState = MechAnimStates.EnterAttackMode;
				MechAnimator.SetBool("MechAttacking", (bool)true);
				MechAttacking = true;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("AttackMode"))
			{
				// Fire Weapons
				if (weaponController) {
					weaponController.TestFire = true;
				}
				CurrentAnimState = MechAnimStates.AttackMode;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("ExitAttackMode"))
			{
				CurrentAnimState = MechAnimStates.Idle;
				MechAnimator.SetBool("MechAttacking", (bool)false);
				MechAttacking = false;
			}
			else if(MechAnimator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
			{				
				MechAnimator.SetBool("MechAlive", (bool)false);
				MechAlive = false;
				CurrentAnimState = MechAnimStates.Death;
			}
		}
	}

	private void DoJumpForward() {
		if (myRigidbody != null) {
			jumpForcePosition = myTransform.position - (myTransform.forward * 3);
			jumpForcePosition.y = jumpForcePosition.y - 6;
			if (JumpFromTrans != null) {
				JumpFromTrans.position = jumpForcePosition;
			}
//			myRigidbody.AddExplosionForce(JumpingForce, jumpForcePosition, 40);
			Vector3 forceDirection = jumpForcePosition - myTransform.position;
			forceDirection *= JumpingForce;
			myRigidbody.AddForceAtPosition(forceDirection, jumpForcePosition, ForceMode.Impulse);
		}
	}

	void FixedUpdate() {
		if (!UseNoMovement) {
			if (myRigidbody != null) {
				if (CurrentAnimState == MechAnimStates.WalkForward) {
					Vector3 velocityForward = myTransform.forward * ForwardMoveSpeed;
					myRigidbody.velocity = velocityForward;
				}
				else if (CurrentAnimState == MechAnimStates.WalkBackward) {
					Vector3 velocityForward = myTransform.forward * -ForwardMoveSpeed;
					myRigidbody.velocity = velocityForward;
				}
				else if (CurrentAnimState == MechAnimStates.TurnLeft) {
					Vector3 rotationVelocity = new Vector3(0, -(RotationSpeed * Time.deltaTime), 0);
					Quaternion deltaRotation = Quaternion.Euler(rotationVelocity * Time.deltaTime);
					myRigidbody.MoveRotation(myRigidbody.rotation * deltaRotation);
				}
				else if (CurrentAnimState == MechAnimStates.TurnRight) {
					Vector3 rotationVelocity = new Vector3(0, RotationSpeed * Time.deltaTime, 0);
					Quaternion deltaRotation = Quaternion.Euler(rotationVelocity * Time.deltaTime);
					myRigidbody.MoveRotation(myRigidbody.rotation * deltaRotation);
				}
				if (CurrentAnimState == MechAnimStates.JumpForward) {
					if (!Jumping) {
						DoJumpForward();
						Jumping = true;
					}
				}
			}
		}
	}
}
