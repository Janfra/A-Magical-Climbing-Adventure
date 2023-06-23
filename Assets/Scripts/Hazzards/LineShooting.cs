using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineShooting : MonoBehaviour
{
    [Header("Dependencies")]
    private ObjectPooler spawner;

    [Header("Configuration")]
    [SerializeField]
    private ObjectPooler.PoolObjName spawnObject;
    [SerializeField]
    private Vector2 targetDirection = Vector3.right;
    [SerializeField]
    private float distance = 1.0f;
    [SerializeField]
    private float objectSpeed = 1.0f;

    [SerializeField]
    private Timer timePerShot;

    private void OnValidate()
    {
       targetDirection.Normalize();
    }

    private void Start()
    {
        spawner = ObjectPooler.Instance;
        timePerShot.SetTimerOnDoneAction(ShootObjectToTarget);
        timePerShot.StartLoopTimer(this);
        ShootObjectToTarget();
    }

    private void ShootObjectToTarget()
    {
        var getter = spawner.SpawnFromGetterPool(spawnObject, transform.position, Quaternion.identity);
        if(getter.TryGetComponent(out ShootingObject shootingObject))
        {
            shootingObject.speed = objectSpeed;
            shootingObject.Shoot(DirectionAsVector3AtPosition());
        }
    }

    private Vector3 DirectionAsVector3AtPosition()
    {
        return new Vector3(targetDirection.x, targetDirection.y) * distance + transform.position;
    }

    private void OnDrawGizmos()
    {
        if(targetDirection != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, DirectionAsVector3AtPosition());
        }
    }
}
