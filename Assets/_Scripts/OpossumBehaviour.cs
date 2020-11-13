using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RampDirection
{
    UP,
    DOWN
};

public class OpossumBehaviour : MonoBehaviour
{

    public float runForce;
    private Rigidbody2D rigidbody2D;

    public Transform LookAheadPoint;
    public LayerMask collisionGroundLayer;
    public Transform LookInFrontPoint;
    public LayerMask collisionWallLayer;
    public Transform MidBodyPoint;
    public bool isGroundAhead;
    public Vector2 direction;
    public bool onRamp;
    public RampDirection rampDirection;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        direction = Vector2.left;
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
        var wallHit = Physics2D.Linecast(transform.position, LookInFrontPoint.position, collisionWallLayer);
        if (wallHit)
        {
            _FlipX();
            //if (!wallHit.collider.CompareTag("Ramps"))
            //{
            //    _FlipX();
            //    rampDirection = RampDirection.DOWN;

            //}
            //else
            //{
            //    rampDirection = RampDirection.UP;
            //}
        }
        Debug.DrawLine(transform.position, LookInFrontPoint.position, Color.red);
    }

    private void _LookAhead()
    {
        RaycastHit2D groundHit = Physics2D.Linecast(transform.position, LookAheadPoint.position, collisionGroundLayer);
        if (groundHit)
        {

            //vertical raycast to detect if ramp is below us. makes transitions smoother to do vertical line instead of from look ahead point
            RaycastHit2D verticalHit = Physics2D.Linecast(MidBodyPoint.position, MidBodyPoint.position + Vector3.down * 3, collisionGroundLayer);
            if (verticalHit.collider.CompareTag("Ramps")) // there is a ramp below us
            {
                Vector2 testLine2D = new Vector2(MidBodyPoint.position.x - verticalHit.point.x, MidBodyPoint.position.y - verticalHit.point.y);
                float angle = Vector2.SignedAngle(testLine2D, verticalHit.normal);

                //when direction.x < 0 we are moving left

                if (angle < 0 && direction.x < 0) //moving up a ramp from the right
                {
                    rampDirection = RampDirection.UP;
                }
                else if (angle < 0 && direction.x > 0) // moving down a ramp from the left
                {
                    rampDirection = RampDirection.DOWN;
                }
                else if (angle > 0 && direction.x < 0) // moving down a ramp from the right
                {
                    rampDirection = RampDirection.DOWN;
                }
                else if (angle > 0 && direction.x > 0) // moving up a ramp from the left
                {
                    rampDirection = RampDirection.UP;
                }

                //debug line for the normal based on the colision point
                Debug.DrawLine(verticalHit.point, verticalHit.point + verticalHit.normal * 3, Color.red);

                //debug line for the vertical check line
                Debug.DrawLine(MidBodyPoint.position, MidBodyPoint.position + Vector3.down * 3, Color.yellow);

                Debug.Log(angle);
                Debug.Log("Ramps!!");
                onRamp = true;
            }
            else if (verticalHit.collider.CompareTag("Platforms"))
            {
                Debug.Log("Platforms!!");
                onRamp = false;
            }

            ////old check that would be jittery
            //if (groundHit.collider.CompareTag("Ramps"))
            //{
            //    Vector3 testAngle = (transform.position - LookAheadPoint.position);
            //    Vector2 testAngle2D = new Vector2(testAngle.x, testAngle.y);
            //    float angle = Vector2.SignedAngle(testAngle2D, groundHit.normal);
            //    if(angle > 0) //going up
            //    {
            //        rampDirection = RampDirection.UP;
            //    }
            //    else //going down
            //    {
            //        rampDirection = RampDirection.DOWN;
            //    }
            //    Debug.Log(angle);
            //    Debug.Log("Ramps!!");
            //    onRamp = true;
            //}
            //else if (groundHit.collider.CompareTag("Platforms"))
            //{
            //    Debug.Log("Platforms!!");
            //    onRamp = false;
            //}
            isGroundAhead = true;
        }
        else
        {
            isGroundAhead = false;
        }

       

        //debug line for the look ahead point
        Debug.DrawLine(transform.position, LookAheadPoint.position, Color.green);
        
        


    }

    private void _Move()
    {
        if(isGroundAhead)
        {
            rigidbody2D.AddForce(direction * runForce * Time.deltaTime);
            if(onRamp)
            {
                if(rampDirection == RampDirection.UP)
                {
                    rigidbody2D.AddForce(Vector2.up * runForce * 0.5f * Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0.0f, 0.0f, 26.0f * direction.x);
                }
                else // ramp direction is down
                {
                    transform.rotation = Quaternion.Euler(0.0f, 0.0f, -26.0f * direction.x);
                } 
            }
            else //not on ramp
            {
                transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            }
            rigidbody2D.velocity *= 0.9f;

        }
        else
        {
            if(!onRamp)
            {
            _FlipX();

            }
        }
    }

   private void _FlipX()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        direction.x *=-1; //flip our direction
    }


  
}
