using Clickbait.Utilities;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace ShooterLearning
{
    public class ShooterAgent : Agent
    {
        [Header("References")]
        [SerializeField] GameObject _agentBrainHandlerGO;
        [SerializeField] AgentInput _agentInput;
        [SerializeField] InputReader _heuristicInputReader;
        [SerializeField] AgentRewardSO _rewards;

        [Header("Settings")]
        [SerializeField] float _heuristicRotScaler = 0.01f;
        
        [Header("Debugging")]
        [SerializeField, NonEditable] float _reward;

        // BehaviorParameters _behaviorParameters;
        IAgentBrainHandler _agentBrainHandler;
        
        // Since the decision requester's period time is 5, the agent's action will run 5 times before the brain makes
        // its next decision. This variable makes sure one time tasks like shooting happens only for the first frame
        bool _isFirstDecisionPeriodFrame = true;

        public bool DisableSelf;

        protected override void Awake()
        {
            base.Awake();

            // _behaviorParameters = GetComponent<BehaviorParameters>();
            _agentBrainHandler = _agentBrainHandlerGO.GetComponent<IAgentBrainHandler>();
        }

        public override void Initialize()
        {
            // _behaviorType = ArenaBehaviourBase.GetBehaviourType(_behaviorParameters);

            _agentBrainHandler.Initialize();
        }

        public override void OnEpisodeBegin()
        {
            _agentBrainHandler.OnEpisodeBegin();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            _agentBrainHandler.CollectObservations(sensor);
            _isFirstDecisionPeriodFrame = true;
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            if (DisableSelf) return;
            
            float moveX = actions.ContinuousActions[0];
            float moveY = actions.ContinuousActions[1];
            float lookX = actions.ContinuousActions[2];
            // float lookY = actions.ContinuousActions[3];
            float lookY = 0; // Don't let it look up/down
            int shoot = actions.DiscreteActions[0];
            int jump = actions.DiscreteActions[1];
            // int sprint = actions.DiscreteActions[2];
            
            Vector2 move = new Vector2(moveX, moveY);
            Vector2 look = new Vector2(lookX, lookY);
            bool isShooting = shoot == 1;
            // bool isSprinting = sprint == 1; // The agent doesn't sprint
            bool isSprinting = false;
            bool isJumping = jump == 1;

            if (!_isFirstDecisionPeriodFrame)
            {
                // Don't repeat these one-time actions after the first frame of the Decision Requester's decision period
                isShooting = false;
                isJumping = false;
            }
            
            _agentBrainHandler.OnActionReceived(move, look, isShooting, isJumping, isSprinting);

            if (_rewards.TimeStepPunishment)
            {
                IncrementReward(-1f / MaxStep);
            }
            
            _isFirstDecisionPeriodFrame = false;
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
            continuousActions[0] = _heuristicInputReader.Move[0];
            continuousActions[1] = _heuristicInputReader.Move[1];
            continuousActions[2] = _heuristicInputReader.Look[0] * _heuristicRotScaler;

            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
            discreteActions[0] = _heuristicInputReader.IsShooting.Value ? 1 : 0;
            discreteActions[1] = _heuristicInputReader.Jump ? 1 : 0;
            // discreteActions[2] = _heuristicInputReader.Sprint ? 1 : 0;
        }
        
        public void OnShootEnemy() => IncrementReward(_rewards.ShootEnemyReward);
        public void OnMissEnemy() => IncrementReward(_rewards.MissPunishment);
        
        public void TerminateEpisode() => EndEpisode();
        public void IncrementReward(float reward)
        {
            AddReward(reward);
            _reward = GetCumulativeReward();
        }
    }
}