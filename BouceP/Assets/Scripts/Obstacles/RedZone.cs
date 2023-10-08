using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class RedZone : MonoBehaviour
{
    public Material redZoneMaterial;
    public float    timer = 0f;

    [SerializeField] private float maxTimer = 3f;
    [SerializeField] private float fadeAwayMultiplyer = 10f;

    // Start is called before the first frame update
    void Start()
    {
        redZoneMaterial = gameObject.GetComponent<MeshRenderer>().material;
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
        GetComponent<BoxCollider>().isTrigger = false;
        RedZone tempRedZone = GetComponent<RedZone>();
        Color newcolor = tempRedZone.redZoneMaterial.GetColor("_BorderColor");
        tempRedZone.timer = 0;
        newcolor.a = 1f;
        tempRedZone.redZoneMaterial.SetColor("_BorderColor", newcolor);
        tempRedZone.redZoneMaterial.SetColor("_HexColor", newcolor);
    }
}
