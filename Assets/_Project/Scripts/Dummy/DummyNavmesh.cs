using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class DummyNavmesh : DummyBase
{
    [SerializeField] Transform _arenaTransform;
    [SerializeField] float _range = 10;
    [SerializeField] float _arenaRadius = 16;
    
    NavMeshAgent _navmeshAgent;

    void Start() => OnInitialize();
    
    public override void OnInitialize()
    {
        _navmeshAgent = GetComponent<NavMeshAgent>();
    }
    
    public override void OnEpisodeStart()
    {
        transform.position = GetRandomPointInSurface();
        SetRandomDestination();
        
        _navmeshAgent.speed = GetRandomSpeed();
    }

    void Update()
    {
        if (!_navmeshAgent.pathPending && _navmeshAgent.remainingDistance <= _navmeshAgent.stoppingDistance)
            SetRandomDestination();
    }

    Vector3 GetRandomPointInSurface()
    {
        Vector3 randomPoint = new Vector3(
            _arenaTransform.position.x + Random.Range(-_arenaRadius,  _arenaRadius),
            transform.position.y,
            _arenaTransform.position.z + Random.Range(-_arenaRadius,  _arenaRadius)
        );

        // During training, keep the arenas far apart so that the target doesn't go to a neighbouring arena
        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, _range, NavMesh.AllAreas))
            return hit.position;

        print("Could not find any point on the NavMeshSurface");
        return transform.position;
    }
    
    void SetRandomDestination()
    {
        Vector3 randomPoint = GetRandomPointInSurface();
        _navmeshAgent.SetDestination(randomPoint);
        _targetVisualizer.position = randomPoint;
    }
}