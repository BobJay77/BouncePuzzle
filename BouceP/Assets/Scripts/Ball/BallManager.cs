using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] private AudioCollection bounceSounds = null;
    
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

            AudioManager.instance.PlayOneShotSound(bounceSounds.audioGroup,
                                                   bounceSounds.audioClip, 
                                                   transform.position,
                                                   bounceSounds.volume,
                                                   bounceSounds.spatialBlend,
                                                   bounceSounds.priority);


            GameSystem.instance.TriggerVFX(GameSystem.instance.hitPrefab);

            //if (GameSystem.instance.hitPrefab != null)
            //{
            //    var hitVFX = Instantiate(GameSystem.instance.hitPrefab, GameSystem.instance.playerBall.transform.position, Quaternion.identity) as GameObject;

            //    var ps = hitVFX.GetComponent<ParticleSystem>();
            //    if (ps == null)
            //    {
            //        var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
            //        Destroy(hitVFX, psChild.main.duration);
            //    }
            //    else
            //        Destroy(hitVFX, ps.main.duration);
            //}
        }

        else if (collision.gameObject.tag == "Spike")
        {
            GameSystem.instance.actionText.text = "Hit Spike";
            GameSystem.instance.roundEnded = true;
        }
    }
}
