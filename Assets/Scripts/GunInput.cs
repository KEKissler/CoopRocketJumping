using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunInput : MonoBehaviour {
    //public GameObject character;
    public bool controller = false;
	// Use this for initialization
	void Start () {
    }
	
    public bool inputChange(float deadZoneThreshold)
    {
        if (controller)
        {
            return Vector2.Distance(new Vector2(), new Vector2(Input.GetAxis("Joystick_2_x"), Input.GetAxis("Joystick_2_y"))) >= deadZoneThreshold;
        }
        else
        {
            return Vector2.Distance(new Vector2(), new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"))) > 0;
        }
    }

    public Vector3 getRotation()
    {
        if (controller)
        {
            return new Vector3(0f, 0f, Mathf.Rad2Deg * Mathf.Atan2(Input.GetAxis("Joystick_2_x"), Input.GetAxis("Joystick_2_y")));
        }
        else
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
    
            Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
            mousePos.x = mousePos.x - objectPos.x;
            mousePos.y = mousePos.y - objectPos.y;
    
            float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg + 90f;
            return new Vector3(0, 0, angle);
        }
    }

    public bool getJump()
    {
        if (controller)
            return Input.GetKeyDown(KeyCode.Joystick1Button0);
        else
            return Input.GetKeyDown(KeyCode.Space);
    }

    public float getXAxis()
    {
        if (controller)
            return Input.GetAxis("Joystick_1_x");
        else
            return Input.GetAxis("Horizontal");
    }

    public bool getFire()
    {
        if (controller)
            return Input.GetAxis("RightTrigger") != 0;
        else
            return Input.GetKeyDown(KeyCode.Mouse0);
    }
}
