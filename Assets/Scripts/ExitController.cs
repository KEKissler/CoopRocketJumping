using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ExitController : NetworkBehaviour  {

    public int ready_count = 0;
    //public List<Vector2> futureExits;
    public Vector3 nextRespawn;
    private Transform tp;
    private GameObject rp;
    //public int currentLevel;
	void Start () {
        tp = this.transform.GetChild(0);
        rp = GameObject.Find("RespawnPoint");
	}


    // Update is called once per frame
    void Update () {
    
       if (ready_count >=2)
        {

            rp.transform.position = nextRespawn;
            tp.gameObject.SetActive(false);
            this.enabled = false;
   
        }

    }
     private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
                ready_count++;

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && ready_count <2)
           ready_count--;
    }
}
