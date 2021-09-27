using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AStar;
using UnityEngine.UI;

public class CityGraph : MonoBehaviour
{
    Graph<StopPoint> mCityGraph = new Graph<StopPoint>();

    [SerializeField]
    GameObject VertexPrefab;

    [SerializeField]
    CarController carController;

    [SerializeField]
    Text StatusText;

    AStarPathFinder<StopPoint> mPathFinder = new AStarPathFinder<StopPoint>();

    Graph<StopPoint>.Vertex mGoal;
    Graph<StopPoint>.Vertex mStart;

    Graph<StopPoint>.Vertex preGoal;
    Graph<StopPoint>.Vertex preStart;

    LineRenderer mPathViz;

    public Transform[] wayPoints;

    [System.Serializable]
    public class Pairs
    {
        public int[] pair = new int[2];
    }

    public Pairs[] pairs;

    public enum Goals // your custom enumeration
    {
        point0,
        point1,
        point2,
        point3,
        point4,
        point5
    };

    public Goals goal = Goals.point0;

    void CreateGraph()
    {
        foreach (var item in wayPoints)
            mCityGraph.AddVertex(new StopPoint("stop_" + item.position.x + "_" + item.position.y, item.position.x, item.position.y, item.position.z));

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

        for (int i = 0; i < mCityGraph.Vertices.Count; ++i)
        {
            Vector3 pos = Vector3.zero;
            pos.x = mCityGraph.Vertices[i].Value.Point.x;
            pos.y = mCityGraph.Vertices[i].Value.Point.y;
            pos.z = mCityGraph.Vertices[i].Value.Point.z;

            GameObject obj = Instantiate(VertexPrefab, pos, Quaternion.identity);
            obj.name = mCityGraph.Vertices[i].Value.Name;

            PathHighlighter vertexViz = obj.AddComponent<PathHighlighter>();
            vertexViz.SetVertex(mCityGraph.Vertices[i]);
        }

        carController.transform.position = new Vector3(mCityGraph.Vertices[0].Value.Point.x, mCityGraph.Vertices[0].Value.Point.y, mCityGraph.Vertices[0].Value.Point.z);

        mStart = mCityGraph.Vertices[0];
        preStart = mCityGraph.Vertices[0];

        mPathFinder.HeuristicCost = StopPoint.GetManhattanCost;
        mPathFinder.NodeTraversalCost = StopPoint.GetEuclideanCost;
        mPathFinder.onSuccess = OnPathFound;
        mPathFinder.onFailure = OnPathNotFound;

        // We create a line renderer to show the path.
        mPathViz = transform.gameObject.AddComponent<LineRenderer>();
        
        mPathViz.startWidth = 0.2f;
        mPathViz.endWidth = 0.2f;
        mPathViz.startColor = Color.magenta;
        mPathViz.endColor = Color.magenta;

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
        }

        mPathFinder.Initialize(mStart, mGoal);
        StartCoroutine(Coroutine_FindPathSteps());
    }

    IEnumerator Coroutine_FindPathSteps()
    {
        while (mPathFinder.Status == PathFinderStatus.RUNNING)
        {
            mPathFinder.Step();
            yield return null;
        }
    }

    public void OnPathFound()
    {
        if (StatusText)
        {
            StatusText.text = "Path found to destination";
        }
        PathFinder<StopPoint>.PathFinderNode node = mPathFinder.CurrentNode;

        List<StopPoint> reverse_indices = new List<StopPoint>();

        while (node != null)
        {
            reverse_indices.Add(node.Location.Value);
            node = node.Parent;
        }

        mPathViz.positionCount = reverse_indices.Count;
        for (int i = reverse_indices.Count - 1; i >= 0; i--)
        {
            carController.AddWayPoint(new Vector3(reverse_indices[i].Point.x, reverse_indices[i].Point.y, reverse_indices[i].Point.z));
            mPathViz.SetPosition(i, new Vector3(reverse_indices[i].Point.x, reverse_indices[i].Point.y + 0.5f, reverse_indices[i].Point.z));
        }

        // We set the goal to be the start for next pathfinding
        mStart = mGoal;
    }

    void OnPathNotFound()
    {
        Debug.Log("Cannot find path to destination");

        if (StatusText)
        {
            StatusText.text = "Cannot find path to destination";
        }
    }

    private bool dropped = false;

    private void Update()
    {
        Vector3 g = new Vector3(mGoal.Value.Point.x, mGoal.Value.Point.y, mGoal.Value.Point.z);
        if (Vector3.Distance(carController.transform.position, g ) == 0f)
        {
            if(!dropped)
            {
                Debug.Log("Back to stop");

                mPathFinder.Initialize(preGoal, preStart);
                StartCoroutine(Coroutine_FindPathSteps());
                dropped = true;
            }

        }
    }
}
