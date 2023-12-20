using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class PathFindings : MonoBehaviour
{
    private List<Node> _openList = new List<Node>();
    private List<Node> _closedList = new List<Node>();

    private int _distance = 10;

    private Grid _grid;

    private void Awake()
    {
        _grid = GetComponent<Grid>();
    }

    private void Update()
    {
        FindThePath();
    }

    private void FindThePath()
    {
        Node startNode = _grid.NodeRequest(_grid.Start.transform.position);
        Node goalNode = _grid.NodeRequest(_grid.Goal.transform.position);

        _openList.Add(startNode);

        while(_openList.Count > 0)
        {
            Node currentNode = _openList[0];
            for(int i = 1; i< _openList.Count; i++)
            {
                if (_openList[i].FCost < currentNode.FCost || _openList[i].FCost == currentNode.FCost && _openList[i].HCost < currentNode.HCost)
                {
                    currentNode = _openList[i];
                }
            }
            _openList.Remove(currentNode);
            _closedList.Add(currentNode);

            if(currentNode == goalNode)
            {
                PathTracer(startNode, goalNode);
                return;
            }

            foreach(Node neighbour in _grid.GetNeighborNodes(currentNode))
            {
                if(!neighbour.Walkable || _closedList.Contains(neighbour))
                {
                    continue;
                }

                int calcMoveCost = currentNode.GCost + GetDistance(currentNode, neighbour);
                if(calcMoveCost < neighbour.GCost || !_openList.Contains(neighbour))
                {
                    neighbour.GCost = calcMoveCost;
                    neighbour.HCost = GetDistance(neighbour, goalNode);

                    neighbour.ParentNode = currentNode;
                    if(!_openList.Contains(neighbour))
                    {
                        _openList.Add(neighbour);
                    }
                }
            }
        }
    }

    private void PathTracer(Node startNode, Node goalNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = goalNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.ParentNode;
        }

        path.Reverse();
        _grid.Path = path;
    }

    private int GetDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(a.PosX - b.PosX);
        int distZ = Mathf.Abs(a.PosZ - b.PosZ);

        return _distance * (distX + distZ);
    }
}
