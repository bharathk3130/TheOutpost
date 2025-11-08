using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] bool _maintainOffsetPosition;
    [SerializeField] bool _maintainOffsetRotation;

    Vector3 _initialPosOffset;
    Quaternion _initialRotOffset;

    void Awake()
    {
        if (_maintainOffsetPosition)
            _initialPosOffset = transform.position - _target.position;

        if (_maintainOffsetRotation)
            _initialRotOffset = Quaternion.Inverse(_target.rotation) * transform.rotation;
    }
    
    void LateUpdate()
    {
        if (_maintainOffsetPosition)
            transform.position = _target.position + _initialPosOffset;
        else
            transform.position = _target.position;

        if (_maintainOffsetRotation)
            transform.rotation = _target.rotation * _initialRotOffset;
        else
            transform.rotation = _target.rotation;
    }
}