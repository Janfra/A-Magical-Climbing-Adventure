using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ShootingObject : MonoBehaviour, ObjectPooler.IPoolGetComponent
{
    [Header("Dependencies")]
    [SerializeField]
    private Rigidbody2D objectRigidbody;

    [Header("Configuration")]
    public float speed;

    private Vector2 target;
    private Vector3 movementDirection;
    private bool isShot;

    public int Index { get; set; }

    private void Awake()
    {
        if(objectRigidbody == null)
        {
            objectRigidbody = GetComponent<Rigidbody2D>();
        }
    }

    private void FixedUpdate()
    {
        if (isShot)
        {
            MoveToTarget();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Shooting object touched an object");
        gameObject.SetActive(false);
    }

    public void Shoot(Vector2 _target)
    {
        isShot = true;
        target = _target;
        movementDirection = target - new Vector2(transform.position.x, transform.position.y);
        movementDirection.Normalize();
    } 

    protected void MoveToTarget()
    {
        objectRigidbody.MovePosition(transform.position + movementDirection * speed * Time.fixedDeltaTime);
        float direction = Vector2.Dot(movementDirection, (target - new Vector2(transform.position.x, transform.position.y)).normalized);

        if (direction < 0.9)
        {
            // Debug.Log($"Shooting object at target, position: {transform.position} target: {target}, direction result: {direction}");
            gameObject.SetActive(false);
        }
    }
}
