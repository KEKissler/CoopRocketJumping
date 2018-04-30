using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Scene_Loader : NetworkBehaviour  {

    public int ready_count = 0;

    public GameObject player1;
    public GameObject player2;
	// Use this for initialization
	void Start () {
		
	}


    // Update is called once per frame
    void Update () {
    
       if (ready_count ==2)
        {
            LoadOnline("NetworkTest_Tina");
        }

    }

    [ServerCallback]
    public void LoadOnline(string sceneName)
    {

        NetworkLobbyManager.singleton.ServerChangeScene(sceneName);
       
 
    }
}
