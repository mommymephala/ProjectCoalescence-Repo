using System;
using HorrorEngine;
using UnityEngine;

public class SeamlessTeleporter : MonoBehaviour
{
    public Transform teleportTarget;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = teleportTarget.position;
        }
    }
}