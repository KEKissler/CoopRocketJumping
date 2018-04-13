using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController_Mouse : MonoBehaviour {
    public GameObject projectile, projectileParent;
    public float jumpForce, groundedCheckDist, deadZoneThreshold, walkSpeed, airWalkSpeed, explosionRadius, explosionForce, fireRate;
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
	void Update () {
        //Debug.Log(Vector2.Distance(new Vector2(), new Vector2(Input.GetAxisRaw("Joystick_2_x"), Input.GetAxisRaw("Joystick_2_y")))+"\n" + new Vector2(Input.GetAxisRaw("Joystick_2_x"), Input.GetAxisRaw("Joystick_2_y")).magnitude);
        
        if (Vector2.Distance(new Vector2(), new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))) >= deadZoneThreshold)//use circle and own deadzone calc to make circular deadzone
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
    
            Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
            mousePos.x = mousePos.x - objectPos.x;
            mousePos.y = mousePos.y - objectPos.y;
    
            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg + 90f;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            //determine the angle for the gun to be facing according to player input iff player gave strong enough input
           // gunRotation.eulerAngles = new Vector3(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
            //transform.rotation = gunRotation;
        }
        //manage grounded state via raycast down. Also a timer in terms of calls to this function that is the cooldown for doing that raycast check
        //prevents player being allowed to double jump due to being deemed grounded the frame after jumping and still being close to the ground
        if (!isGrounded && --groundedCheckReset <= 0)
        {
            RaycastHit2D test = Physics2D.Raycast((Vector2)(transform.position), Vector2.down, groundedCheckDist, layerMask: 1);
            if (test.collider != null) {
               // Debug.Log(test.collider.gameObject.name);
                isGrounded = true;
                rocket1.color = active;
                rocket2.color = active;
                rocket3.color = active;
                numRocketsLeft = 3;
            }
        }
        //the jump itself
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            isGrounded = false;
            groundedCheckReset = numUpdatesToIgnoreGroundedCheck;
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        }

        //left right movement both for ground and in air
        if (isGrounded)
        {
            //determine if input at all
            if(Input.GetAxis("Horizontal") != 0)
            {
                //right
                if (Input.GetAxis("Horizontal") > 0)
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
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                //right
                if (Input.GetAxisRaw("Horizontal") > 0)
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
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (timeSinceLastProjectile >= fireRate && !hasFired)
            {
                hasFired = true;
                //Debug.Log("angle: " + (this.transform.rotation.eulerAngles.z - 90) + "\nexpanded vector:" + new Vector2(Mathf.Cos(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90)), Mathf.Sin(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90))));
                RaycastHit2D test = Physics2D.Raycast((Vector2)(transform.position), new Vector2(Mathf.Cos(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90)), Mathf.Sin(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90))), distance: Mathf.Infinity, layerMask: 1);
                if (!isGrounded)
                {
                    
                    if (numRocketsLeft == 1)
                        rocket1.color = inactive;
                    if (numRocketsLeft == 2)
                        rocket2.color = inactive;
                    if (numRocketsLeft == 3)
                        rocket3.color = inactive;

                }

                if (numRocketsLeft > 0)
                {
                    projectile.transform.position = transform.position;
                    projectileController pC = projectile.GetComponent<projectileController>();
                    pC.Fire(new Vector2(Mathf.Cos(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90)), Mathf.Sin(Mathf.Deg2Rad * (this.transform.rotation.eulerAngles.z - 90))));
                
                }
                if (test.collider != null)
                {

                    //Debug.Log(test.collider.gameObject.name + "   " + Vector2.Distance((Vector2)(test.transform.position), (Vector2)transform.position));
                    //projectile.transform.position = new Vector3(test.point.x, test.point.y, projectile.transform.position.z);
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

    void LateUpdate()
    {
        if(rb.velocity.x > 0)
            GetComponent<SpriteRenderer>().flipX = true;
        else
            GetComponent<SpriteRenderer>().flipX = false;
    }
}
