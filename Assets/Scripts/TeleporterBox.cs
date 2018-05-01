using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterBox : MonoBehaviour {
    public GameObject ObjectLocationToTPTo;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PhysObj")
        {
            other.GetComponent<boxTpController>().resetToSpawnPoint();
        }
        else
        {
            if (other.transform.parent != null)
            {
                other.transform.parent.GetComponent<Rigidbody2D>().velocity = new Vector2();
                other.transform.parent.position = ObjectLocationToTPTo.transform.position;
            }
            else
            {
                other.transform.position = ObjectLocationToTPTo.transform.position;
            }
        }
    }
}
