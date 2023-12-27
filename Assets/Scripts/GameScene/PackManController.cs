using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using DG.Tweening;

public class PackManController : MonoBehaviour
{
    private const float SPEED = 5f;
    
    private Vector3 _currentDirection = Vector3.zero;

    private Vector3 _nextPos;
    private Vector3 _destination;

    [SerializeField]
    private LayerMask _unwalkableLayerMask;

    /// <summary>
    /// リセット
    /// </summary>
    private Vector3 _initPosition;

    /// <summary>
    /// リセットされることを知らせるオブザーバー
    /// </summary>
    private Subject<Unit> _onResetSubject = new Subject<Unit>();
    public IObservable<Unit> OnResetAsObservable => _onResetSubject.AsObservable();

    /// <summary>
    /// 消えるときのエフェクト
    /// </summary>
    [SerializeField]
    private GameObject _vanishingEffectPrefab;

    /// <summary>
    /// 敵に当たったかどうか
    /// </summary>
    public bool IsEnemyHit;

    /// <summary>
    /// 敵に当たって何秒後に消えるようにするか
    /// </summary>
    private const float DELAY_VANISH_TIME = 0.5f;

    /// <summary>
    /// 敵に当たって消滅してから再開するまでにかかる時間
    /// </summary>
    private const float DELEY_RESTART_TIME = 2f;

    /// <summary>
    /// 敵を食べた時にゲットする得点
    /// </summary>
    private const int ENEMY_EAT_POINT = 400;

    // Start is called before the first frame update
    void Start()
    {
        _initPosition = transform.position;

        Reset();
    }

    public void Reset()
    {
        transform.position = _initPosition;
        _currentDirection = InGameConst.Up;
        _nextPos = Vector3.forward;
        _destination = transform.position;
    }

    /// <summary>
    /// 手動Update
    /// </summary>
    /// <param name="deltaTime"></param>
    public void ManualUpdate(float deltaTime)
    {
        Move(deltaTime);
    }

    private void Move(float deltaTime)
    {
        transform.position = Vector3.MoveTowards(transform.position, _destination, SPEED * deltaTime);

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
            {
                if(MoveValid())
                {
                    _destination = transform.position + _nextPos;
                }
            }
        }
    }

    /// <summary>
    /// 動ける状態かどうか
    /// 目の前が壁であればfalseを返し前に移動しないようにする
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

    private async void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ghost")
        {
            PathFindings pGhost = other.GetComponent<PathFindings>();
            if(pGhost.State == GhostStates.FRIGHTEND)
            {
                pGhost.State = GhostStates.GOT_EATEN;
                GameManager.Instance.AddScore(ENEMY_EAT_POINT);
            }
            else if(pGhost.State != GhostStates.FRIGHTEND && pGhost.State != GhostStates.GOT_EATEN)
            {
                IsEnemyHit = true;

                await HitEnemy(); 

                GameManager.Instance.LoseLife();
                Reset();

                //ライフが0でない場合は再度リスタートする際のテキストを表示する
                if (GameManager.Instance.Life != 0)
                    _onResetSubject.OnNext(Unit.Default);

                IsEnemyHit = false;
            }
        }
    }

    /// <summary>
    /// 敵と衝突したときに呼ばれる
    /// </summary>
    private async UniTask HitEnemy()
    {
        await UniTask.WaitForSeconds(DELAY_VANISH_TIME);

        await gameObject.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);

        //消滅するエフェクトを生成
        EffectCreate(_vanishingEffectPrefab);

        await UniTask.WaitForSeconds(DELEY_RESTART_TIME);

        gameObject.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// エフェクトを生成する
    /// エフェクトのStart Life Timeを過ぎると破棄する
    /// </summary>
    private void EffectCreate(GameObject effectPrefab)
    {
        var effect = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        var mainModule = effectPrefab.GetComponent<ParticleSystem>().main;
        float lifeTime = mainModule.startLifetime.constant;

        Destroy(effect, lifeTime);
    }
}
