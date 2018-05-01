using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class explosionController : NetworkBehaviour
{
    //[SyncVar]
    public float fadeTime;
    [SyncVar(hook = "OnScaleChange")]
    [HideInInspector]
    public float scale;
    private float totalTimeAlive = 0;
    private SpriteRenderer sr;
    // Use this for initialization
    void Start()
    {
        sr = transform.GetComponent<SpriteRenderer>();
    }

    private void OnScaleChange(float newScale)
    {
        scale = newScale;
        transform.localScale = new Vector3(scale, scale, 1);
    }

    // Update is called once per frame
    void Update()
    {
        totalTimeAlive += Time.deltaTime;
        if (totalTimeAlive > fadeTime)
        {
            Destroy(this.gameObject);
        }
        else
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1 - totalTimeAlive / fadeTime);
        }
    }
}
