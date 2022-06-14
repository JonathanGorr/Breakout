using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PaddleController paddle;

    public Transform leftLimit;
    public Transform rightLimit;

    private Vector3 currentVelocity;
    public float smoothTime = 0.5f;
    public float yAngle = 6;

    private void LateUpdate()
    {
        float normalizedPosition = paddle.NormalizedPosition;
        Vector3 position = Vector3.Lerp(leftLimit.position, rightLimit.position, normalizedPosition);
        transform.position = Vector3.SmoothDamp(transform.position, position, ref currentVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            Mathf.Lerp(-yAngle, yAngle, normalizedPosition), 
            transform.rotation.eulerAngles.z);
    }
}
