using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Teleporter : MonoBehaviour
{
    public GameObject otherPortal;
    private Vector3 portalNormal;
    public bool isTeleporting = false;

    private void Start()
    {
        CalculatePortalNormal();
    }

    private void CalculatePortalNormal()
    {
        // Calculate the normal vector of the portal plane
        portalNormal = transform.up;
    }

    private void TeleportObject(Transform objToTeleport)
    {
        Vector3 offset = objToTeleport.position - transform.position;

        // Calculate the exit position and rotation
        Vector3 exitPosition = otherPortal.transform.position;
        Quaternion rotationChange = Quaternion.FromToRotation(transform.up, otherPortal.transform.up);

        // Teleport the object to the exit portal
        objToTeleport.position = exitPosition + rotationChange * offset;

        // Apply the appropriate rotation
        objToTeleport.rotation = rotationChange * objToTeleport.rotation;

        // Calculate the new velocity direction after teleportation
        Vector3 originalVelocity = objToTeleport.GetComponent<Rigidbody>().velocity; // Assuming you have a Rigidbody on the object
        Vector3 newVelocity = rotationChange * originalVelocity;

        objToTeleport.GetComponent<Rigidbody>().velocity = newVelocity; // Update the velocity
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile") && !isTeleporting)
        {

            // Teleport the object that enters the portal
            otherPortal.GetComponent<Teleporter>().isTeleporting = true;
            TeleportObject(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        isTeleporting = false;
    }
}

