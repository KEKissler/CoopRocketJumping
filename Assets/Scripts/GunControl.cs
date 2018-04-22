using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GunControl : NetworkBehaviour {
    public GameObject projectile, p1_indicator;
    public float jumpForce, groundedCheckDist, deadZoneThreshold, walkSpeed, airWalkSpeed, airWalkSpeedThreshold, percentVelocityLossPerSecond, fireRate;
    public int numUpdatesToIgnoreGroundedCheck = 5;
    public SpriteRenderer rocket1, rocket2, rocket3; 
    public Color active, inactive;
    private float timeSinceLastProjectile;
    private int groundedCheckReset;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private int numRocketsLeft = 3;
    private bool hasFired = false;
    private GunInput input;
    // Use this for initialization

    public override void OnStartLocalPlayer() // the player client is controlling will have a tiny arrow on top
    {
        GameObject p1 = Instantiate(p1_indicator, this.transform);
        p1.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().assignObjToFollow(transform);
    }

    void Start () {
        groundedCheckReset = numUpdatesToIgnoreGroundedCheck;
        rb = transform.GetComponent<Rigidbody2D>();
        timeSinceLastProjectile = fireRate;
        rocket1.color = active;
        rocket2.color = active;
        rocket3.color = active;
        input = GetComponent<GunInput>();

        Physics2D.IgnoreLayerCollision(8, 8);//objects on layer of player cannot collide with one another, meaning players cannot bump into one another
    }
	
	// Update is called once per frame
	void Update () {

        if (!isLocalPlayer)
        {
            return;
        }

        rb.velocity = (1 - Mathf.Clamp(percentVelocityLossPerSecond, 0, 1)) * rb.velocity;
        //Debug.Log(Vector2.Distance(new Vector2(), new Vector2(Input.GetAxis("Joystick_2_x"), Input.GetAxis("Joystick_2_y")))+"\n" + new Vector2(Input.GetAxis("Joystick_2_x"), Input.GetAxis("Joystick_2_y")).magnitude);
        
        if (input.inputChange(deadZoneThreshold))//use circle and own deadzone calc to make circular deadzone
        {
            Rotate_gun();
            
        }
        //manage grounded state via raycast down. Also a timer in terms of calls to this function that is the cooldown for doing that raycast check
        //prevents player being allowed to double jump due to being deemed grounded the frame after jumping and still being close to the ground
        RaycastHit2D groundedCheck = Physics2D.Raycast((Vector2)(transform.position), Vector2.down, groundedCheckDist, layerMask: 13);
        if (!isGrounded){
            if (--groundedCheckReset <= 0 && groundedCheck.collider != null) {
                // Debug.Log(test.collider.gameObject.name);
                Cmdrocket_allactive();
            }
        }
        else if (groundedCheck.collider == null)
        {
            isGrounded = false;
        }
        //the jump itself
        if (isGrounded && input.getJump())
        {
            isGrounded = false;
            groundedCheckReset = numUpdatesToIgnoreGroundedCheck;
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        }

        float xAxis = input.getXAxis();
        //left right movement both for ground and in air
        if (isGrounded)
        {
            //determine if input at all
            if (xAxis != 0)
            {
                //determine if input is trying to add to velocity rn or not
                if (rb.velocity.x * xAxis > 0)//dot product between velocity and (xAxis)
                {
                    //in same direction as velocity, so set velocity to be walk speed if and only if the player is already moving slower than that speed
                    if (rb.velocity.magnitude <= walkSpeed)
                    {
                        //right
                        if (xAxis > 0)
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
                        if (xAxis > 0)
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
                    else
                    {
                        //fast movement
                        //right
                        if (xAxis > 0)
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
            if (xAxis != 0)
            {
                //determine if input is trying to add to velocity rn or not
                //if not in same direction or velocity is below threshold
                if (rb.velocity.x * xAxis <= 0 || Mathf.Abs(rb.velocity.x) < airWalkSpeedThreshold)
                {
                    //right
                    if (xAxis > 0)
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
        if (timeSinceLastProjectile < fireRate)
        {
            timeSinceLastProjectile += Time.deltaTime;
        }
        //shooting projectiles
        if (input.getFire() && numRocketsLeft > 0)
        {
            if (timeSinceLastProjectile >= fireRate && !hasFired)
            {
                timeSinceLastProjectile = 0;
                CmdFire();
            }
        }else
        {
            hasFired = false;
        }
    }



    void Rotate_gun()
    {
        transform.GetChild(0).rotation = Quaternion.Euler(input.getRotation());
    }


    [Command]
    void Cmdrocket_allactive()
    {
        isGrounded = true;
        rocket1.color = active;
        rocket2.color = active;
        rocket3.color = active;
        numRocketsLeft = 3;
    }

    [Command]
    void CmdFire()
    {
        GameObject rocket = Instantiate(projectile);
        rocket.transform.position = this.transform.position;

        projectileController pC = rocket.GetComponent<projectileController>();
        pC.playerWhoFiredThis = transform;
        Debug.Log(transform.gameObject.name + " fired a rocket");

        pC.Fire(new Vector2(Mathf.Cos(Mathf.Deg2Rad * (this.transform.GetChild(0).rotation.eulerAngles.z - 90)), Mathf.Sin(Mathf.Deg2Rad * (this.transform.GetChild(0).rotation.eulerAngles.z - 90))));

        hasFired = true;
        

        NetworkServer.Spawn(rocket);
        //Debug.Log("angle: " + (this.transform.rotation.eulerAngles.z - 90) + "\nexpanded vector:" + new Vector2(Mathf.Cos(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90)), Mathf.Sin(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90))));
        //RaycastHit2D test = Physics2D.Raycast((Vector2)(transform.position), new Vector2(Mathf.Cos(Mathf.Deg2Rad * (this.transform.GetChild(0).rotation.eulerAngles.z - 90)), Mathf.Sin(Mathf.Deg2Rad * (this.transform.GetChild(0).rotation.eulerAngles.z - 90))), distance: Mathf.Infinity, layerMask: 13);
        if (!isGrounded)
        {

            if (numRocketsLeft == 1)
                rocket1.color = inactive;
            if (numRocketsLeft == 2)
                rocket2.color = inactive;
            if (numRocketsLeft == 3)
                rocket3.color = inactive;
            numRocketsLeft -= 1;
        }
        //if (!isGrounded)
          //  numRocketsLeft -= 1;
        //if (test.collider != null)

        //Debug.Log(test.collider.gameObject.name + "   " + Vector2.Distance((Vector2)(test.transform.position), (Vector2)transform.position));
        //projectile.transform.position = new Vector3(test.point.x, test.point.y, projectile.transform.position.z);


        //if (Vector2.Distance(test.point, (Vector2)transform.position) < explosionRadius && numRocketsLeft > 0)
        //if (isGrounded)
        //  numRocketsLeft += 1;



    }
    [ClientRpc]
    public void RpcApplyRocketForceToSelf(float explForce, Vector2 explosionCenter)
    {
        isGrounded = false;
        groundedCheckReset = numUpdatesToIgnoreGroundedCheck;
        rb.AddForce(-explForce * (explosionCenter - (Vector2)this.transform.position).normalized, ForceMode2D.Impulse);
    }
}
