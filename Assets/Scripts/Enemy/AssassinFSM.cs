using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AssassinFSM : MonoBehaviour
{
    // inspector values
    [field: Header("Movement")]
    [field: SerializeField] public float WalkSpeed { get; private set; } = 3f;
    [field: SerializeField] public float RunSpeed { get; private set; } = 6f;
    [field: SerializeField] public float SneakSpeed { get; private set; } = 2f;

    [field: Header("Ranges")]
    [field: SerializeField] public float PatrolRadius { get; private set; } = 3f;
    [field: SerializeField] public float AlertRadius { get; private set; } = 3.5f;
    [field: SerializeField] public float AttackRange { get; private set; } = 1f;

    // states
    private State currentState;

    // default behaviour states
    public PatrolState Patrol { get; private set; }
    public StunnedState Stunned { get; private set; }
    public DeathState Death { get; private set; }

    // attack sequence states
    public AlertState Alert { get; private set; }
    public ProwlState Prowl { get; private set; }
    public AttackState Attack { get; private set; }
    public HideState Hide { get; private set; }
    public FleeState Flee { get; private set; }

    // push obstacle sequnce states
    public WaitState Wait { get; private set; }
    public PushState Push { get; private set; }

    // trap states
    public LayTrapState LayTrap { get; private set; }
    public TrapTriggeredState TrapTriggered { get; private set; }

    // movement AI
    public NavMeshAgent Agent { get; private set; }

    void Awake()
    {
        // instantiate a new instance of each state
        Patrol = new PatrolState(this);
        Stunned = new StunnedState(this);
        Death = new DeathState(this);
        Alert = new AlertState(this);
        Prowl = new ProwlState(this);
        Attack = new AttackState(this);
        Hide = new HideState(this);
        Flee = new FleeState(this);
        Wait = new WaitState(this);
        Push = new PushState(this);
        LayTrap = new LayTrapState(this);
        TrapTriggered = new TrapTriggeredState(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        // get nav agent
        Agent = GetComponent<NavMeshAgent>();

        // set default state
        currentState = Patrol;
        currentState?.OnEnter();
    }

    // Update is called once per frame
    void Update()
    {
        currentState?.OnUpdate();
    }

    // public methods
    public void SwitchState(State newState)
    {
        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
    }

    public bool PlayerNearby(float range, out Transform player)
    {
        // create a hit variable for output
        RaycastHit hit;
        // use sphere cast, and ensure hit object has the 'Player' tag
        if (Physics.SphereCast(transform.position, range, transform.forward, out hit) && hit.collider.CompareTag("Player"))
        {
            player = hit.transform;
            return true;
        }
        player = null;
        return false;
    }

    public bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        // get a random point in a sphere
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        // get the position on the random point on the navmesh
        if (NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }

    // gizmos
    void OnDrawGizmosSelected()
    {
        // show patrol radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, PatrolRadius);
        // show alert radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, AlertRadius);
        // show attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }
}
