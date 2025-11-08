using Clickbait.Utilities;
using StarterAssets;
using UnityEngine;

public abstract class DummyBase : MonoBehaviour, IAgentInitializable
{
    [SerializeField] ThirdPersonController _playerControllerPrefab;
    [SerializeField] float _standingProbability = 0.2f;
    [SerializeField] float _runningProbability = 0.5f;
    
    [Header("Debugging")]
    [SerializeField, NonEditable] protected MoveSpeed _moveSpeed;
    [SerializeField] protected Transform _targetVisualizer;
    
    protected enum MoveSpeed
    {
        Stand,
        Run,
        Sprint
    }

    public abstract void OnInitialize();
    public abstract void OnEpisodeStart();
    
    protected float GetRandomSpeed()
    {
        float speed;
        int rand = Random.Range(0, 10);
        if (rand < _standingProbability * 10)
        {
            speed = 0;
            _moveSpeed = MoveSpeed.Stand;
        } else if (rand < (_standingProbability + _runningProbability) * 10)
        {
            speed = _playerControllerPrefab.MoveSpeed;
            _moveSpeed = MoveSpeed.Run;
        }
        else
        {
            speed = _playerControllerPrefab.SprintSpeed;
            _moveSpeed = MoveSpeed.Sprint;
        }
        
        return speed;
    }
}