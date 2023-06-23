using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] float rotationSpeedMin;
    [SerializeField] float rotationSpeedMax;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(0,0, Random.Range(rotationSpeedMin, rotationSpeedMax) * Time.deltaTime);
    }


    
}
