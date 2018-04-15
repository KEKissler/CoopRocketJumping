using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterBox : MonoBehaviour {
    public GameObject ObjectLocationToTPTo;

    public void OnTriggerEnter2D(Collider2D other)
    {
        other.transform.position = ObjectLocationToTPTo.transform.position;
    }
}
