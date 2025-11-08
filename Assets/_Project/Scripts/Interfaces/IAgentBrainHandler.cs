using Unity.MLAgents.Sensors;
using UnityEngine;

namespace ShooterLearning
{
    public interface IAgentBrainHandler
    {
        void Initialize();
        void OnEpisodeBegin();
        void CollectObservations(VectorSensor sensor);
        void OnActionReceived(Vector2 move, Vector2 look, bool shoot, bool jump, bool sprint);
    }
}