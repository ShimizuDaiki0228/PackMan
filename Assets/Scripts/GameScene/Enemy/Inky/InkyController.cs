using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InkyController : PathFindings
{
    [SerializeField]
    private Transform _blinky;

    protected override void Start()
    {
        base.Start();

        _ghost = Ghosts.INKY;
    }


    protected override void CheckState()
    {
        base.CheckState();

        if(State == GhostStates.CHASE)
        {
            Behaviour();
        }
    }

    private void Behaviour()
    {
        Transform blinkyToPackman = new GameObject().transform;
        Transform target = new GameObject().transform;
        Transform goal = new GameObject().transform;

        blinkyToPackman.position = new Vector3(_packManTarget.position.x - _blinky.position.x,
                                               0,
                                               _packManTarget.position.z - _blinky.position.z);

        target.position = new Vector3(_packManTarget.position.x + blinkyToPackman.position.x,
                                      0,
                                      _packManTarget.position.z + blinkyToPackman.position.z);

        goal.position = _grid.GetNearestNonWallNode(target.position);
        _currentTarget = goal;
        Debug.DrawLine(transform.position, _currentTarget.position);

        Destroy(blinkyToPackman.gameObject);
        Destroy(target.gameObject);
        Destroy(goal.gameObject);
    }

    protected override void PathTracer(Node startNode, Node goalNode)
    {
        base.PathTracer(startNode, goalNode);

        _grid.InkyPath = _path;
    }
}
