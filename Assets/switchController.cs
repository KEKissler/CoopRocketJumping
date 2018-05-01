using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchController : MonoBehaviour {

    [HideInInspector]
    public bool isActivated = false;//read from this to know if switch active or not
    public float remainActiveTime;
    private float timeLeftActive = 0;

    void OnTriggerEnter2D(Collider2D Coll2D)
    {
        isActivated = true;
        timeLeftActive = remainActiveTime;
    }

    void Update()
    {
        if(timeLeftActive < 0)
        {
            isActivated = false;
        }else
        {
            timeLeftActive -= Time.deltaTime;
        }
    }
}
