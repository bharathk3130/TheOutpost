using Clickbait.Utilities;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    Transform _cam;
    
    void Start()
    {
        _cam = Camera.main.transform;
    }

    void Update()
    {
        Vector3 lookPos = (transform.position - _cam.position).With(y: 0).normalized;
        transform.rotation = Quaternion.LookRotation(lookPos);
    }
}
