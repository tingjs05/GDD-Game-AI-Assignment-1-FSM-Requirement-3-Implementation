using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    [field: SerializeField] public float FleeDistance { get; private set; } = 5f;
    [field: SerializeField] public float PlayerInObstacleRange { get; private set; } = 3f;

    [field: Header("Durations")]
    [field: SerializeField] public float AttackDuration { get; private set; } = 0.25f;
    [field: SerializeField] public float MaxFleeDuration { get; private set; } = 5f;
    [field: SerializeField] public float MinFaceEnemyDuration { get; private set; } = 0.5f;
    [field: SerializeField] public float MaxHideDuration { get; private set; } = 5f;
    [field: SerializeField] public float MaxWaitDuration { get; private set; } = 5f;
    [field: SerializeField] public float StunDuration { get; private set; } = 3f;
    [field: SerializeField] public float PushDuration { get; private set; } = 1.5f;
    [field: SerializeField] public float LayTrapDuration { get; private set; } = 1f;
    [field: SerializeField] public Vector2 PushCheckCooldown { get; private set; } = new Vector2(3f, 5f);
    [field: SerializeField] public Vector2 PlaceTrapCooldown { get; private set; } = new Vector2(3f, 5f);

    [Header("Thresholds")]
    [SerializeField, Range(0f, 1f)] private float facingEnemyThreshold = 0.8f;
    [field: SerializeField] public float MinHideDistanceThreshold { get; private set; } = 3f;
    [field: SerializeField, Range(0f, 1f)] public float PushTransitionChance { get; private set; } = 0.3f;
    [field: SerializeField, Range(0f, 1f)] public float LayTrapChance { get; private set; } = 0.3f;

    [Header("UI Display")]
    [SerializeField] Text stateText;

    [Header("Prefabs")]
    [SerializeField] public GameObject trapPrefab;

    // other variables
    [HideInInspector] public bool canBeStunned = true;

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

        // set can be stunned boolean
        canBeStunned = true;

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

    public void SetStateText(string text)
    {
        if (stateText == null) return;
        stateText.text = text;
    }

    // any state transitions
    public void Stun()
    {
        if (!canBeStunned) return;
        SwitchState(Stunned);
    }

    // check if player is nearby within a certain range around the enemy
    public bool PlayerNearby(float range, out Transform player)
    {
        // use sphere cast all, check all nearby objects
        Collider[] hits = Physics.OverlapSphere(transform.position, range);
        // check if anything is hit
        if (hits.Length > 0)
        {
            // loop through all hit objects and check if player is hit
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    player = hit.transform;
                    return true;
                }
            }
        }
        player = null;
        return false;
    }
    
    // get a random point around a center point (usually self)
    public bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        // get a random point in a sphere
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        // get the position on the random point on the navmesh
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }

    // check if the player is moving towards the enemy
    public bool PlayerIsMovingTowardsEnemy(Transform player)
    {
        // get direction player would have to move in to go towards enemy
        Vector3 dirFromPlayer = (transform.position - player.position).normalized;
        // get player controller, return false if not found
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController == null) return false;
        // return false if the player is not moving
        if (playerController.MoveDir == Vector3.zero) return false;
        // use dot product to see how close the player is moving towards the enemy
        return Mathf.Abs(Vector3.Dot(playerController.MoveDir, dirFromPlayer) * dirFromPlayer.magnitude) >= facingEnemyThreshold;
    }

    // place down a trap at current location
    public void PlaceTrap()
    {
        if (trapPrefab == null || TrappablePositionManager.Instance == null) return;
        GameObject obj = Instantiate(
                trapPrefab, 
                new Vector3(transform.position.x, 0f, transform.position.y), 
                Quaternion.identity, 
                TrappablePositionManager.Instance.transform
            );
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
        // show flee distance
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, FleeDistance);
    }
}
