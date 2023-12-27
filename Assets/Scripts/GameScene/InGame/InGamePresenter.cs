using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class InGamePresenter : MonoBehaviour
{
    /// <summary>
    /// �p�b�N�}��
    /// </summary>
    [SerializeField]
    private PackManController _packMan;

    /// <summary>
    /// �G
    /// </summary>
    [SerializeField]
    private List<PathFindings> _enemy;

    /// <summary>
    /// View
    /// </summary>
    [SerializeField]
    private InGameView _view;

    /// <summary>
    /// �Q�[�����J�n����Ă��邩�ǂ���
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
    /// ���Z�b�g
    /// �p�b�N�}�����G�ɓ��������Ƃ��ɌĂ΂��
    /// </summary>
    private void Reset()
    {
        _view.ResetView();
    }
}
