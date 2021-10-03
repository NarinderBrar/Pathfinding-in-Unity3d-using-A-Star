using System.Collections;
using System.Collections.Generic;

namespace AStar
{
	public class Graph<T>
	{
		public class Vertex : Node<T>
		{
			// List of costs associated with the traversal of each edge.
			private List<float> mCosts = new List<float>();

			// List of neighbours for this vertex
			private List<Node<T>> mNeighbours = new List<Node<T>>();

			//Node
			public Vertex( T value ): base( value )
			{ }

			//Node with list of neighbours
			public Vertex( T value, List<Node<T>> neighbours ): base( value )
			{
				mNeighbours = neighbours;
			}

			//get Neighbours
			public List<Node<T>> Neighbours
			{
				get
				{
					return mNeighbours;
				}
			}

			//get Costs
			public List<float> Costs
			{
				get
				{
					return mCosts;
				}
			}

			//getting Neighbours using function
			public override List<Node<T>> GetNeighbours()
			{
				return mNeighbours;
			}
		}

		private List<Vertex> mVertices;

		public delegate void DelegateGraphNode( Vertex n );
		public delegate void DelegateOnAddEdge( Vertex a, Vertex b );

		public DelegateGraphNode mOnAddNode;
		public DelegateGraphNode mOnRemoveNode;
		public DelegateOnAddEdge mOnAddDirectedEdge;

		public Graph(): this( null )
		{
		}

		public Graph( List<Vertex> mVertices )
		{
			if( mVertices == null )
				this.mVertices = new List<Vertex>();
			else
				this.mVertices = mVertices;
		}

		// Add a vertex to the graph.
		public void AddVertex( Vertex node )
		{
			mVertices.Add( node );
			mOnAddNode?.Invoke( node );
		}

		// Add a vertex by taking in the value type of the graph. 
		public void AddVertex( T value )
		{
			AddVertex( new Vertex( value ) );
		}

		// Add a directed edge with the associated cost.
		public void AddDirectedEdge(Vertex from,Vertex to, float cost )
		{
			from.Neighbours.Add( to );
			from.Costs.Add( cost );
			mOnAddDirectedEdge?.Invoke( from, to );
		}

		// Add an undirected edge with the associated cost as same for both directions.
		public void AddUndirectedEdge(Vertex a,Vertex b,float cost )
		{
			AddUndirectedEdge( a, b, cost, cost );
		}

		// Add an undirected edge with different associated costs for each direction.
		public void AddUndirectedEdge(Vertex a,Vertex b,float cost1,float cost2 )
		{
			AddDirectedEdge( a, b, cost1 );
			AddDirectedEdge( b, a, cost2 );
		}

		// Find a vertex that has a T of value.
		public static Vertex FindByValue(List<Vertex> nodes,T value )
		{
			// search the list for the value
			foreach( Vertex node in nodes )
				if( node.Value.Equals( value ) )
					return node;

			// if we reached here, we didn't find a matching node
			return null;
		}

		public bool Contains( T value )
		{
			return FindByValue( mVertices, value ) != null;
		}

		public bool Remove( T value )
		{
			// first remove the node from the mVertices
			Vertex nodeToRemove = FindByValue( mVertices, value );
			if( nodeToRemove == null )
				// node wasn't found
				return false;

			mOnRemoveNode( nodeToRemove );
			// otherwise, the node was found
			mVertices.Remove( nodeToRemove );

			// enumerate through each node in the mVertices, 
			// removing edges to this node
			foreach( Vertex gnode in mVertices )
			{
				int index = gnode.Neighbours.IndexOf( nodeToRemove );
				if( index != -1 )
				{
					// remove the reference to the node 
					// and associated cost
					gnode.Neighbours.RemoveAt( index );
					gnode.Costs.RemoveAt( index );
				}
			}

			return true;
		}

		public List<Vertex> Vertices
		{
			get
			{
				return mVertices;
			}
		}

		public int Count
		{
			get { return mVertices.Count; }
		}

	}
}
