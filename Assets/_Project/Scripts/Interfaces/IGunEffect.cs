using UnityEngine;

public interface IGunEffect
{
    void PlayEffect(Vector3 bulletSpawnPos, Vector3 endPos, Vector3 normal, bool hit, GameObject hitObject, GunDataSO gunData);
}