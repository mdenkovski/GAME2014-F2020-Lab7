using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpossumBehaviour : MonoBehaviour
{

    public float runForce;
    private Rigidbody2D rigidbody2D;

    public Transform LookAheadPoint;
    public LayerMask collisionGroundLayer;
    public Transform LookInFrontPoint;
    public LayerMask collisionWallLayer;
    public bool isGroundAhead;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _LookInFront();
        _LookAhead();
        _Move();
    }

    private void _LookInFront()
    {

        if (Physics2D.Linecast(transform.position, LookInFrontPoint.position, collisionWallLayer))
        {
            _FlipX();
        }
        Debug.DrawLine(transform.position, LookInFrontPoint.position, Color.red);
    }

    private void _LookAhead()
    {
        isGroundAhead = Physics2D.Linecast(transform.position, LookAheadPoint.position, collisionGroundLayer);
        Debug.DrawLine(transform.position, LookAheadPoint.position, Color.green);


    }

    private void _Move()
    {
        if(isGroundAhead)
        {
            rigidbody2D.AddForce(Vector2.left * runForce * Time.deltaTime * transform.localScale.x);
            rigidbody2D.velocity *= 0.9f;

        }
        else
        {
            _FlipX();
        }
    }

   private void _FlipX()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }


  
}
