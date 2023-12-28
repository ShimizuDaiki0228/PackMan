using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkyController : PathFindings
{


    protected override void Start()
    {
        base.Start();

        PointsToCollect = 0;
        Released = true;
    }

    protected override void PathTracer(Node startNode, Node goalNode)
    {
        base.PathTracer(startNode, goalNode);

        _grid.BlinkyPath = _path;
    }

    public override void Reset()
    {
        base.Reset();

        StateProp.Value = GhostStates.SCATTER;
    }
}
