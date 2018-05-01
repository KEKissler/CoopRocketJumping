using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boxTpController : MonoBehaviour {
    //this script will save the initial position of the box and if the resetToSpawnPoint function is called, reset the box to be
    //stationary at that spawn point at that spawn rotation.
    private Vector2 respawnPoint;
    private Quaternion respawnRotation;

    void Start () {
        respawnPoint = (Vector2)transform.position;
        respawnRotation = transform.rotation;
	}

    public void resetToSpawnPoint()
    {
        transform.position = respawnPoint;
        transform.rotation = respawnRotation;
        GetComponent<Rigidbody2D>().velocity = new Vector2();
    }
}
