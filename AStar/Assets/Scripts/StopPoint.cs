using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopPoint : System.IEquatable<StopPoint>
{

	//name of stop point
	public string Name { get; set; }
	//location of stop point
	public Vector3 Point { get; set; }

	//Constructor for stop points
	public StopPoint()
	{

	}

	//Constructor for stop points
	public StopPoint( string names, Vector2 point )
	{
		Name = names;
		Point = point;
	}

	//Constructor for stop points
	public StopPoint( string name, float x, float y, float z )
	{
		Name = name;
		Point = new Vector3( x, y, z );
	}

	//method to check if object is a stop point
	public override bool Equals( object obj ) => this.Equals( obj as StopPoint );

	public bool Equals( StopPoint p )
	{
		//if stop point is null, return false
		if( p is null )
			return false;
		
		// Optimization for a common success case
		if( System.Object.ReferenceEquals( this, p ) )
			return true;

		// If run-time types are not exactly the same, return false.
		if( this.GetType() != p.GetType() )
			return false;

		//if the fields match, return true 
		return ( Name == p.Name );
	}

	//get hash code with given name and point
	public override int GetHashCode() => (Name, Point).GetHashCode();


	//Calculate Heuristic Cost between two given points
	public static float GetHeuristicCost( StopPoint a, StopPoint b )
	{
		float c = Mathf.Abs( a.Point.x - b.Point.x ) + Mathf.Abs( a.Point.y - b.Point.y );
		Debug.Log( "Heuristic Cost: From " + a.Name + " to " + b.Name +" is : "+ c); 
		return c;
	}

	//Calculate Euclidean Cost between two given points
	public static float GetEuclideanCost( StopPoint a, StopPoint b )
	{
		return ( a.Point - b.Point ).magnitude;
	}

	//a handy function to calculate distance btw two points
	public static float Distance( StopPoint a, StopPoint b )
	{
		return ( a.Point - b.Point ).magnitude;
	}

	//a handy function to get angle between two stops
	public static float GetAngleBetweenTwoStops( StopPoint a, StopPoint b )
	{
		float delta_x = b.Point.x - a.Point.x;
		float delta_y = b.Point.y - a.Point.y;
		float theta_radians = Mathf.Atan2( delta_y, delta_x );
		return theta_radians;
	}
}