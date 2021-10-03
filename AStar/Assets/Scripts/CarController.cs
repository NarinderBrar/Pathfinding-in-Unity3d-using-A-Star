using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
	//speed of the cars
	public float speed = 5.0f;
	//Wait time for Pedestrian 
	public float PedestrianWaitTime = 3.0f;
	//range of the ray cast to detect obstacles
	public float senseDistance;
	//car moving or stopped
	public bool stop = false;
	//Script for refrencing trafic lights
	public TraficLights traficLights;
	//list of waypoints
	public Queue<Vector3> mWayPoints = new Queue<Vector3>();

	void Start()
	{
		//start coroutine for moving toward a waypoint
		StartCoroutine( Move() );
	}

	public void AddWayPoint( float x, float y, float z )
	{
		AddWayPoint( new Vector3( x, y, z ) );
	}

	public void AddWayPoint( Vector3 pt )
	{
		mWayPoints.Enqueue( pt );
	}

	public IEnumerator Move()
	{
		while( true )
		{
			//if waypoint count is greater than 0, start another coroutine
			while( mWayPoints.Count > 0 )
				// coroutine for one way point to another
				yield return StartCoroutine( MoveToWaypoint( mWayPoints.Dequeue(), speed ) );
			yield return null;
		}
	}

	IEnumerator MoveToWaypoint( Vector3 p, float speed )
	{
		float duration = ( transform.position - p ).magnitude / speed;
		//now we have destination point, so third coroutine is for actual movement in seconds
		yield return StartCoroutine( MoveOverSeconds( transform.gameObject, p, duration ) );
	}

	// coroutine to move smoothly
	private IEnumerator MoveOverSeconds( GameObject car, Vector3 end, float seconds )
	{
		//car should look at destination
		car.transform.LookAt( end );

		//time will decrease as car cover its distance
		float elapsedTime = 0;

		//how fast it will slow down when obstacle detected
		float slowDown = 1.0f;

		//start poision
		Vector3 startingPos = car.transform.position;

		//raycast hit for collison detection
		RaycastHit hit;

		//keep moving untile elapsedTime less than actual duration
		while( elapsedTime < seconds )
		{
			//Linearly interpolates between two points with a value 0 to 1
			car.transform.position = Vector3.Lerp( startingPos, end, ( elapsedTime / seconds ) );

			//if stop, slow down the car
			if( stop )
			{
				//slow down so that elapsedTime should stop increasing
				elapsedTime += Time.deltaTime * slowDown;

				slowDown -= Time.deltaTime * 0.8f;
				slowDown = Mathf.Max( slowDown, 0 );
			}
			else
			{
				//detect if there is any object
				if( Physics.Raycast( transform.position, transform.forward, out hit, senseDistance ) )
				{
					//if its trafic lights
					if( hit.collider.tag == "stopPoint" )
					{
						Transform tlightTf = hit.transform;

						if( Vector3.Dot( transform.forward, -tlightTf.forward ) < 0 )
						{
							stop = true;
							traficLights = hit.collider.GetComponent<TraficLights>();

							Debug.Log( "Stopping on trafic light having red signal" );
						}
					}
					//or pedestrian
					if( hit.collider.tag == "pedestrian" )
					{
						stop = true;
						Debug.Log( "Pedestrian detected, stoped and waiting him for cross the road." );
						Invoke( "Restart", PedestrianWaitTime );
					}
				}
				elapsedTime += Time.deltaTime;

				//it should reset
				slowDown = 1.0f;
			}

			yield return new WaitForEndOfFrame();
		}

		//set car position to end, once waypoints done
		car.transform.position = end;
	}

	private void Update()
	{
		if( traficLights != null )
		{
			//check if traic lights on green signal
			if( traficLights.count == 0 )
			{
				traficLights = null;
				stop = false;
			}
		}
	}
	private void Restart()
	{
		RaycastHit hit;

		//detect if there is any object
		if( Physics.Raycast( transform.position, transform.forward, out hit, senseDistance ) )
		{
			//if its trafic lights
			if( hit.collider.tag == "stopPoint" )
			{
				Debug.Log( "Stopping on trafic light having red signal" );
				return;
			}
			//or pedestrian
			if( hit.collider.tag == "pedestrian" )
			{
				Debug.Log( "Pedestrian detected, stoped and waiting him for cross the road." );
				Invoke( "Restart", PedestrianWaitTime );
				return;
			}
		}

		stop = false;
	}
}
