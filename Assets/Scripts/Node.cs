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

    // 0 = �Ȃɂ��Ȃ�, 1 = ��Q��, 2 = �X�^�[�g�n�_, 3 = �S�[��, 4 = �p�b�N�}��
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
