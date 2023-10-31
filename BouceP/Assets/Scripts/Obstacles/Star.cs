using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Star : MonoBehaviour
{
    public float minScale = 0.5f; // Minimum scale factor
    public float maxScale = 1.5f; // Maximum scale factor
    public float duration = 0.5f; // Duration of the twinkle animation
    public LeanTweenType leanTweenType;
    private LTDescr tween;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Collider>().enabled = true;
        Twinkle();

        GameSystem.instance.StarCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Projectile" && GameSystem.instance.GetState().ToString() == "Resolution")
        {
            GameSystem.instance.StarCounter++;
            this.GetComponent<Collider>().enabled = false;

            LeanTween.cancel(gameObject, tween.id);
            LeanTween.scale(gameObject, new Vector3(0, 0, 0), 0.1f).setEase(leanTweenType);
        }
    }

    void Twinkle()
    {
        // Scale the star up
        tween = LeanTween.scale(gameObject, new Vector3(maxScale, maxScale, maxScale), duration).setEase(leanTweenType)
            .setOnComplete(() =>
            {
                // Scale the star back down
                tween = LeanTween.scale(gameObject, new Vector3(minScale, minScale, minScale), duration).setEase(leanTweenType)
                    .setOnComplete(() =>
                    {
                        // Call Twinkle function recursively to create a continuous twinkling effect
                        if(this.GetComponent<Collider>().enabled)
                        {
                            Twinkle();
                        }
                    });
            });

    }

    public void Restart()
    {
        LeanTween.cancel(gameObject, tween.id);
        this.GetComponent<Collider>().enabled = true;
        GameSystem.instance.StarCounter = 0;
        Twinkle();
    }

    private void OnDestroy()
    {
        if(GameSystem.instance.StarCounter != 0) GameSystem.instance.StarCounter = 0;
    }
}
