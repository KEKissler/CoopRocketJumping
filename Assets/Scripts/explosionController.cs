using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosionController : MonoBehaviour {
    public float fadeTime;
    private float totalTimeAlive = 0;
    private SpriteRenderer sr;
	// Use this for initialization
	void Start () {
        sr = transform.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        totalTimeAlive += Time.deltaTime;
        if(totalTimeAlive > fadeTime)
        {
            Destroy(this.gameObject);
        }
        else
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1-totalTimeAlive/fadeTime);
        }
	}
}
