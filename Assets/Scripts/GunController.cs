using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
    public GameObject projectile, projectileParent;
    public float jumpForce, groundedCheckDist, deadZoneThreshold, walkSpeed, airWalkSpeed, explosionRadius, explosionForce, fireRate;
    public int numUpdatesToIgnoreGroundedCheck = 5;
    private float timeSinceLastProjectile;
    private int groundedCheckReset;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private Quaternion gunRotation = new Quaternion();//temp variable reset every frame that is the same as the transform rotation
	// Use this for initialization
	void Start () {
        groundedCheckReset = numUpdatesToIgnoreGroundedCheck;
        rb = GetComponent<Rigidbody2D>();
        timeSinceLastProjectile = fireRate;
    }
	
	// Update is called once per frame
	void Update () {
        if (Vector2.Distance(new Vector2(), new Vector2(Input.GetAxis("Joystick_2_x"), Input.GetAxis("Joystick_2_y"))) >= deadZoneThreshold)//use circle and own deadzone calc to make circular deadzone
        {
            //determine the angle for the gun to be facing according to player input iff player gave strong enough input
            gunRotation.eulerAngles = new Vector3(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(Input.GetAxis("Joystick_2_x"), Input.GetAxis("Joystick_2_y")));
            transform.rotation = gunRotation;
        }
        //manage grounded state via raycast down. Also a timer in terms of calls to this function that is the cooldown for doing that raycast check
        //prevents player being allowed to double jump due to being deemed grounded the frame after jumping and still being close to the ground
        if (!isGrounded && --groundedCheckReset <= 0)
        {
            RaycastHit2D test = Physics2D.Raycast((Vector2)(transform.position), Vector2.down, groundedCheckDist, layerMask: 1);
            if (test.collider != null) {
               // Debug.Log(test.collider.gameObject.name);
                isGrounded = true;
            }
        }
        //the jump itself
        if (isGrounded && Input.GetKeyDown(KeyCode.Joystick1Button0))
        {
            isGrounded = false;
            groundedCheckReset = numUpdatesToIgnoreGroundedCheck;
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        }

        //left right movement both for ground and in air
        if (isGrounded)
        {
            //determine if input at all
            if(Input.GetAxis("Joystick_1_x") != 0)
            {
                //right
                if (Input.GetAxis("Joystick_1_x") > 0)
                {
                    rb.AddForce(walkSpeed * Vector2.right, ForceMode2D.Force);
                }
                //left
                else
                {
                    rb.AddForce(walkSpeed * Vector2.left, ForceMode2D.Force);
                }
            }
        }
        else
        {
            //determine if input at all
            if (Input.GetAxisRaw("Joystick_1_x") != 0)
            {
                //right
                if (Input.GetAxisRaw("Joystick_1_x") > 0)
                {
                    rb.AddForce(airWalkSpeed * Vector2.right, ForceMode2D.Force);
                }
                //left
                else
                {
                    rb.AddForce(airWalkSpeed * Vector2.left, ForceMode2D.Force);
                }
            }
        }
        //managing fireRate stuff
        if(timeSinceLastProjectile < fireRate)
        {
            timeSinceLastProjectile += Time.deltaTime;
        }
        //shooting projectiles
        if (Input.GetAxis("RightTrigger") != 0 && timeSinceLastProjectile >= fireRate)
        {
            //Debug.Log("angle: " + (this.transform.rotation.eulerAngles.z - 90) + "\nexpanded vector:" + new Vector2(Mathf.Cos(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90)), Mathf.Sin(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90))));
            RaycastHit2D test = Physics2D.Raycast((Vector2)(transform.position), new Vector2(Mathf.Cos(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90)), Mathf.Sin(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90))), distance: Mathf.Infinity, layerMask: 1);
            if (test.collider != null)
            {
                
                //Debug.Log(test.collider.gameObject.name + "   " + Vector2.Distance((Vector2)(test.transform.position), (Vector2)transform.position));
                projectile.transform.position = new Vector3(test.point.x, test.point.y, projectile.transform.position.z);
                timeSinceLastProjectile = 0;
                if (Vector2.Distance(test.point, (Vector2)transform.position) < explosionRadius)
                {
                    rb.AddForce(-explosionForce * (test.point - (Vector2)this.transform.position).normalized, ForceMode2D.Impulse);
                }
                
            }
        }
    }
}
