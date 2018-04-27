using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Scene_Loader : NetworkBehaviour  {


    bool p1_rdy = false;
    bool p2_rdy = false;

	// Use this for initialization
	void Start () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "Player")
        {
          
            if (collision.transform.parent.GetComponent<GunControl>().netId.ToString() == "1")
                p1_rdy = true;
            else
                p2_rdy = true;

        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (collision.transform.parent.GetComponent<GunControl>().netId.ToString() == "1")
                p1_rdy = false;
            else
                p2_rdy = false;

        }
    }

    // Update is called once per frame
    void Update () {


        if (p1_rdy && p2_rdy)
        {
            LoadOnline("NetworkTest_Tina");
        }
    }
    [ServerCallback]
    public void LoadOnline(string sceneName)
    {

        NetworkManager.singleton.ServerChangeScene(sceneName);

    }
}
