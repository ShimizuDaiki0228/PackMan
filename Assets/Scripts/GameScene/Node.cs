using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{

    public int GCost;
    public int HCost;

    public int FCost
    {
        get { return GCost + HCost; }
    }

    public Node ParentNode;

    public int PosX;
    public int PosZ;

    // 0 = なにもない, 1 = 障害物, 2 = スタート地点, 3 = ゴール, 4 = パックマン
    public int State;

    public bool Walkable;

    public Node(int posX, int posZ, int state, bool walkable)
    {
        PosX = posX;
        PosZ = posZ;
        State = state;
        Walkable = walkable;
    }
}
