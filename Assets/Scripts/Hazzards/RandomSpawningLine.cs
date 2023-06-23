using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawningLine : MonoBehaviour
{
    private ObjectPooler spawner;

    [Header("Config")]
    [SerializeField]
    ObjectPooler.PoolObjName prefabSpawning;
    [SerializeField]
    private Vector2 lineStart;
    [SerializeField]
    private Vector2 lineEnd;
    [SerializeField]
    private Timer timePerSpawn;

    private void Start()
    {
        spawner = ObjectPooler.Instance;
        timePerSpawn.SetTimerOnDoneAction(SpawnAtRandomPoint);
        timePerSpawn.StartLoopTimer(this);
    }

    private void SpawnAtRandomPoint()
    {
        Vector3 spawnPosition = Vector2.Lerp(lineStart, lineEnd, Random.Range(0.0f, 1.0f));
        spawner.SpawnFromPool(prefabSpawning, spawnPosition, Quaternion.identity);
        Debug.Log("Spawned!");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(lineStart, lineEnd);
    }
}
