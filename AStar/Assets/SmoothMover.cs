using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMover : MonoBehaviour
{
	public float speed = 5.0f;

	public Vector3[] positions;
	int i = 0;

	void Start()
	{
		StartCoroutine( Move() );
	}

	public IEnumerator Move()
	{
		while( i < positions.Length )
		{
			yield return StartCoroutine( MoveToWaypoint( positions[i], speed ) );
			i++;
		}
		yield return null;
	}

	IEnumerator MoveToWaypoint( Vector3 p, float speed )
	{
		float duration = ( transform.position - p ).magnitude / speed;
		yield return StartCoroutine( MoveOverSeconds( transform.gameObject, p, duration ) );
	}

	private IEnumerator MoveOverSeconds( GameObject obj, Vector3 end, float seconds )
	{
		float elapsedTime = 0;
		Vector3 startingPos = obj.transform.position;

		while( elapsedTime < seconds )
		{
			obj.transform.position = Vector3.Lerp( startingPos, end, ( elapsedTime / seconds ) );
			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		obj.transform.position = end;
	}
}