using UnityEngine;

public interface IAngleOverride
{
    public Vector3 GetReflection(Vector3 incomingAngle, Vector3 hitPoint, Vector3 hitNormal);
}
