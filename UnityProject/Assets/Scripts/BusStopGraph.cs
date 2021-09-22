using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameAI.PathFinding;
using UnityEngine.UI;

public class BusStopGraph : MonoBehaviour
{
    Graph<BusStop> mBusStopGraph = new Graph<BusStop>();
    private Rect mExtent = new Rect();

    [SerializeField]
    GameObject VertexPrefab;

    [SerializeField]
    NPC Npc;

    [SerializeField]
    Transform Destination;

    [SerializeField]
    Text StatusText;

    AStarPathFinder<BusStop> mPathFinder = new AStarPathFinder<BusStop>();

    Graph<BusStop>.Vertex mGoal;
    Graph<BusStop>.Vertex mStart;

    LineRenderer mPathViz;

    public Transform[] wayPoints;

    List<List<int>> pairs = new List<List<int>>();

    void CreateGraph()
    {
        pairs.Add(new List<int> { 0, 1 });
        pairs.Add(new List<int> { 1, 2 });
        pairs.Add(new List<int> { 1, 3 });

        foreach (var item in wayPoints)
            mBusStopGraph.AddVertex(new BusStop("stop_" + item.position.x + "_" + item.position.y, item.position.x, item.position.y, item.position.z));

        for (int i = 0; i < pairs.Count; i++)
        {
            mBusStopGraph.AddDirectedEdge(mBusStopGraph.Vertices[pairs[i][0]], mBusStopGraph.Vertices[pairs[i][1]], BusStop.Distance(mBusStopGraph.Vertices[pairs[i][0]].Value, mBusStopGraph.Vertices[pairs[i][1]].Value));
            mBusStopGraph.AddDirectedEdge(mBusStopGraph.Vertices[pairs[i][1]], mBusStopGraph.Vertices[pairs[i][0]], BusStop.Distance(mBusStopGraph.Vertices[pairs[i][1]].Value, mBusStopGraph.Vertices[pairs[i][0]].Value));
        }
    }

    void Start()
    {
        CreateGraph();

        for (int i = 0; i < mBusStopGraph.Vertices.Count; ++i)
        {
            Vector3 pos = Vector3.zero;
            pos.x = mBusStopGraph.Vertices[i].Value.Point.x;
            pos.y = mBusStopGraph.Vertices[i].Value.Point.y;
            pos.z = mBusStopGraph.Vertices[i].Value.Point.z;

            GameObject obj = Instantiate(VertexPrefab, pos, Quaternion.identity);
            obj.name = mBusStopGraph.Vertices[i].Value.Name;

            Vertex_Viz vertexViz = obj.AddComponent<Vertex_Viz>();
            vertexViz.SetVertex(mBusStopGraph.Vertices[i]);
        }

        Npc.transform.position = new Vector3(mBusStopGraph.Vertices[0].Value.Point.x, mBusStopGraph.Vertices[0].Value.Point.y, -1.0f);

        mStart = mBusStopGraph.Vertices[0];

        mPathFinder.HeuristicCost = BusStop.GetManhattanCost;
        mPathFinder.NodeTraversalCost = BusStop.GetEuclideanCost;
        mPathFinder.onSuccess = OnPathFound;
        mPathFinder.onFailure = OnPathNotFound;

        // We create a line renderer to show the path.
        mPathViz = transform.gameObject.AddComponent<LineRenderer>();
        mPathViz.startWidth = 0.2f;
        mPathViz.endWidth = 0.2f;
        mPathViz.startColor = Color.magenta;
        mPathViz.endColor = Color.magenta;

        mGoal = mBusStopGraph.Vertices[2];
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
        PathFinder<BusStop>.PathFinderNode node = mPathFinder.CurrentNode;

        List<BusStop> reverse_indices = new List<BusStop>();

        while (node != null)
        {
            reverse_indices.Add(node.Location.Value);
            node = node.Parent;
        }

        mPathViz.positionCount = reverse_indices.Count;
        for (int i = reverse_indices.Count - 1; i >= 0; i--)
        {
            Npc.AddWayPoint(new Vector3(reverse_indices[i].Point.x, reverse_indices[i].Point.y, reverse_indices[i].Point.z));
            mPathViz.SetPosition(i, new Vector3(reverse_indices[i].Point.x, reverse_indices[i].Point.y, reverse_indices[i].Point.z));
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
}
