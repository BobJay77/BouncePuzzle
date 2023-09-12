using System.Collections;
using System.Collections.Generic;
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
            Color newcolor = redZoneMaterial.color;
            
            timer += Time.deltaTime;
            
            newcolor.a = newcolor.a - (Time.deltaTime * fadeAwayMultiplyer * newcolor.a * (timer / maxTimer));
            redZoneMaterial.color = newcolor;
        }

        //else
        //{
        //    timer = 0;
            
        //    Color newcolor = redZoneMaterial.color;
        //    newcolor.a = 1f;

        //    redZoneMaterial.color = newcolor;
        //}
    }
}
