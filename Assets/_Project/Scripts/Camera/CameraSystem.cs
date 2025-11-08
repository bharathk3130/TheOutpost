using System;
using Cinemachine;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _mainVirtualCam;
    [SerializeField] CinemachineVirtualCamera _aimVirtualCam;

    [SerializeField] InputReader _inputReader;
    
    // CameraSystem[] _otherCameraSystems;

    void Awake()
    {
        // _otherCameraSystems = new CameraSystem[transform.parent.childCount - 1];
        // for (int i = 0, otherCamIndex = 0; i < transform.parent.childCount; i++)
        // {
        //     CameraSystem cameraSystem = transform.parent.GetChild(i).GetComponent<CameraSystem>();
        //     if (cameraSystem == this) continue;
        //     
        //     _otherCameraSystems[otherCamIndex++] = cameraSystem;
        // }
    }
    
    void Start()
    {
        _inputReader.IsAiming.AddListener(OnAimToggle);
    }

    public void EnableAndSetTarget(Transform target)
    {
        gameObject.SetActive(true);
        _mainVirtualCam.Follow = target;
        _aimVirtualCam.Follow = target;
        
        // Array.ForEach(_otherCameraSystems, otherCameraSystem =>  otherCameraSystem.gameObject.SetActive(false));
    }

    void OnAimToggle(bool isAiming)
    {
        _mainVirtualCam.gameObject.SetActive(!isAiming);
        _aimVirtualCam.gameObject.SetActive(isAiming);
    }
}
