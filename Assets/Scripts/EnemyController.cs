using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent agent;

    [Header("AI Control Parameters")]
    [SerializeField] int alertRadius;
    [SerializeField] int escapeRadius;
    [SerializeField] float visualDetectionAngle;
    [SerializeField] EnemyAIMovementType movementType = EnemyAIMovementType.Chase;
    [SerializeField] EnemyDetectionType detectionType = EnemyDetectionType.Radial;
    [SerializeField] List<Transform> patrolNodes = new List<Transform>();
    [SerializeField] private EnemyAIState currentAIState = EnemyAIState.Active;
    [SerializeField] bool canEscapeEnemy = true;
    [SerializeField] LayerMask targetLayers;

    private int currentPatrolIndex = 0;
    private Animator anim;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (!agent)
        {
            Debug.LogError("No nav mesh agent found on this enemy! Please place a nav mesh agent component on the enemy!");
        }

        anim = GetComponent<Animator>();
        if (!anim)
        {
            Debug.Log("No animator detected for enemy controller... skipping animations.");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentAIState = EnemyAIState.Patrol;

        // Set initial state
        SetState();
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
        SetAnimationState();
    }

    private void SetState()
    {
        switch (currentAIState)
        {
            case EnemyAIState.Active:
                SetActive();
                break;
            case EnemyAIState.Idle:
                SetIdle();
                break;
            case EnemyAIState.Patrol:
                SetPatrol();
                break;
        }
    }

    private void CheckState()
    {
        switch (currentAIState)
        {
            case EnemyAIState.Active:
                PerformActive();
                break;
            case EnemyAIState.Idle:
                SetIdle();
                break;
            case EnemyAIState.Patrol:
                PerformPatrol();
                break;
        }
    }

    private void PerformActive()
    {
        switch (movementType)
        {
            case EnemyAIMovementType.Chase:
                agent.destination = target.position;
                break;
            case EnemyAIMovementType.Avoid:
                agent.destination = GetAvoidDestination();
                break;
        }

        // If player escapes return to patrol
        if (canEscapeEnemy && IsTargetEscaped())
        {
            SetPatrol();
        }
    }

    private void SetActive()
    {
        Debug.Log("Switching to Active state...");
        currentAIState = EnemyAIState.Active;
    }

    private Vector3 GetAvoidDestination()
    {
        // Opposite heading of target multiplied by the magnitude of the alertRadius (try to escape alert radius)
        if (agent.remainingDistance <= alertRadius)
        {
            Vector3 heading = (transform.position - target.position).normalized;
            return (heading * alertRadius * 1.5f);
        }
        else
        {
            return agent.destination;
        }
    }

    private void SetIdle()
    {
        Debug.Log("Switching to Idle state...");
        agent.ResetPath();
        agent.isStopped = true;
    }

    private void PerformPatrol()
    {
        if (patrolNodes.Count > 0)
        {
            if ((agent.remainingDistance - agent.stoppingDistance) <= 0)
            {
                currentPatrolIndex++;
                if (currentPatrolIndex >= patrolNodes.Count)
                    currentPatrolIndex = 0;

                agent.destination = patrolNodes[currentPatrolIndex].position;
            }
            else if (agent.destination == null)
            {
                agent.destination = patrolNodes[currentPatrolIndex].position;
            }
        }

        // If we detect player
        if (InPlayerRange())
        {
            Debug.Log("Detected Player!");

            SetActive();
        }
    }

    private void SetPatrol()
    {
        Debug.Log("Switching to Patrol state...");
        currentAIState = EnemyAIState.Patrol;

        agent.destination = patrolNodes[currentPatrolIndex].position;
    }

    private void SetAnimationState()
    {
        if (anim)
        {
            // TODO: Animator
        }
    }

    private bool InPlayerRange()
    {
        bool inRange = false;
        switch (detectionType)
        {
            case EnemyDetectionType.Radial:
                inRange = RadiusCheck();
                break;
            case EnemyDetectionType.Visual:
                inRange = VisualCheck();
                break;
        }

        return inRange;
    }

    private bool IsTargetEscaped()
    {
        return (Vector3.Distance(transform.position, target.position) <= escapeRadius);
    }

    private bool RadiusCheck()
    {
        return (Vector3.Distance(transform.position, target.position) <= alertRadius);
    }

    private bool VisualCheck()
    {
        bool inRange = false;

        Vector3 heading = (agent.destination - transform.position).normalized;
        float dotProduct = Vector3.Dot(heading, transform.forward);
        if (dotProduct < Math.Cos(Mathf.Deg2Rad * visualDetectionAngle))
        {
            RaycastHit hit = new RaycastHit();
            Ray ray = new Ray(transform.position, heading);

            // Player is in range, check for distance
            if (Physics.Raycast(ray, out hit, alertRadius, targetLayers))
            {
                inRange = true;
            }

        }

        return inRange;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, alertRadius);
    }

    enum EnemyAIState
    {
        Idle,
        Active,
        Patrol
    }

    enum EnemyAIMovementType
    {
        Chase,
        Avoid
    }

    enum EnemyDetectionType
    {
        Radial,
        Visual
    }
}
