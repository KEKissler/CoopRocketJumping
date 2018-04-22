using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CameraController : MonoBehaviour {

    public Transform toFollow;

    private Vector3 offset;

    
    private bool Player_found = false, beenHereOnceBefore = false;
	// Use this for initialization
	void Start () {

        /*GameObject[] toTest = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject g in toTest)
        {
            if (g.GetComponent<NetworkIdentity>() && g.GetComponent<NetworkIdentity>().isLocalPlayer)
            {
                toFollow = g;
                offset = transform.position - toFollow.transform.position;
                Player_found = true;
            }
        }*/

    }

    // Update is called once per frame
    void Update() {

        if (!Player_found)
        {
            
        }
        else
        {
            if(beenHereOnceBefore)
            transform.position = toFollow.position + offset;
            if (!beenHereOnceBefore)
                beenHereOnceBefore = true;
        }
    }

    public void assignObjToFollow(Transform t)
    {
        toFollow = t;
        offset = transform.position - toFollow.position;
        Player_found = true;
    }
}
