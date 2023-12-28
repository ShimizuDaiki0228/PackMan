using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro.EditorUtilities;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UniRx;

/// <summary>
/// ステートマシン
/// </summary>
public enum GhostStates
{
    HOME,
    LEAVING_HOME,
    CHASE,
    SCATTER,
    FRIGHTEND,
    GOT_EATEN
}

public enum Ghosts
{
    BLINKY,
    CLYDE,
    INKY,
    PINKY
}

public class PathFindings : MonoBehaviour
{
    protected List<Node> _path = new List<Node>();

    private int _distance = 10;

    [SerializeField]
    protected Grid _grid;

    protected Transform _currentTarget;

    [SerializeField]
    protected Transform _packManTarget;

    [SerializeField]
    private List<Transform> _homeTarget = new List<Transform>();

    [SerializeField]
    protected List<Transform> _scatterTarget = new List<Transform>();

    private Vector3 _nextPos;
    protected Vector3 _destination;

    private Vector3 _currentDirection = Vector3.zero;

    private float _speed = 3f;

    private Node _lastVisitedNode;

    [SerializeField]
    private GameObject[] _appearance;

    public ReactiveProperty<GhostStates> StateProp;
    public GhostStates State => StateProp.Value;

    private const float HOME_TIMER = 3f;
    private float _currentTimer = 0f;

    public int PointsToCollect;
    public bool Released = false;

    private Vector3 _initPosition;
    private GhostStates _initState;

    protected virtual void Start()
    {
        _initPosition = transform.position;
        _initState = State;
        _destination = transform.position;
        _currentDirection = InGameConst.Up;

        foreach (var target in _scatterTarget)
        {
            target.GetComponent<MeshRenderer>().enabled = false;
        }
        foreach (var target in _homeTarget)
        {
            target.GetComponent<MeshRenderer>().enabled = false;
        }

        StateProp.Subscribe(state =>
                SetAppearance(state)
            ).AddTo(this);
    }

    /// <summary>
    /// 手動Update
    /// </summary>
    public void ManualUpdate(float deltaTime)
    {
        CheckState();
    }

    protected virtual void FindThePath()
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

    protected virtual void PathTracer(Node startNode, Node goalNode)
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

    protected virtual void CheckState()
    {
        switch(State)
        {
            case GhostStates.HOME:
                _speed = 1.5f;

                TargetContainsCheck(_homeTarget);
                SetTarget(_homeTarget);

                if(Released)
                {
                    _currentTimer += Time.deltaTime;
                    if( _currentTimer >= HOME_TIMER)
                    {
                        _currentTimer = 0;
                        StateProp.Value = GhostStates.CHASE;
                    }
                }

                Move();
                break;

            case GhostStates.LEAVING_HOME:
                break;

            case GhostStates.CHASE:
                _currentTarget = _packManTarget;
                _speed = 3f;

                Move();
                break;

            case GhostStates.SCATTER:
                _speed = 3f;

                TargetContainsCheck(_scatterTarget);
                SetTarget(_scatterTarget);

                Move();

                break;

            case GhostStates.FRIGHTEND:
                _speed = 1.5f;

                TargetContainsCheck(_homeTarget);
                SetTarget(_homeTarget);

                Move();
                break;

            case GhostStates.GOT_EATEN:
                _speed = 7f;
                _currentTarget = _homeTarget[0];

                if (Vector3.Distance(transform.position, _homeTarget[0].position) < 0.0001f)
                {
                    StateProp.Value = GhostStates.HOME;
                }
                Move();

                break;
        }
    }

    /// <summary>
    /// 現在のターゲットが対象となるターゲットのリストの要素のどれかと一致しているか
    /// 一致している場合、現在の状態に対して正しく動けているということになる
    /// </summary>
    /// <param name="targetList">対象となるターゲットのTransformリスト</param>
    private void TargetContainsCheck(List<Transform> targetList)
    {
        if (!targetList.Contains(_currentTarget))
        {
            _currentTarget = targetList[0];
        }
    }

    /// <summary>
    /// 現在のターゲットを設定する
    /// </summary>
    private void SetTarget(List<Transform> targetList)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            if (Vector3.Distance(transform.position, targetList[i].position) < 0.0001f && _currentTarget == targetList[i])
            {
                _currentTarget = targetList[(i + 1) % targetList.Count];
            }
        }
    }

    /// <summary>
    /// GhostStatesによって見た目を変更する
    /// </summary>
    /// <param name="state"></param>
    private void SetAppearance(GhostStates state)
    {
        int activeAppearanceNumber;

        if(state == GhostStates.HOME || state == GhostStates.LEAVING_HOME || state == GhostStates.CHASE || state == GhostStates.SCATTER)
            activeAppearanceNumber = 0;

        else if(state == GhostStates.FRIGHTEND)
            activeAppearanceNumber = 1;

        else
            activeAppearanceNumber = 2;

        for (int i = 0; i < _appearance.Length; i++)
        {
            _appearance[i].SetActive(i == activeAppearanceNumber);
        }
    }

    /// <summary>
    /// リセット
    /// </summary>
    public virtual void Reset()
    {
        transform.position = _initPosition;
        StateProp.Value = _initState;

        _destination = transform.position;

        _currentDirection = InGameConst.Up;

        transform.localEulerAngles = new Vector3(0, 0, 0);
    }
}
