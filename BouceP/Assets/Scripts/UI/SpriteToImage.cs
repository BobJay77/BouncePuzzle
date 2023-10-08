using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteToImage : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent<Animator>().SetBool("activated", true);

    }

    private void OnDisable()
    {
        GetComponent<Animator>().SetBool("activated", false);
    }

    private void OnAnimatorMove()
    {
        GetComponent<Image>().sprite = GetComponent<SpriteRenderer>().sprite;
    }
     
   
}
