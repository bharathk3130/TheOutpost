using Clickbait.Utilities;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace ShooterLearning
{
    public class Stage4ObstacleAvoidanceBehaviour : ArenaBehaviourBase
    {
        [Header("Rotation")]
        [SerializeField] float _rotationSmoothing = 4;
        [SerializeField] float _goodAimAlignment = 0.8f;
        [SerializeField] float _perfectAimAlignment = 0.9f;
        
        [Header("Stage 3: Movement")]
        [SerializeField] Transform _dummyTransform;
        [SerializeField] float _tooFarDist = 20;
        [SerializeField] float _tooCloseDist = 5;

        [Header("Stage 4: Obstacle Avoidance")]
        [SerializeField] ThirdPersonShooterController _agentShooterController;
        [SerializeField] Transform _agentShootPoint;
        [SerializeField] WeaponManager _agentWeaponManager;

        DynamicRaycastSensor _isPlayerVisibleSensor;
        
        IDeath _dummyDeath;

        Vector2 _smoothedLook;
        Vector2 _moveInput;

        void Awake()
        {
            _agentShooterController.OnCollisionStayDetected += OnAgentCollisionStay;
        }
        
        public override void Initialize()
        {
            base.Initialize();

            _dummyDeath = _dummyTransform.GetComponent<IDeath>();
            _isPlayerVisibleSensor = new DynamicRaycastSensor(_agentShootPoint, 50);
            _isPlayerVisibleSensor.SetLayerMask(_agentWeaponManager.ShootingLayerMask);
            
            _stage = Stage.Obstacle;

            _dummyDeath.OnDeath += OnDummyDeath;
        }

        void OnAgentCollisionStay(ControllerColliderHit hit)
        {
            if (hit.gameObject.CompareTag("Obstacle") || hit.gameObject.CompareTag("Border"))
            {
                // Punishment for hitting walls
                float dot = Vector3.Dot(hit.normal, GetMoveInputWorldDirection());
                if (dot < -0.6f) // Walking straight into the wall
                {
                    _agent.IncrementReward(_rewards.WallCollisionPenalty);
                }
            }
        }
        
        void OnDummyDeath()
        {
            _agent.IncrementReward(_rewards.KillEnemyReward);
            OnWin();
            _agent.TerminateEpisode();
        }
        
        public override void CollectObservations(VectorSensor sensor)
        {
            // Agent position
            sensor.AddObservation(_agentTransform.localPosition);
            
            // Enemy position
            Vector3 _localDummyPos = _agentTransform.InverseTransformPoint(_dummyTransform.position);
            sensor.AddObservation(_localDummyPos);
            
            // Shooting
            sensor.AddObservation(_weaponManager.PlayerAhead());
            sensor.AddObservation(_weaponManager.Gun.GetNormalizedTimeTillNextShoot());
            
            // Agent rotation
            sensor.AddObservation(_agentTransform.forward.x);
            sensor.AddObservation(_agentTransform.forward.z);
            
            // Agent velocity
            sensor.AddObservation(_agentController.Velocity.x / _agentController.MoveSpeed);
            sensor.AddObservation(_agentController.Velocity.z / _agentController.MoveSpeed);
        }

        public override void OnActionReceived(Vector2 move, Vector2 look, bool shoot, bool jump, bool sprint)
        {
            base.OnActionReceived(move, look, shoot, sprint, jump);
            
            if (shoot)
            {
                _agent.IncrementReward(_rewards.ShootPunishment);
                _weaponManager.Agent_SingleShot();
            }

            _smoothedLook = Vector2.Lerp(_smoothedLook, look, Time.deltaTime * _rotationSmoothing);

            _agentInput.Look = _smoothedLook;
            _agentInput.Move = move;
            _moveInput = move;

            MaintainDistanceReward();
            PreciseAimingReward();
            IsPlayerUnObstructedReward();
        }

        [SerializeField, NonEditable] bool _playerUnobstructed; 
        void IsPlayerUnObstructedReward()
        {
            _isPlayerVisibleSensor.CastToPoint(_dummyTransform.position.With(y: _agentShootPoint.position.y));
            Debug.DrawLine(_agentShootPoint.position, _dummyTransform.position.With(y: _agentShootPoint.position.y), Color.blue);
            if (_isPlayerVisibleSensor.HasDetectedHit() && _isPlayerVisibleSensor.GetHitCollider().CompareTag("Player"))
            {
                _agent.IncrementReward(_rewards.MaintainVisibilityReward);
                _playerUnobstructed = true;
            } else
            {
                _playerUnobstructed = false;
            }
        }

        Vector3 GetMoveInputWorldDirection()
        {
            Vector3 moveVector3 = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
            return _agentTransform.TransformDirection(moveVector3);
        }

        void MaintainDistanceReward()
        {
            float currDist = Vector3.Distance(_agentTransform.localPosition, _dummyTransform.localPosition);
            if (currDist > _tooFarDist)
            {
                Vector3 dummyDir = (_dummyTransform.localPosition - _agentTransform.localPosition).With(y: 0).normalized;
                float moveDot = Vector3.Dot(GetMoveInputWorldDirection(), dummyDir);
                _agent.IncrementReward(_rewards.MoveTowardsShootRangeReward * moveDot);
            } else if (currDist < _tooCloseDist)
            {
                // Give no reward
                //_agent.IncrementReward(-_rewards.MoveTowardsShootRangeReward * (_tooCloseDist - currDist));
            } else
            {
                _agent.IncrementReward(_rewards.StayWithinShootRangeReward);
            }
        }

        void PreciseAimingReward()
        {
            Vector3 dummyDir = (_dummyTransform.position - _agentTransform.position).With(y: 0).normalized;
            Vector3 agentForward = _agentTransform.forward.With(y: 0).normalized;
            float align = Vector3.Dot(agentForward, dummyDir);

            float multiplier;
            if (align < _goodAimAlignment)
                multiplier = 0;
            else if (align < _perfectAimAlignment)
                multiplier = 0.5f;
            else
                multiplier = 1;
            
            _agent.IncrementReward(align * _rewards.LookAtEnemyRewardMultiplier * multiplier);
        }
    }
}