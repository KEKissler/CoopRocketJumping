using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
    public GameObject projectile, projectileParent;
    public float jumpForce, groundedCheckDist, deadZoneThreshold, walkSpeed, airWalkSpeed, airWalkSpeedThreshold, percentVelocityLossPerSecond, explosionRadius, explosionForce, fireRate;
    public int numUpdatesToIgnoreGroundedCheck = 5;
    public SpriteRenderer rocket1, rocket2, rocket3;
    public Color active, inactive;
    private float timeSinceLastProjectile;
    private int groundedCheckReset;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private int numRocketsLeft = 3;
    private bool hasFired = false;
    private Quaternion gunRotation = new Quaternion();//temp variable reset every frame that is the same as the transform rotation
	// Use this for initialization
	void Start () {
        groundedCheckReset = numUpdatesToIgnoreGroundedCheck;
        rb = GetComponent<Rigidbody2D>();
        timeSinceLastProjectile = fireRate;
        rocket1.color = active;
        rocket2.color = active;
        rocket3.color = active;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        rb.velocity = (1 - Mathf.Clamp(percentVelocityLossPerSecond, 0, 1)) * rb.velocity;// * Time.deltaTime;

        Debug.Log(rb.velocity);
        if (Vector2.Distance(new Vector2(), new Vector2(Input.GetAxis("Joystick_2_x"), Input.GetAxis("Joystick_2_y"))) >= deadZoneThreshold)//use circle and own deadzone calc to make circular deadzone
        {
            //determine the angle for the gun to be facing according to player input iff player gave strong enough input
            gunRotation.eulerAngles = new Vector3(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(Input.GetAxis("Joystick_2_x"), Input.GetAxis("Joystick_2_y")));
            transform.rotation = gunRotation;
        }
        //manage grounded state via raycast down. Also a timer in terms of calls to this function that is the cooldown for doing that raycast check
        //prevents player being allowed to double jump due to being deemed grounded the frame after jumping and still being close to the ground
        RaycastHit2D groundedCheck = Physics2D.Raycast((Vector2)(transform.position), Vector2.down, groundedCheckDist, layerMask: 13);
        if (!isGrounded){
            if (--groundedCheckReset <= 0 && groundedCheck.collider != null) {
               // Debug.Log(test.collider.gameObject.name);
                isGrounded = true;
                rocket1.color = active;
                rocket2.color = active;
                rocket3.color = active;
                numRocketsLeft = 3;
            }
        }
        else if (groundedCheck.collider == null)
        {
            isGrounded = false;
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
            if (Input.GetAxis("Joystick_1_x") != 0)
            {
                //determine if input is trying to add to velocity rn or not
                if (rb.velocity.x * Input.GetAxis("Joystick_1_x") > 0)//dot product between velocity and (Input.GetAxis("Joystick_1_x"))
                {
                    //in same direction as velocity, so set velocity to be walk speed if and only if the player is already moving slower than that speed
                    if (rb.velocity.magnitude <= walkSpeed)
                    {
                        //right
                        if (Input.GetAxis("Joystick_1_x") > 0)
                        {
                            //rb.AddForce(walkSpeed * Vector2.right, ForceMode2D.Force);
                            rb.velocity = walkSpeed * Vector2.right;
                        }
                        //left
                        else
                        {
                            //rb.AddForce(walkSpeed * Vector2.left, ForceMode2D.Force);
                            rb.velocity = walkSpeed * Vector2.left;
                        }
                    }
                }
                else
                {
                    //given input is in opposite direction as velocity, so if they are fast add force opposite them to slow them down, if they are slow, just set it
                    if (rb.velocity.magnitude <= walkSpeed)
                    {
                        //slow movement
                        //right
                        if (Input.GetAxis("Joystick_1_x") > 0)
                        {
                            //rb.AddForce(walkSpeed * Vector2.right, ForceMode2D.Force);
                            rb.velocity = walkSpeed * Vector2.right;
                        }
                        //left
                        else
                        {
                            //rb.AddForce(walkSpeed * Vector2.left, ForceMode2D.Force);
                            rb.velocity = walkSpeed * Vector2.left;
                        }
                    }else
                    {
                        //fast movement
                        //right
                        if (Input.GetAxis("Joystick_1_x") > 0)
                        {
                            rb.AddForce(walkSpeed * Vector2.right, ForceMode2D.Force);
                            //rb.velocity = walkSpeed * Vector2.right;
                        }
                        //left
                        else
                        {
                            rb.AddForce(walkSpeed * Vector2.left, ForceMode2D.Force);
                            //rb.velocity = walkSpeed * Vector2.left;
                        }
                    }
                }
            }
        }
        else
        //player is not grounded
        {
            //determine if input at all
            if (Input.GetAxis("Joystick_1_x") != 0)
            {
                //determine if input is trying to add to velocity rn or not
                //if not in same direction or velocity is below threshold
                if (rb.velocity.x * Input.GetAxis("Joystick_1_x") <= 0 || Mathf.Abs(rb.velocity.x) < airWalkSpeedThreshold)
                {
                    //right
                    if (Input.GetAxis("Joystick_1_x") > 0)
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
        }
        //managing fireRate stuff
        if(timeSinceLastProjectile < fireRate)
        {
            timeSinceLastProjectile += Time.deltaTime;
        }
        //shooting projectiles
        if (Input.GetAxis("RightTrigger") != 0 && numRocketsLeft > 0)
        {
            if (timeSinceLastProjectile >= fireRate && !hasFired)
            {
                hasFired = true;
                //Debug.Log("angle: " + (this.transform.rotation.eulerAngles.z - 90) + "\nexpanded vector:" + new Vector2(Mathf.Cos(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90)), Mathf.Sin(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90))));
                RaycastHit2D test = Physics2D.Raycast((Vector2)(transform.position), new Vector2(Mathf.Cos(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90)), Mathf.Sin(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90))), distance: Mathf.Infinity, layerMask: 13);
                if (!isGrounded)
                {
                    
                    if (numRocketsLeft == 1)
                        rocket1.color = inactive;
                    if (numRocketsLeft == 2)
                        rocket2.color = inactive;
                    if (numRocketsLeft == 3)
                        rocket3.color = inactive;

                }
                if (test.collider != null)
                {

                    //Debug.Log(test.collider.gameObject.name + "   " + Vector2.Distance((Vector2)(test.transform.position), (Vector2)transform.position));
                    projectile.transform.position = new Vector3(test.point.x, test.point.y, projectile.transform.position.z);
                    timeSinceLastProjectile = 0;

                    if (Vector2.Distance(test.point, (Vector2)transform.position) < explosionRadius && numRocketsLeft > 0)
                    {
                        if (isGrounded)
                            numRocketsLeft += 1;
                        isGrounded = false;
                        groundedCheckReset = numUpdatesToIgnoreGroundedCheck;
                        rb.AddForce(-explosionForce * (test.point - (Vector2)this.transform.position).normalized, ForceMode2D.Impulse);
                    }

                }
                if(!isGrounded)
                numRocketsLeft -= 1;
            }
        }else
        {
            hasFired = false;
        }
    }
}
