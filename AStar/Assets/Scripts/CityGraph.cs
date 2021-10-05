using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AStar;
using UnityEngine.UI;

public class CityGraph : MonoBehaviour
{
    //Graph of the city having stop points as nodes
    Graph<StopPoint> mCityGraph = new Graph<StopPoint>();

    //A Star Path Finder alogorith with stop points as nodes
    AStarPathFinder<StopPoint> mPathFinder = new AStarPathFinder<StopPoint>();

    //Start and End vertex
    Graph<StopPoint>.Vertex mStart;
    Graph<StopPoint>.Vertex mGoal;

    //Previous Start and End vertex
    Graph<StopPoint>.Vertex preGoal;
    Graph<StopPoint>.Vertex preStart;

    //Vertex for visulisation
    public GameObject vertex;
    //Car controller script
    public CarController carController;
    //Text for showing status
    public Text StatusText;
    //gamobject for holding passenger
    public GameObject passenger;

    //line renderer for path
    private LineRenderer mPathViz;
    //path material
    public Material pathMat;
    //dropped variable for passanger
    public bool dropped = false;

    [Header("Nodes and Connections")]

    //list of way points
    public Transform[] wayPoints;

    List<Vector3> pathPointS;

    [System.Serializable]
    public class Pairs
    {
        public int[] pair = new int[2];
    }

    //connections between nodes
    public Pairs[] pairs;

    //start points
    public enum Starts
    {
        point0,
        point1,
        point2,
        point3,
        point4,
        point5,
        point6,
        point7,
        point8,
        point9
    };

    [Header("Start And Goals Selection")]
    public Starts start = Starts.point0;

    //goal points
    public enum Goals
    {
        point0,
        point1,
        point2,
        point3,
        point4,
        point5,
        point6,
        point7,
        point8,
        point9
    };

    public Goals goal = Goals.point5;

    void CreateGraph()
    {
        //iterate over waypoints and add them as stop points in the graph
        foreach (var item in wayPoints)
            mCityGraph.AddVertex(new StopPoint("wayPoint_" + item.position.x + "_" + item.position.y, item.position.x, item.position.y, item.position.z));

        //take the pairs and add as connections to the graph
        for (int i = 0; i < pairs.Length; i++)
        {
            int PointA = pairs[i].pair[0];
            int PointB = pairs[i].pair[1];

            mCityGraph.AddDirectedEdge(mCityGraph.Vertices[PointA], mCityGraph.Vertices[PointB], StopPoint.Distance(mCityGraph.Vertices[PointA].Value, mCityGraph.Vertices[PointB].Value));
            mCityGraph.AddDirectedEdge(mCityGraph.Vertices[PointB], mCityGraph.Vertices[PointA], StopPoint.Distance(mCityGraph.Vertices[PointB].Value, mCityGraph.Vertices[PointA].Value));
        }
    }

    void Start()
    {
        CreateGraph();

        //set start points
        switch (start)
        {
            case Starts.point0:
                mStart = mCityGraph.Vertices[0];
                preStart = mCityGraph.Vertices[0];
                break;
            case Starts.point1:
                mStart = mCityGraph.Vertices[1];
                preStart = mCityGraph.Vertices[1];
                break;
            case Starts.point2:
                mStart = mCityGraph.Vertices[2];
                preStart = mCityGraph.Vertices[2];
                break;
            case Starts.point3:
                mStart = mCityGraph.Vertices[3];
                preStart = mCityGraph.Vertices[3];
                break;
            case Starts.point4:
                mStart = mCityGraph.Vertices[4];
                preStart = mCityGraph.Vertices[4];
                break;
            case Starts.point5:
                mStart = mCityGraph.Vertices[5];
                preStart = mCityGraph.Vertices[5];
                break;
            case Starts.point6:
                mStart = mCityGraph.Vertices[6];
                preStart = mCityGraph.Vertices[6];
                break;
            case Starts.point7:
                mStart = mCityGraph.Vertices[7];
                preStart = mCityGraph.Vertices[7];
                break;
            case Starts.point8:
                mStart = mCityGraph.Vertices[8];
                preStart = mCityGraph.Vertices[8];
                break;
            case Starts.point9:
                mStart = mCityGraph.Vertices[9];
                preStart = mCityGraph.Vertices[9];
                break;
        }

        //set goal points
        switch (goal)
        {
            case Goals.point0:
                mGoal = mCityGraph.Vertices[0];
                preGoal = mCityGraph.Vertices[0];
                break;
            case Goals.point1:
                mGoal = mCityGraph.Vertices[1];
                preGoal = mCityGraph.Vertices[1];
                break;
            case Goals.point2:
                mGoal = mCityGraph.Vertices[2];
                preGoal = mCityGraph.Vertices[2];
                break;
            case Goals.point3:
                mGoal = mCityGraph.Vertices[3];
                preGoal = mCityGraph.Vertices[3];
                break;
            case Goals.point4:
                mGoal = mCityGraph.Vertices[4];
                preGoal = mCityGraph.Vertices[4];
                break;
            case Goals.point5:
                mGoal = mCityGraph.Vertices[5];
                preGoal = mCityGraph.Vertices[5];
                break;
            case Goals.point6:
                mGoal = mCityGraph.Vertices[6];
                preGoal = mCityGraph.Vertices[6];
                break;
            case Goals.point7:
                mGoal = mCityGraph.Vertices[7];
                preGoal = mCityGraph.Vertices[7];
                break;
            case Goals.point8:
                mGoal = mCityGraph.Vertices[8];
                preGoal = mCityGraph.Vertices[8];
                break;
            case Goals.point9:
                mGoal = mCityGraph.Vertices[9];
                preGoal = mCityGraph.Vertices[9];
                break;
        }

        //set car position from start
        carController.transform.position = new Vector3( mStart.Value.Point.x, mStart.Value.Point.y, mStart.Value.Point.z );

        passenger.transform.position = new Vector3( mGoal.Value.Point.x, mGoal.Value.Point.y, mGoal.Value.Point.z );

        //iterate over vertices
        for (int i = 0; i < mCityGraph.Vertices.Count; ++i)
        {
            Vector3 pos = Vector3.zero;
            pos.x = mCityGraph.Vertices[i].Value.Point.x;
            pos.y = mCityGraph.Vertices[i].Value.Point.y;
            pos.z = mCityGraph.Vertices[i].Value.Point.z;

            //add vertex for visulisation
            GameObject obj = Instantiate(vertex, pos, Quaternion.identity);
            obj.name = mCityGraph.Vertices[i].Value.Name;

            //add path for visulisation
            PathHighlighter vertexViz = obj.AddComponent<PathHighlighter>();
            vertexViz.SetVertex(mCityGraph.Vertices[i]);
        }

        //calcualte Heuristic Cost
        mPathFinder.HeuristicCost = StopPoint.GetHeuristicCost;
        //calcualte Node Traversal Cost
        mPathFinder.NodeTraversalCost = StopPoint.GetEuclideanCost;
        //if path found 
        mPathFinder.onSuccess = OnPathFound;
        //if path not found
        mPathFinder.onFailure = OnPathNotFound;

        // Line renderer to show the path.
        mPathViz = transform.gameObject.AddComponent<LineRenderer>();
        mPathViz.startWidth = 0.3f;
        mPathViz.endWidth = 0.3f;
        mPathViz.material = pathMat;

        //Initialize the path finder
        mPathFinder.Initialize(mStart, mGoal);

        //Initialize the path finder
        StartCoroutine( GetPathSteps());
    }

