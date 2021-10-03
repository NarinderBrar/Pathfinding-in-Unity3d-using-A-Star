using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestrian : MonoBehaviour
{
    string movement = "forward";
    bool walk = true;
    public float walkAndRestTime;
    public float speed;

    void Start()
    {
        //Invokes the method methodName in 0.0 seconds, then repeatedly every walkAndRestTime seconds.
        InvokeRepeating( "ChangeDirection", 0.0f, walkAndRestTime );
    }

    void ChangeDirection()
    {
        //movement variable will be "forward", "bstop", "backward", "stop"
        switch( movement )
		{
            case "forward":
                walk = true;

                //What it will do in next invoke
                movement = "bstop";
                //for backward motion it should rotate 180
                transform.Rotate( 0.0f, 180.0f, 0.0f, Space.Self );
                break;
            case "bstop":
                walk = false;

                //What it will do in next invoke
                movement = "backward";
                break;
            case "backward":
                walk = true;

                //What it will do in next invoke
                movement = "stop";
                //for forward motion it should rotate 180
                transform.Rotate( 0.0f, 180.0f, 0.0f, Space.Self );
                break;
            case "stop":
                walk = false;

                //What it will do in next invoke
                movement = "forward";
                break;
		}
	}
    void Update()
    {
        //if walk is true keep walking
        if( walk)
            //change the position of the transform with taking foward vector and given speed
            transform.position += transform.forward * Time.deltaTime* speed;
    }
}
