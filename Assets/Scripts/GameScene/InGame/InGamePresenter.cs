using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InGamePresenter : MonoBehaviour
{
    /// <summary>
    /// View
    /// </summary>
    [SerializeField]
    private InGameView _view;

    /// <summary>
    /// パックマン
    /// </summary>
    [SerializeField]
    private PackManController _packMan;

    /// <summary>
    /// 敵
    /// </summary>
    [SerializeField]
    private List<PathFindings> _enemy;

    /// <summary>
    /// ゴーストマネージャー
    /// ゴースト全体を管理する
    /// </summary>
    [SerializeField]
    private GhostManager _ghostManager;

    /// <summary>
    /// アイテムスポナー
    /// </summary>
    [SerializeField]
    private PickupItemSpawner _pickupItemSpawner;

    /// <summary>
    /// オーディオソース
    /// </summary>
    [SerializeField]
    private AudioSource _audioSource;

    /// <summary>
    /// 敵が歩いている効果音
    /// </summary>
    [SerializeField]
    private AudioClip _enemyWalkSFX;

    /// <summary>
    /// 敵が歩く効果音の長さ
    /// </summary>
    private float _enemyWalkSFXLenght;

    /// <summary>
    /// 最後に敵が歩く効果音を鳴らした時
    /// </summary>
    private float _lastEnemyWalkSFXTime = 0;

    /// <summary>
    /// ゲーム開始時の効果音
    /// </summary>
    [SerializeField]
    private AudioClip _startSFX;

    /// <summary>
    /// ゲームが開始されているかどうか
    /// </summary>
    private bool _isStart;

    private async void Start()
    {
        await _view.Initialize();

        _pickupItemSpawner.Initialize();

        _enemyWalkSFXLenght = _enemyWalkSFX.length;

        _audioSource.clip = _startSFX;
        _audioSource.Play();

        Bind();
        SetEvent();
    }

    /// <summary>
    /// バインド
    /// </summary>
    private void Bind()
    {
        _ghostManager.FrightenProp
            .Subscribe(_ =>
                _packMan.ResetEatEnemyAmount()
            ).AddTo(this);

        _packMan.OnEatPelletAsObservable
            .Subscribe(_ghostManager.AddNowGetScore).AddTo(this);
    }

    /// <summary>
    /// イベント設定
    /// </summary>
    private void SetEvent()
    {
        _view.CanStartProp
            .Subscribe(canStart =>
                _isStart = canStart
            ).AddTo(this);

        _packMan.OnResetAsObservable
            .Subscribe(_ => Reset()).AddTo(this);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        if (!_isStart || _packMan.IsEnemyHit || _packMan.IsEnemyEat)
            return;

        _lastEnemyWalkSFXTime -= Time.deltaTime;
        if(_lastEnemyWalkSFXTime < 0)
        {
            _lastEnemyWalkSFXTime += _enemyWalkSFXLenght;
            _audioSource.clip = _enemyWalkSFX;
            _audioSource.Play();
        }

        foreach(var enemy in _enemy)
        {
            enemy.ManualUpdate(deltaTime);
        }

        _packMan.ManualUpdate(deltaTime);
    }

    /// <summary>
    /// リセット
    /// パックマンが敵に当たったときに呼ばれる
    /// </summary>
    private void Reset()
    {
        _view.ResetView();
        _pickupItemSpawner.Reset();

        _lastEnemyWalkSFXTime = 0;
    }
}
