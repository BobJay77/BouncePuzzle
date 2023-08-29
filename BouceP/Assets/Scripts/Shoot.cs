using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Shoot : MonoBehaviour
{

    [SerializeField] GameObject ball;
    [SerializeField] float multiplier;
    [SerializeField] float maxDragDistance = 3;
    Vector3 initPos;

    //get the initial positon of the holder before the drag starts
    void OnMouseDown()
    {
        initPos = transform.position;
    }

    void OnMouseDrag()
    {
        ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        if (Vector3.Distance(transform.position, initPos) < maxDragDistance)
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
    }

    void OnMouseUp()
    {
        
        ball.GetComponent<Rigidbody>().AddForce((initPos - transform.position) * Vector3.Distance(transform.position, initPos) * multiplier);

        this.transform.position = initPos;
    }
}