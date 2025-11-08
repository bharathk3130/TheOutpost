using Clickbait.Utilities;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace ShooterLearning
{
    public class Stage3MoveBehaviour : ArenaBehaviourBase
    {
        [Header("Rotation")]
        [SerializeField] float _rotationSmoothing = 4;
        [SerializeField] float _goodAimAlignment = 0.8f;
        [SerializeField] float _perfectAimAlignment = 0.9f;
        
        [Header("Stage 3: Movement")]
        [SerializeField] Transform _dummyTransform;
        [SerializeField] float _tooFarDist = 20;
        [SerializeField] float _tooCloseDist = 5;
        
        IDeath _dummyDeath;

        Vector2 _smoothedLook;

        bool _isDecisionFrame = true;
        
        public override void Initialize()
        {
            base.Initialize();

            _dummyDeath = _dummyTransform.GetComponent<IDeath>();
            _stage = Stage.Move;

            _dummyDeath.OnDeath += OnDummyDeath;
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
            
            _isDecisionFrame = true;
        }

        public override void OnActionReceived(Vector2 move, Vector2 look, bool shoot, bool jump, bool sprint)
        {
            base.OnActionReceived(move, look, shoot, sprint, jump);

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
                _agent.IncrementReward(_rewards.ShootPunishment);
                _weaponManager.Agent_SingleShot();
            }

            _smoothedLook = Vector2.Lerp(_smoothedLook, look, Time.deltaTime * _rotationSmoothing);

            _agentInput.Look = _smoothedLook;
            _agentInput.Move = move;

            MaintainDistanceReward(move);
            PreciseAimingReward();
        }

        void MaintainDistanceReward(Vector2 moveInput)
        {
            Vector3 moveVector3 = new Vector3(moveInput.x, 0, moveInput.y).normalized;
            float currDist = Vector3.Distance(_agentTransform.localPosition, _dummyTransform.localPosition);
            if (currDist > _tooFarDist)
            {
                Vector3 moveInputWorld = _agentTransform.TransformDirection(moveVector3);
                Vector3 dummyDir = (_dummyTransform.localPosition - _agentTransform.localPosition).With(y: 0).normalized;
                float moveDot = Vector3.Dot(moveInputWorld, dummyDir);
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

        [SerializeField, NonEditable] float _alignment;
        void PreciseAimingReward()
        {
            Vector3 dummyDir = (_dummyTransform.position - _agentTransform.position).With(y: 0).normalized;
            Vector3 agentForward = _agentTransform.forward.With(y: 0).normalized;
            float align = Vector3.Dot(agentForward, dummyDir);
            _alignment = align;

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