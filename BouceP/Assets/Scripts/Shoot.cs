using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Shoot : MonoBehaviour
{

    [SerializeField] GameObject ball;
    [SerializeField] float multiplier;
    [SerializeField] float maxDragDistance = 3;

    [SerializeField] private bool shootMode = false;

    Vector3 initPos;

    //get the initial positon of the holder before the drag starts
    void OnMouseDown()
    {
        initPos = transform.position;
        shootMode = true;
    }

    void OnMouseDrag()
    {
        ball.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        if (Vector3.Distance(transform.position, initPos) < maxDragDistance)
        {
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            ball.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
        }
    }

    void OnMouseUp()
    {
        shootMode = false;
        ball.GetComponent<Rigidbody>().AddForce((initPos - transform.position) * Vector3.Distance(transform.position, initPos) * multiplier * Time.deltaTime);

        this.transform.position = initPos;
    }

    private void Update()
    {
        if (!shootMode)
        {
            this.transform.position = ball.transform.position;
        }
    }
}