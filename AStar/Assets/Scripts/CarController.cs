using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 5.0f;
    public float senseDistance;

    public bool stop = false;

    public TraficLights traficLights;

    public Queue<Vector3> mWayPoints = new Queue<Vector3>();

    void Start()
    {
        StartCoroutine(Coroutine_MoveTo());
    }

    public void AddWayPoint(float x, float y, float z)
    {
        AddWayPoint(new Vector3(x, y, z));
    }

    public void AddWayPoint(Vector3 pt)
    {
        mWayPoints.Enqueue(pt);
    }

    public IEnumerator Coroutine_MoveTo()
    {
        while (true)
        {
            while (mWayPoints.Count > 0)
            {
                yield return StartCoroutine(Coroutine_MoveToPoint(mWayPoints.Dequeue(), speed));
            }
            yield return null;
        }
    }

    IEnumerator Coroutine_MoveToPoint(Vector3 p, float speed)
    {
        float duration = (transform.position - p).magnitude / speed;
        yield return StartCoroutine(Coroutine_MoveOverSeconds(transform.gameObject, p, duration));
    }

    // coroutine to move smoothly
    private IEnumerator Coroutine_MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
    {
        objectToMove.transform.LookAt(end);

        float elapsedTime = 0;
        float slowDown = 1.0f;

        Vector3 startingPos = objectToMove.transform.position;
        RaycastHit hit;

        while (elapsedTime < seconds)
        {
            if (stop)
            {
                objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime * slowDown;

                slowDown -= Time.deltaTime * 0.5f;
                slowDown = Mathf.Max(slowDown, 0);
            }
            else
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, senseDistance))
                    if (hit.collider.tag == "stopPoint")
                    {
                        stop = true;
                        traficLights = hit.collider.GetComponent<TraficLights>();
                    }

                objectToMove.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
                elapsedTime += Time.deltaTime;

                slowDown = 1.0f;
            }

            yield return new WaitForEndOfFrame();
        }

        objectToMove.transform.position = end;
    }

    private void Update()
    {
        if(traficLights!=null)
        {
           if( traficLights.count == 3)
            {
                traficLights = null;
                stop = false;
            }
        }
    }
}
