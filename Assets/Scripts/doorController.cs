using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorController : MonoBehaviour {

	//public GameObject source;
	public buttonController s;
	private Transform child;
	// Use this for initialization
	void Start () {
		//s = source.GetComponent<buttonController>();
		child = transform.GetChild(0);
	}
	
	// Update is called once per frame
	void Update () {
		if (s.active && child.gameObject.activeSelf)
		{
			child.gameObject.SetActive(false);
		}
		if (!s.active && !child.gameObject.activeSelf)
		{
			child.gameObject.SetActive(true);
		}
	}
}
