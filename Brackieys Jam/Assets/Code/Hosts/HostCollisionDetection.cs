using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostCollisionDetection : MonoBehaviour
{
    [SerializeField] private BaseHost host;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        host.HandleCollisonEnter(collision);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        host.HandleTriggerEnter(collider);
    }
}
