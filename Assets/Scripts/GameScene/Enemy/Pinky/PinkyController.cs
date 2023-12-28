using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkyController : PathFindings
{
    protected override void Start()
    {
        base.Start();
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
        Transform aheadTarget = new GameObject().transform;

        int lookAhead = 4;
        aheadTarget.position = _packManTarget.position + _packManTarget.transform.forward * lookAhead;
        for (int i = lookAhead; i > 0; i--)
        {
            if (!_grid.CheckInsideGrid(aheadTarget.position))
            {
                lookAhead--;
            }
            else
            {
                break;
            }
        }
        aheadTarget.position = _packManTarget.position + _packManTarget.transform.forward * lookAhead;
        Debug.DrawLine(transform.position, aheadTarget.position);
        _currentTarget = aheadTarget;
        Destroy(aheadTarget.gameObject);
    }

    protected override void PathTracer(Node startNode, Node goalNode)
    {
        base.PathTracer(startNode, goalNode);

        _grid.PinkyPath = _path;
    }
}
