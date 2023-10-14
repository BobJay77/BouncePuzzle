using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Teleporter : MonoBehaviour
{
    public Teleporter otherPortal;
    public BoxCollider parentCollider;
    public bool isTeleporting = false;

    private void TeleportObject(Transform objToTeleport)
    {
        // Change axis based on rotation
        Vector3 axis = Vector3.one;
        if (otherPortal.transform.rotation.x != 0) axis = Vector3.up;
        else axis = Vector3.down;

        Vector3 offset = (objToTeleport.position - transform.position) + (axis * otherPortal.transform.localScale.y * 0.6f);

        // Calculate the exit position and rotation
        Vector3 exitPosition = otherPortal.transform.position;
        Quaternion rotationChange = Quaternion.FromToRotation(transform.up, otherPortal.transform.up);

        objToTeleport.position = exitPosition + rotationChange * offset;
        objToTeleport.rotation = rotationChange * objToTeleport.rotation;

        // Calculate new velocity direction after teleportation
        Vector3 originalVelocity = objToTeleport.GetComponent<Rigidbody>().velocity; 
        Vector3 newVelocity = rotationChange * originalVelocity;

        objToTeleport.GetComponent<Rigidbody>().velocity = newVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile") && !isTeleporting)
        {
            parentCollider.isTrigger = true;
            otherPortal.parentCollider.isTrigger = true;

            // Teleport the Projectile that enters the portal
            otherPortal.GetComponent<Teleporter>().isTeleporting = true;
            TeleportObject(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        parentCollider.isTrigger = false;
        isTeleporting = false;
    }
}

