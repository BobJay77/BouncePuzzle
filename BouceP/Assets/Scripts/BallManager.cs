using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision collision)
    {
        if (GameSystem.instance.roundEnded) return;
        if (GameSystem.instance.GetState().ToString() != "Resolution") return;

        if(collision.gameObject.tag == "Goal")
        {
            GameSystem.instance.roundEnded = true;
            GameSystem.instance.hitGoal = true;
        }
        else if(collision.gameObject.tag == "Wall")
        {
            GameSystem.instance.currentBounces++;
            GameSystem.instance.bouncesGoal--; 
        }

        else if (collision.gameObject.tag == "Spike")
        {
            GameSystem.instance.actionText.text = "Hit Spike";
            GameSystem.instance.roundEnded = true;
        }
    }
}
