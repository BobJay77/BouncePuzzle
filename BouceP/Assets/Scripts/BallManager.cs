using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    
    private void OnCollisionEnter(Collision collision)
    {
        if (GameSystem.instance.roundEnded) return;

        if(GameSystem.instance.GetState().ToString() == "Resolution" && collision.gameObject.tag == "Wall" )
            GameSystem.instance.currentBounces++;


        if(collision.gameObject.tag == "Goal")
        {
            GameSystem.instance.roundEnded = true;
        }
        else
        {
            GameSystem.instance.bouncesGoal--; 
        }
    }
}
