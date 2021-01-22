using UnityEngine;

public class FogOfWarTestNeutralSpawner : MonoBehaviour
{
    public GameObject prefab;
    public int maxSpawns = 10;
    public float spawnSpeed = 1;
    int _spawns = 0;
    float _lastSpawnTime = 0;

    void Update()
    {
        _lastSpawnTime += Time.deltaTime;
        if (_lastSpawnTime > spawnSpeed && _spawns < maxSpawns)
        {
            Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            _lastSpawnTime = 0;
            ++_spawns;
        }
    }
}
