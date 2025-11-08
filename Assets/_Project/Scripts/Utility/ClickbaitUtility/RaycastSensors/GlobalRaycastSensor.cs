using UnityEngine;

namespace Clickbait.Utilities
{
    // Shoot the raycast in 1 global direction always (Ex: Vector3.down for ground check)
    public class GlobalRaycastSensor : RaycastSensorBase
    {
        Vector3 _castDirection;
        
        public GlobalRaycastSensor(Transform source, Vector3 castDirection, float castLength) : base(source, castLength)
        {
            _castDirection = castDirection;
        }

        public override void Cast() => CastRay(_castDirection);
    }
}