    IEnumerator GetPathSteps()
    {
        while (mPathFinder.Status == PathFinderStatus.RUNNING)
        {
            mPathFinder.Step();
          
            yield return null;
        }
    }

    public void OnPathFound()
    {
        //Debug.Log( "Destination Path found" );
        StatusText.text = "Destination Path found";

        PathFinder<StopPoint>.PathFinderNode node = mPathFinder.CurrentNode;

        pathPointS = new List<Vector3>();

        //reverse the found indices
        List<StopPoint> reverse_indices = new List<StopPoint>();

        //add nodes from the back
        while (node != null)
        {
            reverse_indices.Add(node.Location.Value);
            node = node.Parent;
        }

        //get the count of the nodes for found path

        mPathViz.positionCount = reverse_indices.Count;
        for (int i = reverse_indices.Count - 1; i >= 0; i--)
        {
            //reverse indices will be added as way points for car controller
            carController.AddWayPoint(new Vector3(reverse_indices[i].Point.x, reverse_indices[i].Point.y, reverse_indices[i].Point.z));

            pathPointS.Add( new Vector3( reverse_indices[i].Point.x, reverse_indices[i].Point.y, reverse_indices[i].Point.z ));
            //setting points for found path
            mPathViz.SetPosition(i, new Vector3(reverse_indices[i].Point.x, reverse_indices[i].Point.y + 1.0f, reverse_indices[i].Point.z));
        }

        //For next path finding, we need to start from previouse goal
        mStart = mGoal;

        if(!dropped)
            InvokeRepeating( "stopWatch", 0, 1 );
    }

    int sec = 0;
    void stopWatch()
    {
        sec++;
    }

    void OnPathNotFound()
    {
       // Debug.Log( "Destination not found!" );
        StatusText.text = "Destination not found!";
    }


    Vector3 k;
    private void Update()
    {
        Vector3 g = new Vector3(mGoal.Value.Point.x, mGoal.Value.Point.y, mGoal.Value.Point.z);

        if (Vector3.Distance(carController.transform.position, g ) == 0f)
        {
            if( !dropped )
            {
                //Debug.Log( "Passenger dropped, Now moving back" );
                
                StatusText.text = "Passenger dropped, Now moving back";
                passenger.gameObject.gameObject.SetActive( true );

                k = new Vector3( preStart.Value.Point.x, preStart.Value.Point.y, preStart.Value.Point.z );

                //again initialize path finder but in reverse order
                mPathFinder.Initialize( preGoal, preStart );
                StartCoroutine( GetPathSteps() );
                dropped = true;
            }
        }

        if( dropped && Vector3.Distance( carController.transform.position, k ) == 0 )
        {
            dropped = false;

            CancelInvoke( "stopWatch" );
            Debug.Log( "Time : " + sec + " seconds" );

            Vector3 p = Vector3.zero;
            float d = 0.0f;

			for( int i = 0; i < pathPointS.Count; i++ )
			{
                if( i > 0 )
                    d += Vector3.Distance(p,pathPointS[i]);
                
                p = pathPointS[i];
			}

            Debug.Log( "Distance : " + d );
        }
    }
}
