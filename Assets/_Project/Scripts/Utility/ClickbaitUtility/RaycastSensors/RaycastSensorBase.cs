using UnityEngine;

namespace Clickbait.Utilities
{
    public abstract class RaycastSensorBase
    {
        protected Transform _source;
        float _castLength;
        LayerMask _layerMask = ~0;
        
        Vector3 _localOrigin = Vector3.zero;
        Vector3 _furthestPoint;
        
        RaycastHit _hitInfo;
        QueryTriggerInteraction _queryTriggerInteraction = QueryTriggerInteraction.Ignore;

        bool _visualize;
        float _visualizationDuration = 0.1f;

        protected RaycastSensorBase(Transform source, float castLength)
        {
            _source = source;
            _castLength = castLength;
        }
        
        protected void CastRay(Vector3 castDirection) => CastRay(castDirection, _castLength);

        protected void CastRay(Vector3 castDirection, float castLength)
        {
            Vector3 worldOrigin = _source.TransformPoint(_localOrigin);
            Physics.Raycast(worldOrigin, castDirection.normalized, out _hitInfo, castLength, _layerMask, _queryTriggerInteraction);

            _furthestPoint = worldOrigin + castDirection * castLength;
            
            if (_visualize)
                VisualizeRay(worldOrigin);
        }

        void VisualizeRay(Vector3 origin)
        {
            Vector3 endPoint = HasDetectedHit() ? GetHitPosition() : GetFurthestPoint();
            Debug.DrawLine(origin, endPoint, Color.red, _visualizationDuration);
        }

        public void SetCastOrigin(Vector3 pos) => _localOrigin = _source.InverseTransformPoint(pos);
        public abstract void Cast();
        
        // RaycastHit return methods
        public bool HasDetectedHit() => _hitInfo.collider != null;
        public float GetDistance() => _hitInfo.distance;
        public Vector3 GetFurthestPoint() => _furthestPoint;
        public Vector3 GetNormal() => _hitInfo.normal;
        public Vector3 GetHitPosition() => _hitInfo.point;
        public Collider GetHitCollider() => _hitInfo.collider;
        public Transform GetHitTransform() => _hitInfo.transform;

        public Vector3 GetRayEndPosition() => HasDetectedHit() ? GetHitPosition() : GetFurthestPoint();

        // Layermask methods
        public void SetLayerMask(LayerMask mask) => _layerMask = mask;
        public void IgnoreLayerOf(GameObject go) => _layerMask &= ~(1 << go.layer);

        // Detect trigger colliders?
        public void DetectTriggerColliders() => _queryTriggerInteraction = QueryTriggerInteraction.Collide;
        public void IgnoreTriggerColliders() => _queryTriggerInteraction = QueryTriggerInteraction.Ignore;
        
        // Visualize
        public void EnableVisualization() => _visualize = true;
        public void DisableVisualization() => _visualize = false;
        public void SetVisualizationDuration(float duration) => _visualizationDuration = duration;
    }
}