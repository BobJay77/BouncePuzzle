using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPair : MonoBehaviour
{
    [SerializeField] private WallPair otherWall;
    public Material wallMaterial = null;
    [SerializeField] private float isOn = 1f;
    [SerializeField] private bool isFirst = true;



    private void Start()
    {
        wallMaterial = GetComponent<MeshRenderer>().material;
        Switch();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GameSystem.instance.GetState().ToString() == "Resolution")
        {
            if (collision.transform.tag == "Projectile")
            {
                otherWall.GetComponent<Collider>().isTrigger = false;
                //GetComponent<Collider>().isTrigger = true;
                otherWall.isOn = 1f;
                isOn = 0.3f;
                otherWall.Switch();
                LeanTween.move(gameObject, gameObject.transform.position, 0.1f).setEase(LeanTweenType.linear).setOnComplete(() => { Switch(); });

            }
        }
    }

    public void ResetWallPair()
    {
        if (isFirst)
        {
            GetComponent<Collider>().isTrigger = false;
            otherWall.GetComponent<Collider>().isTrigger = true;
            isOn = 1f;
            otherWall.isOn = 0.3f;
            otherWall.Switch();
            Switch();
        }
    }

    public void Switch()
    {
        if (isOn >= 1)
        {
            GetComponent<Collider>().isTrigger = false;
        }

        else
        {
            GetComponent<Collider>().isTrigger = true;
        }
        Color newBordercolor = wallMaterial.GetColor("_BorderColor");
        Color newWaveColor = wallMaterial.GetColor("_WaveColor");
        newBordercolor.a = isOn;
        newWaveColor.a = isOn;
        wallMaterial.SetColor("_BorderColor", newBordercolor);
        wallMaterial.SetColor("_WaveColor", newWaveColor);
    }
}
