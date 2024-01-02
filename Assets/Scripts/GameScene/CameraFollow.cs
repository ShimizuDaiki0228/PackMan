using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    [SerializeField]
    private Transform _target;

    private float _distX;
    private float _distZ;

    private Vector3 _pos;

    private const float SPEED = 1;

    private Vector3 _initPosition;

    // Start is called before the first frame update
    void Start()
    {
        _initPosition = transform.position;

        _distX = transform.position.x - _target.position.x;
        _distZ = transform.position.z - _target.position.z;

        SetEvent();
    }

    /// <summary>
    /// ÉCÉxÉìÉgê›íË
    /// </summary>
    private void SetEvent()
    {
        GameManager.Instance.LifeProp
            .Subscribe(_ => Reset()).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        _pos.x = _target.position.x + _distX;
        _pos.y = transform.position.y;
        _pos.z = _target.position.z + _distZ;

        transform.position = Vector3.Lerp(transform.position, _pos, SPEED * Time.deltaTime);
    }

    private void Reset()
    {
        transform.position = _initPosition;
    }
}
