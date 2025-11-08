using Unity.MLAgents.Sensors;
using UnityEngine;

namespace ShooterLearning
{
    public class Stage1ShootBehaviour : ArenaBehaviourBase
    {
        [Header("Stage 1: Shoot")]
        [SerializeField] DummyWanderInArea _dummy;
        IDeath _dummyDeath;
        
        public override void Initialize()
        {
            base.Initialize();

            _dummyDeath = _dummy.GetComponent<IDeath>();
            _stage = Stage.Shoot;

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
            // Agent position (3)
            sensor.AddObservation(_agentTransform.localPosition);
            
            // Enemy position (3)
            Vector3 _localDummyPos = _agentTransform.InverseTransformPoint(_dummy.transform.position);
            sensor.AddObservation(_localDummyPos);
            
            // Shooting (2)
            sensor.AddObservation(_weaponManager.PlayerAhead());
            sensor.AddObservation(_weaponManager.Gun.GetNormalizedTimeTillNextShoot());
            
            // Agent rotation (2)
            sensor.AddObservation(0);  // _agentTransform.forward.x);
            sensor.AddObservation(0);  // _agentTransform.forward.z);
            
            // Agent velocity (2)
            sensor.AddObservation(0);  // _controller.Velocity.x / _controller.MoveSpeed);
            sensor.AddObservation(0);  // _controller.Velocity.z / _controller.MoveSpeed);
            
            // Total = 12 values
        }

        public override void OnActionReceived(Vector2 move, Vector2 look, bool shoot, bool jump, bool sprint)
        {
            base.OnActionReceived(move, look, shoot, sprint, jump);
            
            if (shoot)
            {
                _agent.IncrementReward(_rewards.ShootPunishment);
                _weaponManager.Agent_SingleShot();
            }
        }
    }
}