using UnityEngine;

namespace Clickbait.Utilities
{
    // Specify the target point every time you shoot a raycast
    public class DynamicRaycastSensor : RaycastSensorBase
    {
        public DynamicRaycastSensor(Transform source, float castLength) : base(source, castLength) { }

        public void CastToPoint(Vector3 target, float overshootDistance = 1)
        {
            Vector3 dir = target - _source.position;
            float distance = dir.magnitude;
            CastRay(dir, distance + overshootDistance);
        }

        public void CastTowards(Vector3 target)
        {
            Vector3 dir = target - _source.position;
            CastRay(dir);
        }

        public void CastInDirection(Vector3 dir)
        {
            CastRay(dir);
        }

        public override void Cast()
        {
            Debug.LogError("Cast() called in DynamicRaycastSensor with no parameter");
            CastRay(Vector3.zero);
        }
    }
}