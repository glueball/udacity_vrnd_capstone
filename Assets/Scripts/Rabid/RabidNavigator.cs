using System;
using UnityEngine;
using UnityEngine.AI;

public class RabidNavigator : MonoBehaviour
{
  private Animator anim;
  private NavMeshAgent agent;
  private Vector2 smoothDeltaPosition = Vector2.zero;
  private Vector2 velocity = Vector2.zero;
  private readonly int walkingHash = Animator.StringToHash("walking");
  private readonly int vxHash = Animator.StringToHash("vx");
  private readonly int vyHash = Animator.StringToHash("vy");

  private Vector3? targetFacing;

  public AudioSource leftFoot, rightFoot, leftFoot2, rightFoot2;

  public delegate void DoneDelegate();

  private DoneDelegate doneDelegate = null;

  private enum NavState
  {
    Idle,
    Moving,
    Rotating,
    Done
  }

  private NavState state = NavState.Idle;
  public float rotatingSpeed = 0.4f;

  // Use this for initialization
  void Awake()
  {
    agent = GetComponentInChildren<NavMeshAgent>();
    agent.updatePosition = false;

    anim = GetComponentInChildren<Animator>();
  }

  private void OnEnable()
  {
    agent.enabled = true;
  }


  private void Update()
  {
    switch (state)
    {
      case NavState.Idle:
        break;

      case NavState.Moving:
        Move();
        break;

      case NavState.Rotating:
        Rotate();
        break;

      case NavState.Done:
        anim.SetBool(walkingHash, false);
        state = NavState.Idle;

        if (doneDelegate != null)
        {
          var done = doneDelegate;
          doneDelegate = null;
          done();
        }

        break;

      default:
        throw new ArgumentOutOfRangeException();
    }
  }

  private void Rotate()
  {
    if (!targetFacing.HasValue)
    {
      state = NavState.Done;
      return;
    }

    var forward = transform.forward;

    if (Vector3.Angle(forward, targetFacing.Value) < 1e-3)
    {
      anim.SetBool(walkingHash, false);
      state = NavState.Done;
      transform.rotation = Quaternion.LookRotation(targetFacing.Value);
      return;
    }

    var cross = Vector3.Cross(forward, targetFacing.Value);

    anim.SetBool(walkingHash, true);
    anim.SetFloat(vxHash, 0);
    anim.SetFloat(vyHash, cross.y > 0 ? -3f : 3f);

    transform.rotation = Quaternion.RotateTowards(transform.rotation,
                                                  Quaternion.LookRotation(targetFacing.Value),
                                                  rotatingSpeed);
  }

  void Move()
  {
    Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

    // Map 'worldDeltaPosition' to local space
    float dx = Vector3.Dot(transform.forward, worldDeltaPosition);
    float dy = Vector3.Dot(transform.right, worldDeltaPosition);
    var deltaPosition = new Vector2(dx, dy);

    // Low-pass filter the deltaMove
    float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
    smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

    // Update velocity if time advances
    if (Time.deltaTime > 1e-5f)
      velocity = smoothDeltaPosition / Time.deltaTime;

    bool isFar = agent.remainingDistance > 1e-3;
    bool shouldMove = velocity.magnitude > 0.5f && isFar;

    // Update animation parameters
    anim.SetBool(walkingHash, shouldMove);
    anim.SetFloat(vxHash, velocity.x);
    anim.SetFloat(vyHash, velocity.y);
    if (worldDeltaPosition.magnitude > agent.radius / 3)
    {
      agent.nextPosition = transform.position + 0.9f * worldDeltaPosition;
    }

    if (!shouldMove && !isFar && !agent.pathPending)
    {
      state = targetFacing.HasValue ? NavState.Rotating : NavState.Done;
    }
  }

  public void SetDestination(Vector3 destination, DoneDelegate done = null)
  {
    agent.SetDestination(destination);
    targetFacing = null;
    state = NavState.Moving;
    doneDelegate = done;
  }

  public void SetDestination(Vector3 destination, Vector3 facing, DoneDelegate done = null)
  {
    SetDestination(destination, done);
    facing.y = 0;
    targetFacing = facing.normalized;
  }

  public void SetOrientation(Vector3 facing, DoneDelegate done = null)
  {
    facing.y = 0;
    targetFacing = facing.normalized;
    state = NavState.Rotating;
    doneDelegate = done;
  }

  private void OnAnimatorMove()
  {
    // Update position to agent position
    transform.position = agent.nextPosition;
  }

  public void RightFoot()
  {
    rightFoot.Play();
  }

  public void LeftFoot()
  {
    leftFoot.Play();
  }

  public void RightFoot2()
  {
    rightFoot2.Play();
  }

  public void LeftFoot2()
  {
    leftFoot2.Play();
  }
}