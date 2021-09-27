using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
  public float Speed = 5.0f;
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
        yield return StartCoroutine(Coroutine_MoveToPoint(mWayPoints.Dequeue(), Speed));
      }
      yield return null;
    }
  }

  // coroutine to move smoothly
  private IEnumerator Coroutine_MoveOverSeconds(GameObject objectToMove, Vector3 end, float seconds)
  {
    objectToMove.transform.LookAt(end);

    float elapsedTime = 0;
    Vector3 startingPos = objectToMove.transform.position;

    while (elapsedTime < seconds)
    {
      objectToMove.transform.position =  Vector3.Lerp(startingPos, end, (elapsedTime / seconds));
      elapsedTime += Time.deltaTime;

      yield return new WaitForEndOfFrame();
    }

    objectToMove.transform.position = end;
  }

  IEnumerator Coroutine_MoveToPoint(Vector3 p, float speed)
  {
    float duration = (transform.position - p).magnitude / speed;
    yield return StartCoroutine( Coroutine_MoveOverSeconds( transform.gameObject, p, duration));
  }
}
