using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraficLights : MonoBehaviour
{
    public float switchTime;

    public Collider hitCollider;

    public Material rMat;
    public Material yMat;
    public Material gMat;

    public Color onRedColor;
    public Color onYellowColor;
    public Color onGreenColor;

    public Color offRedColor;
    public Color offYellowColor;
    public Color offGreenColor;

    public int count = 0;

    void Start()
    {
        rMat.color = onRedColor;
        yMat.color = offYellowColor;
        gMat.color = offGreenColor;

        InvokeRepeating("ChangeLights", 2.0f, switchTime);
    }

    void ChangeLights()
    {
        rMat.color = offRedColor;
        yMat.color = offYellowColor;
        gMat.color = offGreenColor;

        count++;

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
        

        Debug.Log(count);
    }

}
