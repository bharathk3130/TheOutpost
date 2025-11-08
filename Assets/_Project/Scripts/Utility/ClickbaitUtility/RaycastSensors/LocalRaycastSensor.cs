using UnityEngine;

namespace Clickbait.Utilities
{
    // Shoot a raycast in a certain direction w.r.t the source (Ex: Forward from a gun)
    public class LocalRaycastSensor : RaycastSensorBase
    {
        public enum CastDirection { Forward, Right, Up, Backward, Left, Down }
        CastDirection _castDirection;
        
        public LocalRaycastSensor(Transform source, CastDirection castDirection, float castLength) : base(source, castLength)
        {
            _castDirection = castDirection;
        }

        public override void Cast() => CastRay(GetCastDirection());

        Vector3 GetCastDirection()
        {
            return _castDirection switch
            {
                CastDirection.Forward => _source.forward,
                CastDirection.Right => _source.right,
                CastDirection.Up => _source.up,
                CastDirection.Backward => -_source.forward,
                CastDirection.Left => -_source.right,
                CastDirection.Down => -_source.up,
                _ => Vector3.one
            };
        }
    }
}