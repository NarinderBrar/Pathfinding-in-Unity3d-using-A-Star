using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraficLights : MonoBehaviour
{
    public float switchTime;

    //car will detect collider, if found will stop
    //this collider will be disabled for green light
    public Collider hitCollider;

    //to chnage the lights, we need materials, where we can change the colour to create the on/off effect
    public Material rMat;
    public Material yMat;
    public Material gMat;

    // colours used to switch on the lights
    public Color onRedColor;
    public Color onYellowColor;
    public Color onGreenColor;

    // colours used to switch off lights
    public Color offRedColor;
    public Color offYellowColor;
    public Color offGreenColor;

    //useful for changing the light
    public int count = 0;

    void Start()
    {
        //start with red light
        rMat.color = onRedColor;
        yMat.color = offYellowColor;
        gMat.color = offGreenColor;

        //Invokes the method methodName in time seconds, then repeatedly every repeatRate seconds.
        InvokeRepeating( "ChangeLights", 2.0f, switchTime);
    }

    void ChangeLights()
    {
        //switch off all lights 
        rMat.color = offRedColor;
        yMat.color = offYellowColor;
        gMat.color = offGreenColor;

        //increase the value of count
        count++;

        //according to the count valuse change the light
        switch (count)
        {
            //red light on
            case 1:
                hitCollider.enabled = true;
                rMat.color = onRedColor;
                break;
            //yellow light on
            case 2:
                yMat.color = onYellowColor;
                break;
            //green light on
            case 3:
                hitCollider.enabled = false;
                gMat.color = onGreenColor;
                count = 0;
                break;
        }
    }
}
