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
    /// �S�[�X�g�}�l�[�W���[
    /// �S�[�X�g�S�̂��Ǘ�����
    /// </summary>
    [SerializeField]
    private GhostManager _ghostManager;

    /// <summary>
    /// �Q�[�����J�n����Ă��邩�ǂ���
    /// </summary>
    private bool _isStart;

    private void Start()
    {
        _view.Initialize();

        Bind();
        SetEvent();
    }

    /// <summary>
    /// �o�C���h
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
    /// �C�x���g�ݒ�
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
    /// ���Z�b�g
    /// �p�b�N�}�����G�ɓ��������Ƃ��ɌĂ΂��
    /// </summary>
    private void Reset()
    {
        _view.ResetView();
    }
}
