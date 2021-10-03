using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AStar;

public class PathHighlighter : MonoBehaviour
{
	public Graph<StopPoint>.Vertex Vertex
	{
		get
		{
			return mVertex;
		}
	}

	private Graph<StopPoint>.Vertex mVertex;

	List<GameObject> mLines = new List<GameObject>();

	private LineRenderer GetOrCreateLine( int index )
	{
		if( index >= mLines.Count )
		{
			GameObject obj = new GameObject();
			obj.name = "line_" + index.ToString();
			obj.transform.SetParent( transform );
			obj.transform.position = new Vector3( 0.0f, 0.0f, 0.0f );

			LineRenderer lr = obj.AddComponent<LineRenderer>();
			obj.layer = 8;

			mLines.Add( obj );

			lr.material = new Material( Shader.Find( "Unlit/Color" ) );
			lr.material.color = new Color( 0.0f, 0.5f, 0.0f, 1.0f );

			lr.startWidth = 3;
			lr.endWidth = 3;
		}
		return mLines[index].GetComponent<LineRenderer>();
	}

	public void SetVertex( Graph<StopPoint>.Vertex vertex )
	{
		mVertex = vertex;
		for( int i = 0; i < mVertex.Neighbours.Count; ++i )
		{
			Graph<StopPoint>.Vertex n = mVertex.Neighbours[i] as Graph<StopPoint>.Vertex;

			Vector3 a = new Vector3( mVertex.Value.Point.x, mVertex.Value.Point.y + 0.5f, mVertex.Value.Point.z );
			Vector3 b = new Vector3( n.Value.Point.x, n.Value.Point.y + 0.5f, n.Value.Point.z );

			// find the direction.
			Vector3 dir = ( b - a );
			float distance = dir.magnitude;

			dir.Normalize();

			// instead of percentage use fixed lengths
			// and arrow heads so that they dont scale.
			Vector3 c = a + dir * 0.22f;
			Vector3 d = b - dir * 0.05f;
			Vector3 e = b - dir * 0.91f;
			Vector3 f = b - dir * 0.9f;

			float p1 = ( e - c ).magnitude / ( d - c ).magnitude;
			float p2 = ( f - c ).magnitude / ( d - c ).magnitude;

			LineRenderer lr = GetOrCreateLine( i );

			lr.widthCurve = new AnimationCurve(
				  new Keyframe( 0, 0.5f ),
				  new Keyframe( p1, 0.5f ), // neck of arrow
				  new Keyframe( p2, 1.5f ), // max width of arrow head
				  new Keyframe( 1, 0f ) );   // tip of arrow

			lr.positionCount = 4;
			lr.SetPositions( new Vector3[] { c, e, f, d } );
		}
	}
}
