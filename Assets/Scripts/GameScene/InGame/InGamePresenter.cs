using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InGamePresenter : MonoBehaviour
{
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
    /// View
    /// </summary>
    [SerializeField]
    private InGameView _view;

    /// <summary>
    /// ゲームが開始されているかどうか
    /// </summary>
    private bool _isStart;

    private void Start()
    {
        _view.Initialize();

        Bind();
    }

    private void Bind()
    {
        _view.CanStartProp
            .Subscribe(canStart =>
                _isStart = canStart
            ).AddTo(this);

        _packMan.OnResetAsObservable
            .Subscribe(_ => Reset() ).AddTo(this);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        if (!_isStart || _packMan.IsEnemyHit)
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
