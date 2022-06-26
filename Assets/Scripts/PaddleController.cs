using UnityEngine;
using Random = UnityEngine.Random;

public interface IPaddle
{
    public void Powerup(Powerup p);
}

public class PaddleController : CubeBase, IAngleOverride, IPaddle
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform leftLimit;
    [SerializeField] private Transform rightLimit;
    [SerializeField] private float smoothTime = 0.1f;
    [SerializeField] private float speed = 1;
    [SerializeField] private float reflectedAngleExtent = 30;

    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Health health;
    [SerializeField] private BallSpawner spawner;
    [SerializeField] private AudioClip powerupCollected;
    [SerializeField] private AudioSource asrc;

    private float direction;
    private float xVelocity;
    private float currentVelocity;

    [Header("Debug")]
    [SerializeField] private float length = 4;
    [SerializeField] private Transform tester;

    #region reflections

    private Vector3 LeftExtent => new Vector3(boxCollider.bounds.min.x, boxCollider.bounds.max.y, boxCollider.bounds.max.z);
    private Vector3 RightExtent => boxCollider.bounds.max;
    public float NormalizedPosition => Mathf.InverseLerp(leftLimit.position.x, rightLimit.position.x, target.transform.position.x);

    public Vector3 GetRandomReflectionVector()
    {
        return Quaternion.AngleAxis(Mathf.Lerp(-reflectedAngleExtent, reflectedAngleExtent, Random.Range(0.3f, 0.7f)),
            Vector3.up) * Vector3.forward;
    }

    public Vector3 GetReflection(Vector3 inVector, Vector3 hitPoint, Vector3 hitNormal)
    {
        float normalizedPositionAlongX = Mathf.InverseLerp(LeftExtent.x, RightExtent.x, hitPoint.x);
        float correctedAngle = Mathf.Lerp(-reflectedAngleExtent, reflectedAngleExtent, normalizedPositionAlongX);
        return Quaternion.AngleAxis(correctedAngle, Vector3.up) * hitNormal;
    }

    #endregion

    #region unity events

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            if (direction > 0)
            {
                currentVelocity = 0;
            }
            direction = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            if (direction < 0)
            {
                currentVelocity = 0;
            }
            direction = 1;
        }
        else
        {
            direction = 0;
        }

        xVelocity = Mathf.SmoothDamp(xVelocity, direction * speed, ref currentVelocity, smoothTime);
        float x = Mathf.Clamp(target.position.x + xVelocity, leftLimit.position.x, rightLimit.position.x);
        target.transform.position = new Vector3(x, target.position.y, target.position.z);
    }

    #endregion

    public void Powerup(Powerup p)
    {
        spawner.Spawn();
        asrc.PlayOneShot(powerupCollected);
    }

    #region debugging

    private void OnDrawGizmos()
    {
        PaintTester();
        //Gizmos.DrawRay(transform.position, GetRandomReflectionVector() * 4);

        void PaintTester()
        {
            if (tester == null || !tester.gameObject.activeInHierarchy)
                return;

            DrawExtents();
            Gizmos.color = Color.red;

            if (Physics.Raycast(tester.position, tester.forward * length, out RaycastHit hit, length))
            {
                Gizmos.color = Color.green;
                float normalizedPositionAlongX = Mathf.InverseLerp(LeftExtent.x, RightExtent.x, hit.point.x);
                Vector3 direction = hit.point - tester.position;
                Vector3 inVector = direction.normalized;
                //Vector3 reflection = Vector3.Reflect(inVector, hit.normal).normalized;
                Vector3 reflection = Quaternion.AngleAxis(Mathf.Lerp(-reflectedAngleExtent, reflectedAngleExtent, 
                    normalizedPositionAlongX), Vector3.up) * hit.normal;
                Gizmos.DrawRay(tester.position, direction);
                Gizmos.DrawRay(hit.point, reflection * 4);
            }
            else
            {
                Gizmos.DrawRay(tester.position, tester.forward * length);
            }
        }

        void DrawExtents()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(LeftExtent, 0.1f);
            Gizmos.DrawSphere(RightExtent, 0.1f);
        }
    }

    #endregion

}
