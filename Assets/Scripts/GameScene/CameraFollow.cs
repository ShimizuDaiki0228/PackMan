using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField]
    private Transform _target;

    private float _distX;
    private float _distZ;

    private Vector3 _pos;

    private const float SPEED = 1;

    // Start is called before the first frame update
    void Start()
    {
        _distX = transform.position.x - _target.position.x;
        _distZ = transform.position.z - _target.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        _pos.x = _target.position.x + _distX;
        _pos.y = transform.position.y;
        _pos.z = _target.position.z + _distZ;

        transform.position = Vector3.Lerp(transform.position, _pos, SPEED * Time.deltaTime);
    }
}
