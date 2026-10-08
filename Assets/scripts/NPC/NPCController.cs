using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 1.5f;

    [Header("Random Movement")]
    [SerializeField] private float roamRadius = 15f;
    [SerializeField] private float idleTime = 3f;

    private NavMeshAgent agent;
    private Animator animator;

    private float idleTimer;

    private int footpathAreaMask;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.speed = walkSpeed;

        int footpathArea = NavMesh.GetAreaFromName("Footpath");
        footpathAreaMask = 1 << footpathArea;
    }

    private void Start()
    {
        ChooseNewDestination();
    }

    private void Update()
    {
        UpdateAnimation();

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTime)
            {
                idleTimer = 0f;
                ChooseNewDestination();
            }
        }
    }

    private void ChooseNewDestination()
    {
        // First priority: Footpath
        if (TryFindDestination(footpathAreaMask))
        {
            return;
        }

        // Fallback: Any other walkable NavMesh area
        TryFindDestination(NavMesh.AllAreas);
    }

    private bool TryFindDestination(int areaMask)
    {
        Vector3 randomDirection =
            Random.insideUnitSphere * roamRadius;

        randomDirection += transform.position;

        if (NavMesh.SamplePosition(
            randomDirection,
            out NavMeshHit hit,
            roamRadius,
            areaMask))
        {
            agent.SetDestination(hit.position);
            return true;
        }

        return false;
    }

    private void UpdateAnimation()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
    }
}