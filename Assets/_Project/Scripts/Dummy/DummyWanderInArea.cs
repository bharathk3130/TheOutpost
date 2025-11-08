using Clickbait.Utilities;
using StarterAssets;
using UnityEngine;
using Random = UnityEngine.Random;

public class DummyWanderInArea : DummyBase
{
    [SerializeField] bool _isOutpostAgent;
    [SerializeField] float _maxDisplacement = 3.3f;
    [SerializeField] float _waitTime = 3;
    [SerializeField] float _waitVariation = 1;
    [SerializeField] float _turnSpeed = 10;
    
    ThirdPersonController _controller;
    AgentInput _agentInput;
    Transform _shooterVisuals;
    Quaternion _shooterVisualsRot;

    Vector3 _target;
    Vector3 _lastPos;
    
    float _stuckTimer;

    bool _reached;

    void Awake() => OnInitialize();
    void Start() => OnEpisodeStart();
    
    public void SetPlayerController(ThirdPersonController controller) => _controller = controller;

    public override void OnInitialize()
    {
        _controller =  GetComponent<ThirdPersonController>();
        _agentInput = GetComponent<AgentInput>();
        _shooterVisuals = transform.GetChild(0);
        _shooterVisualsRot = _shooterVisuals.rotation;
    }

    public override void OnEpisodeStart()
    {
        if (!_isOutpostAgent)
        {
            transform.localPosition = GetRandomPointInBounds();
            _controller.MoveSpeed = GetRandomSpeed();
        }

        UpdateTarget();
    }
    
    void UpdateTarget()
    {
        _reached = false;
        _target = GetRandomPointInBounds();
        if (_targetVisualizer != null) _targetVisualizer.localPosition = _target;
    }
    
    Vector3 GetRandomPointInBounds()
    {
        return new Vector3(Random.Range(-_maxDisplacement, _maxDisplacement), 0,
            Random.Range(-_maxDisplacement, _maxDisplacement));
    }
    
    void Update()
    {
        if (_reached) return;
        
        Vector3 dir = _target - transform.localPosition;
        Movement(dir);
        Rotation(dir);

        if (dir.sqrMagnitude < 0.01f) // Reached target
        {
            _reached = true;
            _agentInput.ResetInputs();
            float randomWaitTime = _waitTime + Random.Range(-_waitVariation, _waitVariation);
            Invoke(nameof(UpdateTarget), randomWaitTime);
        }

        // Detect if stuck
        if (_moveSpeed != MoveSpeed.Stand && (transform.localPosition - _lastPos).sqrMagnitude < 0.0001f)
            _stuckTimer += Time.deltaTime;
        else if (_stuckTimer > 0)
            _stuckTimer = 0;

        if (_stuckTimer > 1f) // stuck for more than 1 second
        {
            print("Stuck");
            UpdateTarget();
            _stuckTimer = 0;
        }

        _lastPos = transform.localPosition;
    }
    
    void Rotation(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.1f) return;
        Quaternion targetRot = Quaternion.LookRotation(dir.With(y: 0));
        _shooterVisualsRot = Quaternion.Lerp(_shooterVisualsRot, targetRot, Time.deltaTime * _turnSpeed);
        _shooterVisuals.rotation = _shooterVisualsRot;
    }

    // Return if true if it has reset to 0
    public bool ResetRotation()
    {
        _shooterVisualsRot = Quaternion.Lerp(_shooterVisualsRot, transform.rotation,Time.deltaTime * _turnSpeed);
        if (Quaternion.Angle(_shooterVisualsRot, transform.rotation) < 0.1f)
        {
            _shooterVisuals.rotation = transform.rotation;
            return true;
        }
        
        _shooterVisuals.rotation = _shooterVisualsRot;
        return false;
    }

    void Movement(Vector3 dir)
    {
        Vector3 dirNormalized = dir.normalized;
        _agentInput.Move = new Vector2(dirNormalized.x, dirNormalized.z);
    }
}
