using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportPlayer : MonoBehaviour
{
    [SerializeField] string placeToTransport;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            GameManager.Instance.sceneLoader.LoadNextPlace(placeToTransport);
        }
    }
}
