using System;
using System.Collections;
using StarterAssets;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace ShooterLearning
{
    public abstract class ArenaBehaviourBase : MonoBehaviour, IAgentBrainHandler
    {
        [Header("References")]
        [SerializeField] protected ThirdPersonController _agentController;
        [SerializeField] protected ShooterAgent _agent;
        [SerializeField] InputReader _heuristicInputReader;
        [SerializeField] protected WeaponManager _weaponManager;
        [SerializeField] protected AgentRewardSO _rewards;
        [SerializeField] MonoBehaviour[] _initializables;

        [Header("Indicators")]
        [SerializeField] MeshRenderer _groundRenderer;
        [SerializeField] Material _winIndicator;
        [SerializeField] Material _loseIndicator;
        [SerializeField] float _indicatorDuration = 0.25f;
        Material _defaultGroundMaterial;
        
        [Header("Settings")]
        [SerializeField] int _maxSteps;

        ModelBehaviourType _behaviorType;
        BehaviorParameters _behaviourParameters;
        
        protected Transform _agentTransform;
        protected AgentInput _agentInput;

        protected Stage _stage;
        public Stage CurrentStage => _stage;
        
        IAgentInitializable[] _agentInitializables;
        
        public virtual void Initialize()
        {
            InitializeVariables();
            SetUpInitializablesArray();

            _agent.MaxStep = _maxSteps;
            if (_behaviorType == ModelBehaviourType.Heuristic)
            {
                _heuristicInputReader.Enable();
                _agentController.OnHeuristicEnable();
            }
            
            Array.ForEach(_agentInitializables, initializable => initializable.OnInitialize());
        }

        void InitializeVariables()
        {
            _agentTransform = _agentController.transform;
            _agentInput = _agentTransform.GetComponent<AgentInput>();
            
            _behaviourParameters = _agent.GetComponent<BehaviorParameters>();
            _behaviorType = GetBehaviourType(_behaviourParameters);
            
            _defaultGroundMaterial = _groundRenderer.material;
        }

        void SetUpInitializablesArray()
        {
            _agentInitializables = new IAgentInitializable[_initializables.Length];
            for (int i = 0; i < _initializables.Length; i++)
            {
                if (_initializables[i] is IAgentInitializable initializable)
                {
                    _agentInitializables[i] = initializable;
                } else
                {
                    Debug.LogError($"{_initializables[i].GetType()} is not IAgentInitializable");
                }
            }
        }

        public virtual void OnEpisodeBegin()
        {
            Array.ForEach(_agentInitializables, initializable => initializable.OnEpisodeStart());
        }

        public virtual void CollectObservations(VectorSensor sensor) { }

        public virtual void OnActionReceived(Vector2 move, Vector2 look, bool shoot, bool jump, bool sprint)
        {
            if (_agent.StepCount >= _agent.MaxStep - 1)
            {
                _agent.IncrementReward(_rewards.ExceedTimeStep);
                OnLose();
            }
        }

        // ---------------------------------------------------------
        
        public static ModelBehaviourType GetBehaviourType(BehaviorParameters behaviorParameters)
        {
            ModelBehaviourType behaviourType;

            if (behaviorParameters.BehaviorType == BehaviorType.HeuristicOnly)
            {
                behaviourType = ModelBehaviourType.Heuristic;
            } else if (behaviorParameters.BehaviorType == BehaviorType.Default)
            {
                // If trainer is connected → Learning
                if (Academy.Instance.IsCommunicatorOn)
                    behaviourType = ModelBehaviourType.Learning;
                // If no model assigned and no trainer → Heuristic
                else if (behaviorParameters.Model == null)
                    behaviourType = ModelBehaviourType.Heuristic;
                // Otherwise → Inference
                else
                    behaviourType = ModelBehaviourType.Inference;
            } else if (behaviorParameters.BehaviorType == BehaviorType.InferenceOnly)
            {
                behaviourType = ModelBehaviourType.Inference;
            } else
            {
                behaviourType = ModelBehaviourType.None;
                Debug.LogError("Could not identify agent behaviour type (Heuristic/Inference/Learning)");
            }

            return behaviourType;
        }

        public void OnWin() => StartCoroutine(FlashMaterial(_winIndicator));
        public void OnLose() => StartCoroutine(FlashMaterial(_loseIndicator));
        
        IEnumerator FlashMaterial(Material mat)
        {
            _groundRenderer.material = mat;
            yield return new WaitForSecondsRealtime(_indicatorDuration);
            _groundRenderer.material = _defaultGroundMaterial;
        }
    }
    
    public enum ModelBehaviourType
    {
        Learning,
        Inference,
        Heuristic,
        None
    }

    public enum Stage
    {
        Shoot,
        Walk,
        Move,
        Obstacle,
        Outpost
    }
}