using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Teleporter exitTeleporter; // Reference to the exit teleporter
    [SerializeField] private BoxCollider parentCollider;
    private bool isTeleported = false; // Track whether the projectile has been teleported
    private Vector3 currentVel = Vector3.zero;

    // This function is called when a projectile enters the teleporter
    private void OnTriggerEnter(Collider projectile)
    {
        // Check if not already teleported
        if (!isTeleported && projectile.CompareTag("Projectile")) 
        {
            Debug.LogError("ONTRIGGERENTER " + gameObject.ToString());
            // Mark the projectile as teleported
            isTeleported = true; 
            exitTeleporter.isTeleported = true;
            parentCollider.isTrigger = true;
            exitTeleporter.parentCollider.isTrigger = true;

            // Save velocity and set to zero
            currentVel = projectile.gameObject.GetComponent<Rigidbody>().velocity;
            projectile.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

            Vector3 originalScale = projectile.gameObject.transform.localScale;

            projectile.gameObject.GetComponentInParent<ScaleToScreenSize>().enabled = false;

            projectile.GetComponent<Collider>().isTrigger = true;

            LeanTween.scale(projectile.gameObject, new Vector3(0, 0, 0), 0.3f).setOnComplete(delegate ()
            {
                projectile.transform.position = exitTeleporter.transform.position;

                LeanTween.scale(projectile.gameObject, originalScale, 0.3f).setOnComplete(delegate ()
                {
                    projectile.gameObject.GetComponentInParent<ScaleToScreenSize>().enabled = true;

                    Vector3 exitDirection = exitTeleporter.transform.up;
                    Vector3 exitVelocity = exitDirection.normalized * currentVel.magnitude;
                    projectile.GetComponent<Rigidbody>().velocity = exitVelocity;
                });
            });
        }
    }

    private void OnTriggerExit(Collider projectile)
    {
        if (!exitTeleporter.isTeleported && projectile.CompareTag("Projectile"))
        {
            projectile.GetComponent<Collider>().isTrigger = false;
        }
            parentCollider.isTrigger = false;
            isTeleported = false;
        Debug.LogError("ONTRIGGER EXIT " + gameObject.ToString());
    }
}

