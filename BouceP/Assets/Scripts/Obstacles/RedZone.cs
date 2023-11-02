using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class RedZone : MonoBehaviour
{
    private Material redZoneMaterial;
    public float    timer = 0f;

    [SerializeField] private float maxTimer = 3f;
    [SerializeField] private float fadeAwayMultiplyer = 10f;
    private RedZone[] redzones;


    public bool reset = false;

    // Start is called before the first frame update
    void Start()
    {
        redZoneMaterial = gameObject.GetComponent<MeshRenderer>().material;
        redzones = GameObject.FindObjectsOfType<RedZone>();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= maxTimer)
        {
            Color newcolor = redZoneMaterial.GetColor("_BorderColor");

            timer += Time.deltaTime;

            newcolor.a = newcolor.a - (Time.deltaTime * fadeAwayMultiplyer * newcolor.a * (timer / maxTimer));
            //redZoneMaterial.color = newcolor;
            redZoneMaterial.SetColor("_BorderColor", newcolor);
            redZoneMaterial.SetColor("_HexColor", newcolor);
        }

        if(reset)
        {
            ResetRedZone();
            reset = false;
        }


        if (GameSystem.instance.GetState() != null && GameSystem.instance.GetState().ToString() == "WinLose")
        {
            if(redzones != null)
            {
                foreach (RedZone redzone in redzones)
                {
                    redzone.GetComponent<Collider>().enabled = true;
                }
            }
            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Projectile")
        {
            ResetRedZone();
        }
    }

    public void ResetRedZone()
    {
        var redzones = GameObject.FindObjectsOfType<RedZone>();

        foreach (RedZone redzone in redzones)
        {
            redzone.GetComponent<Collider>().enabled = true;
            redzone.GetComponent<BoxCollider>().isTrigger = false;
            Color newcolor = redzone.redZoneMaterial.GetColor("_BorderColor");
            redzone.timer = 0;
            newcolor.a = 1f;
            redzone.redZoneMaterial.SetColor("_BorderColor", newcolor);
            redzone.redZoneMaterial.SetColor("_HexColor", newcolor);
        }
    }
}
