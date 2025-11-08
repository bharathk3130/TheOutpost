using System;

public interface IDeath
{
    bool IsDead { get; }
    
    event Action OnDeath;
    void Die();

    // Freeze/Self destruct only if we aren't training or testing and it's the actual game
    static bool IsFreezeOnDeath() => !TrainingManager.Instance.IsTraining && !TrainingManager.Instance.IsTesting;
}