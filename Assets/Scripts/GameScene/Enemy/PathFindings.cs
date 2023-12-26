using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using TMPro.EditorUtilities;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UniRx;

/// <summary>
/// �X�e�[�g�}�V��
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
    /// <summary>
    /// �S�[�X�g�̃^�C�v
    /// </summary>
    protected Ghosts _ghost;

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
    private Vector3 _destination;

    private Vector3 _currentDirection = Vector3.zero;

    private float _speed = 3f;

    private Node _lastVisitedNode;

    

    /// <summary>
    /// ������
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


    public GhostStates State;

    private const float HOME_TIMER = 3f;
    private float _currentTimer = 0f;

    public int PointsToCollect;
    public bool Released = false;

    private static Vector3 _initPosition;
    private static GhostStates _initState;

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
    }

    /// <summary>
    /// �蓮Update
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
                //�ړI�n
                _nextPos = _grid.NextPathPoint(_path[0]);
                _destination = _nextPos;

                //��]
                SetDirection();
                transform.localEulerAngles = _currentDirection;
            }
        }
    }

    /// <summary>
    /// ������ݒ肷��
    /// </summary>
    private void SetDirection()
    {
        int dirX = (int)(_nextPos.x - transform.position.x);
        int dirZ = (int)(_nextPos.z - transform.position.z);

        //�c����
        if(dirX == 0)
        {
            //�����
            if (dirZ > 0)
                _currentDirection = InGameConst.Up;

            //������
            else if(dirZ < 0)
                _currentDirection = InGameConst.Down;
        }
        //������
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
                SetAppearance((int)Appearance.NORMAL);
                _speed = 1.5f;

                TargetContainsCheck(_homeTarget);
                SetTarget(_homeTarget);

                if(Released)
                {
                    _currentTimer += Time.deltaTime;
                    if( _currentTimer >= HOME_TIMER)
                    {
                        _currentTimer = 0;
                        State = GhostStates.CHASE;
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

                TargetContainsCheck(_scatterTarget);
                SetTarget(_scatterTarget);

                Move();

                break;

            case GhostStates.FRIGHTEND:
                SetAppearance((int)Appearance.FRIGHTEND);
                _speed = 1.5f;

                TargetContainsCheck(_homeTarget);
                SetTarget(_homeTarget);

                Move();
                break;

            case GhostStates.GOT_EATEN:
                SetAppearance((int)Appearance.EYESONLY);
                _speed = 7f;
                _currentTarget = _homeTarget[0];

                if (Vector3.Distance(transform.position, _homeTarget[0].position) < 0.0001f)
                {
                    State = GhostStates.HOME;
                }
                Move();

                break;
        }
    }

    /// <summary>
    /// ���݂̃^�[�Q�b�g���ΏۂƂȂ�^�[�Q�b�g�̃��X�g�̗v�f�̂ǂꂩ�ƈ�v���Ă��邩
    /// ��v���Ă���ꍇ�A���݂̏�Ԃɑ΂��Đ����������Ă���Ƃ������ƂɂȂ�
    /// </summary>
    /// <param name="targetList">�ΏۂƂȂ�^�[�Q�b�g��Transform���X�g</param>
    private void TargetContainsCheck(List<Transform> targetList)
    {
        if (!targetList.Contains(_currentTarget))
        {
            _currentTarget = targetList[0];
        }
    }

    /// <summary>
    /// ���݂̃^�[�Q�b�g��ݒ肷��
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
    /// �����ڂ�ύX����
    /// </summary>
    /// <param name="appearanceNumber">�����ڂ̔ԍ�</param>
    private void SetAppearance(int appearanceNumber)
    {
        _activeAppearanceNumber = appearanceNumber;

        for(int i = 0; i < _appearance.Length; i++)
        {
            _appearance[i].SetActive(i == _activeAppearanceNumber);
        }
    }

    /// <summary>
    /// ���Z�b�g
    /// </summary>
    public virtual void Reset()
    {
        transform.position = _initPosition;
        State = _initState;

        _destination = transform.position;
        _currentDirection = InGameConst.Up;
    }
}