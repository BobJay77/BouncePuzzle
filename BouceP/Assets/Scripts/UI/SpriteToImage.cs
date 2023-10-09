using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteToImage : MonoBehaviour
{
    public bool onCanvas = true;
    void OnEnable()
    {
        if(onCanvas)
            GetComponent<Animator>().SetBool("activated", true);

    }

    private void OnDisable()
    {
        if(onCanvas)
            GetComponent<Animator>().SetBool("activated", false);
    }

    private void OnAnimatorMove()
    {
        if(onCanvas)
            GetComponent<Image>().sprite = GetComponent<SpriteRenderer>().sprite;
    }
     
    public void ActivateGifAnim(bool activated)
    {
        GetComponent<Animator>().SetBool("activated", activated);
    }

}
