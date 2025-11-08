using UnityEngine;

[CreateAssetMenu(fileName = "AgentRewardSO", menuName = "ScriptableObjects/AgentRewardSO")]
public class AgentRewardSO : ScriptableObject
{
    [Header("Shooting")]
    public float ShootEnemyReward = 0.5f;
    public float KillEnemyReward = 1f;
    public float ShootPunishment = -0.01f;
    public float MissPunishment = -0.1f;
    
    [Header("Time step")]
    public float ExceedTimeStep = -1f;
    public bool TimeStepPunishment = true;
    
    [Header("Stage 2: Rotation")]
    public float LookAtEnemyRewardMultiplier = 0.001f; // Reward for aiming correctly at the enemy
    public float JitteryRotationPenalty = -0.001f;

    [Header("Stage 3: Movement")]
    public float MoveTowardsShootRangeReward = 0.0002f; // When you're far from the enemy, this reward is given for going closer, and the negative of it is given for going further
    public float StayWithinShootRangeReward = 0.0004f; // Reward for maintaining distance with the enemy
    
    [Header("Stage 4: Obstacle avoidance")]
    public float MaintainVisibilityReward = 0.0005f;
    public float WallCollisionPenalty = -0.001f;

}
