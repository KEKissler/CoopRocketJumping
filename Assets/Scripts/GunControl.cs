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
    [SyncVar(hook = "OnRocketNumChange")]
    private int numRocketsLeft = 3;
    private bool hasFired = false;
    private GunInput input;
    // Use this for initialization

    private Vector2 movement;
    RaycastHit2D groundedCheck;

    private static bool created = false;


    public override void OnStartLocalPlayer() // the player client is controlling will have a tiny arrow on top
    {
        GameObject p1 = Instantiate(p1_indicator, this.transform);
        p1.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 1, this.transform.position.z);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().assignObjToFollow(transform);

        //janks
        //GameObject Manager = GameObject.FindGameObjectWithTag("Network_Manager");
        //transform.SetParent(Manager.transform);
       
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
        Physics2D.IgnoreLayerCollision(8, 10);
        Physics2D.IgnoreLayerCollision(10, 10);
        Physics2D.IgnoreLayerCollision(8, 9);
        Physics2D.IgnoreLayerCollision(9, 13);
        Physics2D.IgnoreLayerCollision(8, 14);
        Physics2D.IgnoreLayerCollision(10, 11);
        Physics2D.IgnoreLayerCollision(11, 13);
        Physics2D.IgnoreLayerCollision(10, 14);
        Physics2D.IgnoreLayerCollision(15, 14);
        //projectiles ignore tpboxes
        Physics2D.IgnoreLayerCollision(9, 12, true);
        Physics2D.IgnoreLayerCollision(11, 12, true);
    }

    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }
        else
        {
            this.gameObject.layer = 8;
        }

        //rb.velocity = (1 - Mathf.Clamp(percentVelocityLossPerSecond, 0, 1)) * rb.velocity;
        //        Debug.Log(Mathf.Round(10*rb.velocity.magnitude)/10);
        //Debug.Log(Vector2.Distance(new Vector2(), new Vector2(Input.GetAxis("Joystick_2_x"), Input.GetAxis("Joystick_2_y")))+"\n" + new Vector2(Input.GetAxis("Joystick_2_x"), Input.GetAxis("Joystick_2_y")).magnitude);

        if (input.inputChange(deadZoneThreshold))//use circle and own deadzone calc to make circular deadzone
        {
            Rotate_gun();

        }
        //manage grounded state via raycast down. Also a timer in terms of calls to this function that is the cooldown for doing that raycast check
        //prevents player being allowed to double jump due to being deemed grounded the frame after jumping and still being close to the ground
        groundedCheck = Physics2D.Raycast((Vector2)(transform.position), Vector2.down, groundedCheckDist, layerMask: 13);


        if (groundedCheck.collider != null)
        {
            isGrounded = true;
            numRocketsLeft = 3;
            OnRocketNumChange(numRocketsLeft);
        }
        else
        {
            isGrounded = false;
        }

        //the jump itself
        if (false == true)//isGrounded && input.getJump())
        {
            isGrounded = false;
            groundedCheckReset = numUpdatesToIgnoreGroundedCheck;
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        }
        float xAxis = input.getXAxis();
        //left right movement both for ground and in air
        if (isGrounded)
        {
            movement.x = xAxis;
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
                if (!isGrounded)
                {
                    if (numRocketsLeft == 1)
                        rocket1.color = inactive;
                    if (numRocketsLeft == 2)
                        rocket2.color = inactive;
                    if (numRocketsLeft == 3)
                        rocket3.color = inactive;
                    if (numRocketsLeft > 0)
                        CmdFire(this.transform.position, transform.GetChild(0).rotation.eulerAngles);
                    numRocketsLeft--;
                }
                else
                {
                    CmdFire(this.transform.position, transform.GetChild(0).rotation.eulerAngles);
                }

            }
        }
        else
        {
            hasFired = false;
        }
    }
    

    private void FixedUpdate()
    {
        transform.Translate(new Vector3(walkSpeed * movement.x * Time.deltaTime, movement.y, 0));
    }

    void Rotate_gun()
    {
        transform.GetChild(0).rotation = Quaternion.Euler(input.getRotation());
    }
    
    private void OnRocketNumChange(int newRocketNum)
    {
        if (newRocketNum == 0)
            rocket1.color = inactive;
        if (newRocketNum == 1)
            rocket2.color = inactive;
        if (newRocketNum == 2)
            rocket3.color = inactive;
        if (newRocketNum == 3)
        {
            rocket1.color = active;
            rocket2.color = active;
            rocket3.color = active;
        }
    }

    [Command]
    void CmdFire(Vector3 firePos, Vector3 eulerAngles)
    {
        
        GameObject rocket = Instantiate(projectile, firePos, Quaternion.identity);
        
        //make rocket ignore collision with the player who fired it
        rocket.gameObject.layer = 9;
        Physics2D.IgnoreCollision(rocket.GetComponent<CircleCollider2D>(), transform.GetChild(0).GetComponent<CircleCollider2D>(), true);
        NetworkServer.Spawn(rocket);
        //     rocket.transform.position = firePos;
        //        rocket.transform.rotation = eulerAngles;

        projectileController pC = rocket.GetComponent<projectileController>();
        pC.velo = pC.speed * (new Vector2(Mathf.Cos(Mathf.Deg2Rad * (eulerAngles.z - 90)), Mathf.Sin(Mathf.Deg2Rad * (eulerAngles.z - 90)))).normalized;
        pC.playerWhoFiredThis = transform;
        pC.netIdOfWhoFiredThis = netId;
        //Debug.Log(transform.gameObject.name + " netId = " + netId + " fired a rocket");


         pC.Fire();

     //below is the old formula
      //  pC.Fire(new Vector2(Mathf.Cos(Mathf.Deg2Rad * (transform.GetChild(0).rotation.eulerAngles.z - 90)), Mathf.Sin(Mathf.Deg2Rad * (transform.GetChild(0).rotation.eulerAngles.z - 90))));
        hasFired = true;


        
        //Debug.Log("angle: " + (this.transform.rotation.eulerAngles.z - 90) + "\nexpanded vector:" + new Vector2(Mathf.Cos(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90)), Mathf.Sin(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90))));
        //RaycastHit2D test = Physics2D.Raycast((Vector2)(transform.position), new Vector2(Mathf.Cos(Mathf.Deg2Rad * (this.transform.GetChild(0).rotation.eulerAngles.z - 90)), Mathf.Sin(Mathf.Deg2Rad * (this.transform.GetChild(0).rotation.eulerAngles.z - 90))), distance: Mathf.Infinity, layerMask: 13);


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
    public void RpcApplyRocketForceToSelf(float explForce, Vector2 explosionCenter, Vector2 direction)
    {
        isGrounded = false;
        groundedCheckReset = numUpdatesToIgnoreGroundedCheck;
        if (direction == new Vector2())
        {
            //note that the true direction is not explosionCenter-this.transform.position, the transform position changes by the time this gets called on the client. [ClientRpc] just does that
            rb.AddForce(-explForce * (explosionCenter - (Vector2)this.transform.position).normalized, ForceMode2D.Impulse);

        }else
        {
            rb.AddForce(-explForce * (direction).normalized, ForceMode2D.Impulse);
        }
        
    }
}
