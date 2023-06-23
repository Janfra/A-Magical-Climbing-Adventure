using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCube : MonoBehaviour, ObjectPooler.IPoolGetComponent
{
    public int Index { get; set; }
    public Rigidbody2D Rigidbody;
}
