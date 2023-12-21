using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class Grid : MonoBehaviour
{

    [SerializeField]
    private GameObject _bottomLeft, _topRight;

    //デバッグ
    public GameObject Start, Goal;

    private Node[,] _myGrid;

    public List<Node> Path;

    [SerializeField]
    private LayerMask _unwalkable;

    private int _xStart, _zStart;

    private int _xEnd, _zEnd;

    private int _vCells, _hCells;

    private const int CELL_WIDTH = 1;
    private const int CELL_HEIGHT = 1;

    private void Awake()
    {
        MPGridCreate();
    }

    private void MPGridCreate()
    {
        _xStart = (int)_bottomLeft.transform.position.x;
        _zStart = (int)_bottomLeft.transform.position.z;

        _xEnd = (int)_topRight.transform.position.x;
        _zEnd = (int)_topRight.transform.position.z;

        _hCells = (int)((_xEnd - _xStart) / CELL_WIDTH);
        _vCells = (int)((_zEnd - _zStart) / CELL_HEIGHT);

        _myGrid = new Node[_hCells + 1, _vCells + 1];

        UpdateGrid();
    }

    public void UpdateGrid()
    {
        for(int i = 0; i <= _hCells; i++)
        {
            for(int j = 0; j <= _vCells; j++)
            {
                bool walkable = !(Physics.CheckSphere(new Vector3(_xStart + i, 0, _zStart + j), 0.4f, _unwalkable));

                _myGrid[i, j] = new Node(i, j, 0, walkable);
            }
        }
    }

    private void OnDrawGizmos()
    {
         if(_myGrid != null)
        {
            foreach(Node node in _myGrid)
            {
                Gizmos.color = (node.Walkable) ? Color.white : Color.red;

                if(Path != null)
                {
                    if(Path.Contains(node))
                    {
                        Gizmos.color = Color.green;
                    }
                }

                Gizmos.DrawWireCube(new Vector3(_xStart + node.PosX, 0.5f, _zStart + node.PosZ), new Vector3(0.8f, 0.8f, 0.8f));
            }
        }
    }

    public Node NodeRequest(Vector3 pos)
    {
        int gridX = (int)Vector3.Distance(new Vector3(pos.x, 0, 0), new Vector3(_xStart, 0, 0));
        int gridZ = (int)Vector3.Distance(new Vector3(0, 0, pos.z), new Vector3(0 , 0, _zStart));

        return _myGrid[gridX, gridZ];
    }

    public Vector3 NextPathPoint(Node node)
    {
        int gridX = (int)(_xStart + node.PosX);
        int gridZ = (int)(_zStart + node.PosZ);

        return new Vector3(gridX, 0, gridZ);
    }

    public List<Node> GetNeighborNodes(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for(int z = -1; z <= 1; z++)
            {
                if(x == 0 && z == 0)
                {
                    continue;
                }

                if (x == -1 && z == 1)
                    continue;

                if (x == 1 && z == 1)
                    continue;

                if (x == -1 && z == -1)
                    continue;

                if (x == 1 && z == -1)
                    continue;

                int checkPosX = node.PosX + x;
                int checkPosZ = node.PosZ + z;

                if((checkPosX >= 0 && checkPosX < (_hCells + 1)) && (checkPosZ >= 0 && checkPosZ < (_vCells + 1)))
                {
                    neighbours.Add(_myGrid[checkPosX, checkPosZ]);
                }
            } 
        }

        return neighbours;
    }
}
