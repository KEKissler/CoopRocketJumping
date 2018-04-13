using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileController : MonoBehaviour {

	private Vector2 direction;
	public float speed;
	public float minSize;
	public float maxSize;
	public float sizeDelta;
	public float minForce;
	public float maxForce;
	public float forceDelta;
	private float currentForce;
	private float currentSize;
	private bool fired = false;

	// Use this for initialization
	void Start () {
		Physics2D.IgnoreLayerCollision(8, 9, true);
	}
	
	// Update is called once per frame
	void Update () {
		if (fired)
		{
			transform.position += new Vector3(speed * direction.x, speed * direction.y, 0);
			if (currentForce + forceDelta <= maxForce)
				currentForce += forceDelta;
			if (currentSize + sizeDelta <= maxSize)
				currentSize += sizeDelta;
			transform.localScale = new Vector3(currentSize,currentSize,1);
			//GetComponent<CircleCollider2D>().radius = currentSize;
		}
	}

	public void Fire(Vector2 dir)
	{
		direction = dir;
		currentSize = minSize;
		//GetComponent<CircleCollider2D>().radius = currentSize;
		transform.localScale = new Vector3(currentSize,currentSize,1);
		currentForce = minForce;
		//transform.position += new Vector3(2*speed * direction.x, 2*speed * direction.y, 0);
		fired = true;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		//Debug.Log(col.gameObject.name);
		
		if (fired && col.gameObject.tag != "Player")
		{
			//Debug.Log(col.gameObject.name);
			fired = false;
			//this.enabled = false;
			//transform.localScale = Vector3.zero;
		}
	}
}
