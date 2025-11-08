using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace ShooterLearning
{
    public class Stage2RotBehaviour : ArenaBehaviourBase
    {
        [Header("Stage 2: Rotation")]
        [SerializeField] DummyWanderInArea _dummy;
        [SerializeField] float _rotationSmoothing = 6;
        IDeath _dummyDeath;

        Vector2 _smoothedLook;
        float _curricula_aimBonusMultiplier;
        
        float _prevRotationInput;
        
        public override void Initialize()
        {
            base.Initialize();

            _dummyDeath = _dummy.GetComponent<IDeath>();
            _stage = Stage.Walk;

            _dummyDeath.OnDeath += OnDummyDeath;
            
            _curricula_aimBonusMultiplier = Academy.Instance.EnvironmentParameters.GetWithDefault("aim_bonus", 0);
        }

        public override void OnEpisodeBegin()
        {
            base.OnEpisodeBegin();
            
            _prevRotationInput = 0;
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
            Vector3 _localDummyPos = _agentTransform.InverseTransformPoint(_dummy.transform.position);
            sensor.AddObservation(_localDummyPos);
            
            // Shooting
            sensor.AddObservation(_weaponManager.PlayerAhead());
            sensor.AddObservation(_weaponManager.Gun.GetNormalizedTimeTillNextShoot());
            
            // Agent rotation
            sensor.AddObservation(_agentTransform.forward.x);
            sensor.AddObservation(_agentTransform.forward.z);
            
            // Agent velocity
            sensor.AddObservation(0);  // _controller.Velocity.x / _controller.MoveSpeed);
            sensor.AddObservation(0);  // _controller.Velocity.z / _controller.MoveSpeed);
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
            
            AimingReward(look);

            _prevRotationInput = look.x;
        }

        void AimingReward(Vector2 lookInput)
        {
            if (TrainingManager.Instance.IsTraining)
            {
                // Curriculum for aiming
                if (_curricula_aimBonusMultiplier != 0)
                {
                    _curricula_aimBonusMultiplier =
                        Academy.Instance.EnvironmentParameters.GetWithDefault("aim_bonus", 0);

                    Vector3 dummyDir = (_dummy.transform.position - _agentTransform.position).normalized;
                    float align = Vector3.Dot(_agentTransform.forward, dummyDir);
                    _agent.IncrementReward(align * _rewards.LookAtEnemyRewardMultiplier *
                                           _curricula_aimBonusMultiplier);
                }

                // Agent turned from left to right - reduce jittering
                if ((lookInput.x < 0 && _prevRotationInput > 0) || (lookInput.x > 0 &&  _prevRotationInput < 0))
                {
                    _agent.IncrementReward(_rewards.JitteryRotationPenalty);
                }
            }
        }
    }
}