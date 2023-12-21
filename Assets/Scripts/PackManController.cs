using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackManController : MonoBehaviour
{

    private const float SPEED = 5f;

    
    
    private Vector3 _currentDirection = Vector3.zero;

    private Vector3 _nextPos;
    private Vector3 _destination;
    private Vector3 _direction;

    private bool _canMove;

    [SerializeField]
    private LayerMask _unwalkableLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        Reset();
    }

    private void Reset()
    {
        _currentDirection = InGameConst.Up;
        _nextPos = Vector3.forward;
        _destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, _destination, SPEED * Time.deltaTime);

        if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            _nextPos = Vector3.forward;
            _currentDirection = InGameConst.Up;
        }
        else if(Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _nextPos = Vector3.back;
            _currentDirection = InGameConst.Down;
        }
        else if(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _nextPos = Vector3.left;
            _currentDirection = InGameConst.Left;
        }
        else if(Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            _nextPos = Vector3.right;
            _currentDirection = InGameConst.Right;
        }

        if(Vector3.Distance(transform.position, _destination) < 0.0001f)
        {
            transform.localEulerAngles = _currentDirection;
            //if(_canMove)
            {
                if(MoveValid())
                {
                    _destination = transform.position + _nextPos;
                    _direction = _nextPos;
                    //_canMove = true;
                }
            }
        }
    }

    /// <summary>
    /// ìÆÇØÇÈèÛë‘Ç©Ç«Ç§Ç©
    /// ñ⁄ÇÃëOÇ™ï«Ç≈Ç†ÇÍÇŒfalseÇï‘ÇµëOÇ…à⁄ìÆÇµÇ»Ç¢ÇÊÇ§Ç…Ç∑ÇÈ
    /// </summary>
    /// <returns></returns>
    private bool MoveValid()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 0.25f, 0), transform.forward);
        RaycastHit raycastHit;

        if(Physics.Raycast(ray, out raycastHit, 1f, _unwalkableLayerMask))
        {
            if(raycastHit.collider.tag == "Wall")
            {
                return false;
            }
        }

        return true;
    }
}
