using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClydeController : PathFindings
{
    protected override void Start()
    {
        base.Start();

        PointsToCollect = 20;
    }

    protected override void CheckState()
    {
        base.CheckState();

        if (State == GhostStates.CHASE)
        {
            Behaviour();
        }
    }

    private void Behaviour()
    {
        if (Vector3.Distance(transform.position, _packManTarget.position) <= 8)
        {
            if (!_scatterTarget.Contains(_currentTarget))
            {
                _currentTarget = _scatterTarget[0];
            }

            for (int i = 0; i < _scatterTarget.Count; i++)
            {
                if (Vector3.Distance(transform.position, _scatterTarget[i].position) < 0.0001f && _currentTarget == _scatterTarget[i])
                {
                    _currentTarget = _scatterTarget[(i + 1) % _scatterTarget.Count];
                }
            }
        }
        else
        {
            _currentTarget = _packManTarget;
        }
    }

    protected override void PathTracer(Node startNode, Node goalNode)
    {
        base.PathTracer(startNode, goalNode);

        _grid.ClydePath = _path;
    }
}
