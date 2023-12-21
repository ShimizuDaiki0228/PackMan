using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class PathFindings : MonoBehaviour
{
    private List<Node> _path = new List<Node>();

    private int _distance = 10;

    [SerializeField]
    private Grid _grid;

    private Transform _currentTarget;

    [SerializeField]
    private Transform _packManTarget;

    [SerializeField]
    private List<Transform> _homeTarget = new List<Transform>();

    [SerializeField]
    private List<Transform> _scatterTarget = new List<Transform>();

    private Vector3 _nextPos;
    private Vector3 _destination;

    private Vector3 _currentDirection = Vector3.zero;

    private float _speed = 3f;

    private Node _lastVisitedNode;

    /// <summary>
    /// ステートマシン
    /// </summary>
    private enum GhostStates
    {
        HOME,
        LEAVING_HOME,
        CHASE,
        SCATTER,
        FRIGHTEND,
        GOT_EATEN
    }

    /// <summary>
    /// 見た目
    /// _activeAppearanceNumber = 0 : Normal, 1 : Frightend, 2 : Eyes Only 
    /// </summary>
    private enum Appearance
    {
        NORMAL,
        FRIGHTEND,
        EYESONLY
    }
    private int _activeAppearanceNumber;
    [SerializeField]
    private GameObject[] _appearance;


    [SerializeField]
    private GhostStates _state;

    private void Start()
    {
        _destination = transform.position;
        _currentDirection = InGameConst.Up;
        foreach(var target in _scatterTarget)
        {
            target.GetComponent<MeshRenderer>().enabled = false;
        }
        foreach (var target in _homeTarget)
        {
            target.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void Update()
    {
        CheckState();
        //Move();
    }

    private void FindThePath()
    {
        Node startNode = _grid.NodeRequest(transform.position);
        Node goalNode = _grid.NodeRequest(_currentTarget.position);

        List<Node> _openList = new List<Node>();
        List<Node> _closedList = new List<Node>();

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
                if(!neighbour.Walkable || _closedList.Contains(neighbour) || neighbour == _lastVisitedNode)
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
            _lastVisitedNode = null;
        }
    }

    private void PathTracer(Node startNode, Node goalNode)
    {
        _lastVisitedNode = startNode;
        _path.Clear();

        Node currentNode = goalNode;

        while (currentNode != startNode)
        {
            _path.Add(currentNode);
            currentNode = currentNode.ParentNode;
        }

        _path.Reverse();
        _grid.Path = _path;
    }

    private int GetDistance(Node a, Node b)
    {
        int distX = Mathf.Abs(a.PosX - b.PosX);
        int distZ = Mathf.Abs(a.PosZ - b.PosZ);

        return _distance * (distX + distZ);
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, _destination, _speed * Time.deltaTime);
        if(Vector3.Distance(transform.position, _destination) < 0.0001f)
        {
            FindThePath();

            if(_path.Count > 0)
            {
                //目的地
                _nextPos = _grid.NextPathPoint(_path[0]);
                _destination = _nextPos;

                //回転
                SetDirection();
                transform.localEulerAngles = _currentDirection;
            }
        }
    }

    /// <summary>
    /// 向きを設定する
    /// </summary>
    private void SetDirection()
    {
        Debug.Log("Set");
        int dirX = (int)(_nextPos.x - transform.position.x);
        int dirZ = (int)(_nextPos.z - transform.position.z);

        //縦方向
        if(dirX == 0)
        {
            //上方向
            if (dirZ > 0)
                _currentDirection = InGameConst.Up;

            //下方向
            else if(dirZ < 0)
                _currentDirection = InGameConst.Down;
        }
        //横方向
        else if(dirZ == 0)
        {
            if (dirX > 0)
                _currentDirection = InGameConst.Right;

            else if (dirX < 0)
                _currentDirection = InGameConst.Left;
        }
    }

    private void CheckState()
    {
        switch(_state)
        {
            case GhostStates.HOME:
                SetAppearance((int)Appearance.NORMAL);
                _speed = 1.5f;

                if (!_homeTarget.Contains(_currentTarget))
                {
                    _currentTarget = _homeTarget[0];
                }

                for (int i = 0; i < _homeTarget.Count; i++)
                {
                    if (Vector3.Distance(transform.position, _homeTarget[i].position) < 0.0001f && _currentTarget == _homeTarget[i])
                    {
                        _currentTarget = _homeTarget[(i + 1) % _homeTarget.Count];
                    }
                }
                Move();
                break;

            case GhostStates.LEAVING_HOME:
                SetAppearance((int)Appearance.NORMAL);
                break;

            case GhostStates.CHASE:
                SetAppearance((int)Appearance.NORMAL);
                _currentTarget = _packManTarget;
                _speed = 3f;
                Move();
                break;

            case GhostStates.SCATTER:
                SetAppearance((int)Appearance.NORMAL);
                _speed = 3f;
                if(!_scatterTarget.Contains(_currentTarget))
                {
                    _currentTarget = _scatterTarget[0];
                }

                for(int i = 0; i < _scatterTarget.Count; i++)
                {
                    if (Vector3.Distance(transform.position, _scatterTarget[i].position) < 0.0001f && _currentTarget == _scatterTarget[i])
                    {
                        _currentTarget = _scatterTarget[(i+1) % _scatterTarget.Count];
                    }
                }
                Move();

                break;

            case GhostStates.FRIGHTEND:
                SetAppearance((int)Appearance.FRIGHTEND);
                _speed = 1.5f;

                if (!_homeTarget.Contains(_currentTarget))
                {
                    _currentTarget = _homeTarget[0];
                }

                for (int i = 0; i < _homeTarget.Count; i++)
                {
                    if (Vector3.Distance(transform.position, _homeTarget[i].position) < 0.0001f && _currentTarget == _homeTarget[i])
                    {
                        _currentTarget = _homeTarget[(i + 1) % _homeTarget.Count];
                    }
                }

                Move();
                break;

            case GhostStates.GOT_EATEN:
                SetAppearance((int)Appearance.EYESONLY);
                _speed = 7f;
                _currentTarget = _homeTarget[0];

                if (Vector3.Distance(transform.position, _homeTarget[0].position) < 0.0001f)
                {
                    _state = GhostStates.HOME;
                }
                Move();

                break;
        }
    }

    private void SetAppearance(int appearanceNumber)
    {
        _activeAppearanceNumber = appearanceNumber;

        for(int i = 0; i < _appearance.Length; i++)
        {
            _appearance[i].SetActive(i == _activeAppearanceNumber);
        }
    }
}
