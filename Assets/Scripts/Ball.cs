using UnityEngine;
using UnityEngine.Events;

public class Ball : CubeBase
{
    [Header("Ball")]
    [SerializeField] private float length = 4;
    [SerializeField] private float speed = 16;
    [SerializeField] private float radius = 0.4f;
    [SerializeField] private Health health;
    [SerializeField] private LayerMask layermask;

    public static int BallCount = 0;
    public static UnityEvent OnDeathEvent = new UnityEvent();
    private Vector3 directionVector = Vector3.zero;
    //private List<Tuple<Vector3, Vector3>> hits = new List<Tuple<Vector3, Vector3>>();

    #region unity events

    private void Awake()
    {
        health.OnDeath.AddListener(OnDeath);
        Application.quitting += OnApplicationQuitting;
    }

    private void Start()
    {
        Ball.BallCount = Ball.BallCount + 1;
        health.Resurrect();
    }

    private void OnEnable()
    {
        GameManager.Instance.GameState.DestroyBallsEvent.AddListener(OnDestroyRequested);
    }

    private void OnDisable()
    {
        GameManager.Instance?.GameState?.DestroyBallsEvent.RemoveListener(OnDestroyRequested);
    }

    private void OnDestroy()
    {
        if (health.State != Health.EHealthState.Dead)
        {
            Ball.BallCount--;
        }
        Application.quitting -= OnApplicationQuitting;
        health.OnDeath.RemoveListener(OnDeath);
    }

    private void OnApplicationQuitting()
    {
        health.OnDeath.RemoveListener(OnDeath);
    }

    #endregion

    private void OnDestroyRequested()
    {
        directionVector = Vector3.zero;
        Destroy(this.gameObject);
    }

    public void Send(Vector3 direction)
    {
        directionVector = direction;
    }

    private void OutOfBoundsCheck()
    {
        if (transform.position.x > 13 || transform.position.x < -13
            || transform.position.y > 5 || transform.position.y < -5
            || transform.position.z > 13 || transform.position.z < -13)
        {
            health.Kill(true);
        }
    }

    private void FixedUpdate()
    {
        if (health.State != Health.EHealthState.Alive)
            return;

        transform.position += directionVector * speed * Time.deltaTime;

        OutOfBoundsCheck();

        if (Physics.Raycast(transform.position, directionVector * radius, out RaycastHit hit, radius, layermask))
        {
            Vector3 direction = hit.point - transform.position;
            Vector3 inVector = direction.normalized;

            if (hit.collider.TryGetComponent<IAngleOverride>(out IAngleOverride angleOverride))
            {
                Vector3 reflection = angleOverride.GetReflection(inVector, hit.point, hit.normal);
                Vector3 flattenedReflection = Vector3.ProjectOnPlane(reflection, Vector3.up);
                directionVector = flattenedReflection;
            }
            else
            {
                Vector3 reflection = Vector3.Reflect(inVector, hit.normal).normalized;
                Vector3 flattenedReflection = Vector3.ProjectOnPlane(reflection, Vector3.up);
                //hits.Add(new Tuple<Vector3, Vector3>(hit.point, flattenedReflection));
                directionVector = flattenedReflection;
            }
            if (hit.collider.TryGetComponent<IHealth>(out IHealth health))
            {
                health.DealDamage(1);
            }
            OnCollide();
        }
    }

    private void OnCollide()
    {
        health.DealDamage(0);
    }

    private void OnDeath()
    {
        Ball.BallCount--;
        directionVector = Vector3.zero;
        speed = 0;
        OnDeathEvent.Invoke();
        Invoke(nameof(OnDestroyRequested), 1f);
    }

    #region debugging

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, radius);

        if (Physics.Raycast(transform.position, directionVector * length, out RaycastHit hit, length))
        {
            Gizmos.color = Color.green;
            Vector3 direction = hit.point - transform.position;
            Vector3 inVector = direction.normalized;
            Vector3 reflection = Vector3.Reflect(inVector, hit.normal).normalized;
            Gizmos.DrawRay(transform.position, direction);
            Gizmos.DrawRay(hit.point, reflection * 4);
        }
        else
        {
            Gizmos.DrawRay(transform.position, directionVector * length);
        }

        Gizmos.color = Color.magenta;
        /*
        foreach(var cachedHits in hits)
        {
            Gizmos.DrawRay(cachedHits.Item1, cachedHits.Item2);
        }
        */
    }

    #endregion

}
