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
    /// ゲームが開始されているかどうか
    /// </summary>
    private bool _isStart;

    private void Start()
    {
        _view.Initialize();

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
    }
}
