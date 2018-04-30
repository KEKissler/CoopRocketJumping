using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class barebone_movement : NetworkBehaviour {

    Vector2 movement;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!isLocalPlayer)
        {
            return;
        }

        movement.x = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 20, ForceMode2D.Impulse);
        }

    }

    private void FixedUpdate()
    {
        transform.Translate(new Vector3(10 * Time.deltaTime * movement.x, movement.y,0));
    }
}

