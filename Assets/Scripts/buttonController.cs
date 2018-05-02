using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class buttonController : NetworkBehaviour {

	public bool active = false;
	//public GameObject toToggle;
    [SyncVar]
	private bool stay = false;
	private SpriteRenderer sr;
	// Use this for initialization
	void Start () {
		sr = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!active)
		{
			if (stay)
			{
				active = true;
                sr.sprite = Resources.Load<Sprite>("Button_On");
                //sr.color = Color.blue;
            }
		}
		else
		{
			if (!stay)
			{
				active = false;
                sr.sprite = Resources.Load<Sprite>("Button_Off" );
				//sr.color = Color.red;
			}
		}
	}

	void OnTriggerStay2D(Collider2D Coll2D)
	{
        if (!stay && Coll2D.tag == "Player" || Coll2D.tag == "PhysObj")
            CmdSetStay(true);
	}
	void OnTriggerExit2D(Collider2D Coll2D)
	{
        /*
        bool shouldNotTouch = false;
        BoxCollider2D colliderInQuestion = GetComponent<BoxCollider2D>();

        BoxCollider2D[] bc = new BoxCollider2D[256];
        bc = FindObjectsOfType<BoxCollider2D>();
        CircleCollider2D[] cc = new CircleCollider2D[256];
        cc = FindObjectsOfType<CircleCollider2D>();

        foreach (CircleCollider2D c in cc)
        {
            if (c.tag == "Player" && colliderInQuestion.bounds.Intersects(c.bounds))
            {
                shouldNotTouch = true;
            }
        }

        foreach (BoxCollider2D b in bc)
        {
            
            if (b.tag == "PhysObj" && colliderInQuestion.bounds.Intersects(b.bounds))
            {
                Debug.Log("hi" + b.name);
                shouldNotTouch = true;
            }
        }
        */
        //if (!shouldNotTouch)
        //{
            CmdSetStay(false);
        //}        
	}

    [Command]
    public void CmdSetStay(bool newVal)
    {
        stay = newVal;
    }
}
