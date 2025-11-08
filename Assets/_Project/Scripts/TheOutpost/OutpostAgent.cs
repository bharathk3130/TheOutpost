using Clickbait.Utilities;
using ShooterLearning;
using UnityEngine;

public class OutpostAgent : MonoBehaviour
{
    [SerializeField] TransmissionZone _transmissionZone;
    [SerializeField] ShooterAgent _agent;

    IDeath _death;
    AgentInput _agentInput;
    DummyWanderInArea _dummyWanderInArea;
    AgentBrainHandler _agentBrainHandler;
    OutpostAgentSpawner _spawner;

    bool _playerInZone;
    bool _resetVisualsRot;

    bool _walkingToZone = true;
    bool _subscribed;

    void Awake()
    {
        _agentBrainHandler = GetComponent<AgentBrainHandler>();
        _dummyWanderInArea = GetComponent<DummyWanderInArea>();
        _agentInput = GetComponent<AgentInput>();
        _death = GetComponent<IDeath>();
        
        // Disable in-zone behaviour
        _agent.DisableSelf = true;
        _dummyWanderInArea.enabled = false;
        _agentInput.ResetInputs();
        
        gameObject.layer = LayerMask.NameToLayer("SpawnedAgent");
    }
    
    void Start()
    {
        Vector3 dir = (_transmissionZone.transform.position - transform.position).With(y: 0).normalized;
        _agentInput.Move = new Vector2(dir.x, dir.z);
    }

    public void Initialize(OutpostAgentSpawner spawner, Transform playerTransform, TransmissionZone zone)
    {
        _spawner = spawner;
        _death.OnDeath += _spawner.OnAgentDeath;
        _agentBrainHandler.SetPlayerTransform(playerTransform);
        _transmissionZone = zone;

        if (!_subscribed)
        {
            Subscribe();
        }
    }

    void Update()
    {
        if (_resetVisualsRot && _playerInZone)
        {
            _resetVisualsRot = !_dummyWanderInArea.ResetRotation(); // If it returns true, then rotation has been reset
        }
    }

    void OnEnable()
    {
        if (!_subscribed && _transmissionZone != null)
        {
            Subscribe();
        }
    }

    void Subscribe()
    {
        _transmissionZone.OnPlayerEnter += OnPlayerEnter;
        _transmissionZone.OnPlayerExit += OnPlayerExit;
        _subscribed = true;
    }

    void OnDisable()
    {
        _transmissionZone.OnPlayerEnter -= OnPlayerEnter;
        _transmissionZone.OnPlayerExit -= OnPlayerExit;
        _subscribed = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (_walkingToZone && col.gameObject.layer == LayerMask.NameToLayer("TransmissionZone"))
        {
            _walkingToZone = false;
            gameObject.layer = LayerMask.NameToLayer("Agent");
            
            if (_transmissionZone.IsPlayerInZone)
            {
                EnableAIBehaviour();
            } else
            {
                EnableWanderBehaviour();
            }
        }
    }

    void OnPlayerEnter()
    {
        _playerInZone = true;
        _resetVisualsRot = true;
        if (!_walkingToZone)
        {
            EnableAIBehaviour();
        }
    }

    void OnPlayerExit()
    {
        _playerInZone = false;
        EnableWanderBehaviour();
    }

    void EnableWanderBehaviour()
    {
        _agent.DisableSelf = true;
        _agentInput.ResetInputs();
        _dummyWanderInArea.enabled = true;
    }
    
    void EnableAIBehaviour()
    {
        _dummyWanderInArea.enabled = false;
        _agentInput.ResetInputs();
        _agent.DisableSelf = false;
    }
}