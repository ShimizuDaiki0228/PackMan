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
    /// �A�C�e���X�|�i�[
    /// </summary>
    [SerializeField]
    private PickupItemSpawner _pickupItemSpawner;

    /// <summary>
    /// �I�[�f�B�I�\�[�X
    /// </summary>
    [SerializeField]
    private AudioSource _audioSource;

    /// <summary>
    /// �G�������Ă�����ʉ�
    /// </summary>
    [SerializeField]
    private AudioClip _enemyWalkSFX;

    /// <summary>
    /// �G���������ʉ��̒���
    /// </summary>
    private float _enemyWalkSFXLenght;

    /// <summary>
    /// �Ō�ɓG���������ʉ���炵����
    /// </summary>
    private float _lastEnemyWalkSFXTime = 0;

    /// <summary>
    /// �Q�[���J�n���̌��ʉ�
    /// </summary>
    [SerializeField]
    private AudioClip _startSFX;

    /// <summary>
    /// �Q�[�����J�n����Ă��邩�ǂ���
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
    /// ���Z�b�g
    /// �p�b�N�}�����G�ɓ��������Ƃ��ɌĂ΂��
    /// </summary>
    private void Reset()
    {
        _view.ResetView();
        _pickupItemSpawner.Reset();

        _lastEnemyWalkSFXTime = 0;
    }
}
