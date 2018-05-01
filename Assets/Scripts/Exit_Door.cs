using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit_Door : MonoBehaviour {

    public bool did_tp = false;
	// Use this for initialization
	void Start () {

	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
                GetComponent<Scene_Loader>().ready_count++;
        // if (GetComponent<Scene_Loader>().ready_count == 1)
        //  GetComponent<Scene_Loader>().player1 = collision.gameObject;
        // else if (GetComponent<Scene_Loader>().ready_count == 2)
        //     GetComponent<Scene_Loader>().player2 = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !did_tp)
           GetComponent<Scene_Loader>().ready_count--;
    }
    // Update is called once per frame
    void Update () {
		
	}
}
