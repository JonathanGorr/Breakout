using UnityEngine;

public class Powerup : MonoBehaviour
{
    public Vector3 directionVector = -Vector3.forward;
    public float moveSpeed = 4;
    public float radius = 1;
    public LayerMask layermask;

    private void FixedUpdate()
    {
        transform.position += directionVector * moveSpeed * Time.deltaTime;

        OutOfBoundsCheck();

        if (Physics.Raycast(transform.position, directionVector * radius, out RaycastHit hit, radius))
        {
            if (hit.collider.TryGetComponent<IPaddle>(out IPaddle paddle))
            {
                paddle.Powerup(this);
                Destroy(this.gameObject);
            }
        }
    }

    private void OutOfBoundsCheck()
    {
        if (transform.position.x > 13 || transform.position.x < -13
            || transform.position.y > 5 || transform.position.y < -5
            || transform.position.z > 13 || transform.position.z < -13)
        {
            Destroy(this.gameObject);
        }
    }

    #region debugging

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (Physics.Raycast(transform.position, directionVector * radius * 4, out RaycastHit hit, radius * 4, layermask))
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
            Gizmos.DrawRay(transform.position, directionVector * radius * 4);
        }
    }

    #endregion

}
