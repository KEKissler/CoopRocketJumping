﻿using System.Collections;
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
        if (Coll2D.tag == "Player" || Coll2D.tag == "PhysObj")
            CmdSetStay(true);
	}
	void OnTriggerExit2D(Collider2D Coll2D)
	{
        CmdSetStay(false);
	}

    [Command]
    public void CmdSetStay(bool newVal)
    {
        stay = newVal;
    }
}
