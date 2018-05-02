using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitChecker : MonoBehaviour {

    public int tp_count  = 0;
	// Use this for initialization
	void Start () {

	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
                tp_count++;

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && tp_count <2)
           tp_count--;
    }
    // Update is called once per frame
    void Update () {
		
	}
}
