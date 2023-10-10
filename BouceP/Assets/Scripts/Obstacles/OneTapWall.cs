using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneTapWall : MonoBehaviour
{
    private bool isActive = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (isActive && collision.transform.tag == "Projectile" && GameSystem.instance.GetState().ToString() == "Resolution")
        {
            // Deactivate the object
            gameObject.SetActive(false);
            isActive = false;
        }
    }

    public void MakeActive()
    {
        var oneTapWalls = GameObject.FindObjectsOfType<OneTapWall>();
        foreach (var oneTapWall in oneTapWalls)
        {
            if (!oneTapWall.isActive)
            {

                // Activate the object
                oneTapWall.gameObject.SetActive(true);
                oneTapWall.isActive = true;
            }
        }
    }
}
