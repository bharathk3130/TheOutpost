using StarterAssets;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace ShooterLearning
{
    public class AgentBrainHandler : MonoBehaviour, IAgentBrainHandler
    {
        [SerializeField] WeaponManager _weaponManager;
        [SerializeField] Transform _playerTransform;
        [SerializeField] float _rotationSmoothing = 8;
        
        Vector2 _smoothedLook;
        ThirdPersonController _controller;
        AgentInput _agentInput;
        IAgentBrainHandler _iAgentBrainHandlerImplementation;
        
        bool _isDecisionFrame = true;
        
        public void SetPlayerTransform(Transform playerTransform) => _playerTransform = playerTransform;

        void Start() => Initialize();
        
        public void Initialize()
        {
            _controller = GetComponent<ThirdPersonController>();
            _agentInput = GetComponent<AgentInput>();
        }

        public void OnEpisodeBegin() { }

        public void CollectObservations(VectorSensor sensor)
        {
            // Agent position
            sensor.AddObservation(transform.localPosition);
            
            // Enemy position
            Vector3 localPlayerPosition = _playerTransform == null
                ? Vector3.zero
                : transform.InverseTransformPoint(_playerTransform.position);
            sensor.AddObservation(localPlayerPosition);
            
            // Shooting
            sensor.AddObservation(_weaponManager.PlayerAhead());
            sensor.AddObservation(_weaponManager.GetNormalizedTimeTillNextShoot());
            
            // Agent rotation
            sensor.AddObservation(transform.forward.x);
            sensor.AddObservation(transform.forward.z);
            
            // Agent velocity
            sensor.AddObservation(_controller.Velocity.x / _controller.MoveSpeed);
            sensor.AddObservation(_controller.Velocity.z / _controller.MoveSpeed);
            
            _isDecisionFrame = true;
        }

        public void OnActionReceived(Vector2 move, Vector2 look, bool shoot, bool jump, bool sprint)
        {
            if (!_isDecisionFrame)
            {
                shoot = false;
                jump = false;
            } else
            {
                _isDecisionFrame = false;
            }
            
            if (shoot)
            {
                // _agent.IncrementReward(_rewards.ShootPunishment);
                _weaponManager.Agent_SingleShot();
            }

            _smoothedLook = Vector2.Lerp(_smoothedLook, look, Time.deltaTime * _rotationSmoothing);

            _agentInput.Look = _smoothedLook;
            _agentInput.Move = move;
        }
    }
}