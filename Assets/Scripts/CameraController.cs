using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject toFollow;

    private Vector3 offset;
    private int count;

    private bool Player_found = false;
	// Use this for initialization
	void Start () {

        toFollow = GameObject.FindGameObjectWithTag("Player");
        
	}

    // Update is called once per frame
    void Update() {

        if (!Player_found)
        {
            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                offset = transform.position - toFollow.transform.position;
                
                Player_found = true;
            }
        }
        else
        {
            transform.position = toFollow.transform.position + offset;

        }
    }
}
