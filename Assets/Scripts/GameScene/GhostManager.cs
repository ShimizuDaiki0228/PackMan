using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UniRx;
using UnityEditor;

public class GhostManager : MonoBehaviour
{
    /// <summary>
    /// �G�̃��X�g
    /// </summary>
    [SerializeField]
    private List<GameObject> _ghostList = new List<GameObject>();


    /// <summary>
    /// ���݂̓G�̏��
    /// </summary>
    private bool _scatter;
    private bool _chase;
    private readonly ReactiveProperty<bool> _frightenProp = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> FrightenProp => _frightenProp;
    private bool _frighten => _frightenProp.Value;

    /// <summary>
    /// scatter��Ԃł��鎞��
    /// </summary>
    private const float SCATTER_TIMER = 7f;
    private float _currentScatterTimer = 0f;

    /// <summary>
    /// chase��Ԃł��鎞��
    /// </summary>
    private const float CHASE_TIMER = 20f;
    private float _currentChaseTimer = 0f;

    /// <summary>
    /// frighten��Ԃł��鎞��
    /// </summary>
    private const float FRIGHTEND_ALERT_TIMER = 5f;
    private const float FRIGHTEND_TIMER = 7f;
    private float _currentFrightendTimer = 0f;

    /// <summary>
    /// ������Ԃ����낻���������̂�m�点�锒���S�[�X�g
    /// �S�S�[�X�g��
    /// </summary>
    [SerializeField]
    private GameObject[] _ghostFrightenCancelBody;

    private bool _isAlert;

    /// <summary>
    /// ���ݎ擾���Ă���X�R�A
    /// �Q�[���I�[�o�[�⎟�̃��x���Ɉړ������0�ɂȂ�
    /// �܂��a��H�ׂ����݂̂̃X�R�A�̍��v
    /// </summary>
    private int _nowGetScore = 0;

    private void Start()
    {
        Reset();

        SetEvent(); 
    }

    /// <summary>
    /// �C�x���g�ݒ�
    /// </summary>
    private void SetEvent()
    {
        GameManager.Instance.OnFrightenAsObservable
            .Subscribe(_ =>
            {
                _frightenProp.Value = true;
            }
            ).AddTo(this);

        GameManager.Instance.OnLoseLifeAsObservable
            .Subscribe(_ =>
                GhostReset()
            ).AddTo(this);
    }

    private void Update()
    {
        if (GameManager.Instance.Life == 0)
            return;

        Timing();
    }

    private void Timing()
    {
        UpdateStates();
        if (_chase)
        {
            _currentChaseTimer += Time.deltaTime;
            if (_currentChaseTimer >= CHASE_TIMER)
            {
                _currentChaseTimer = 0f;
                _chase = false;
                _scatter = true;
            }
        }
        if (_scatter)
        {
            _currentScatterTimer += Time.deltaTime;
            if (_currentScatterTimer >= SCATTER_TIMER)
            {
                _currentScatterTimer = 0f;
                _chase = true;
                _scatter = false;
            }
        }
        if (_frighten)
        {
            if (_currentChaseTimer != 0 || _currentScatterTimer != 0)
            {
                _chase = false;
                _scatter = false;
                _currentChaseTimer = 0;
                _currentScatterTimer = 0;
            }


            _currentFrightendTimer += Time.deltaTime;
            if(_currentFrightendTimer >= FRIGHTEND_ALERT_TIMER)
            {
                if(!_isAlert)
                {
                    _isAlert = true;
                    FrightenAlertMode().Forget();
                }

                if (_currentFrightendTimer >= FRIGHTEND_TIMER)
                {
                    _currentFrightendTimer = 0f;
                    _chase = true;
                    _scatter = false;
                    _frightenProp.Value = false;
                    _isAlert = false;

                    foreach(var body in _ghostFrightenCancelBody)
                        body.SetActive(false);
                }
            }
        }

    }

    /// <summary>
    /// ������Ԃ���ʏ�̏�Ԃɖ߂肻���ɂȂ鎞�Ƀ��[�U�[�Ɍx�����邽�߂ɃS�[�X�g�̌����ڂ�ύX����
    /// </summary>
    /// <returns></returns>
    private async UniTask FrightenAlertMode()
    {
        while(_frighten)
        {
            foreach(var body in _ghostFrightenCancelBody)
                body.SetActive(!body.activeSelf);

            await UniTask.WaitForSeconds(0.25f);
        }
    }

    private void UpdateStates()
    {
        foreach (var ghost in _ghostList)
        {
            PathFindings pGhost = ghost.GetComponent<PathFindings>();
            if (pGhost.State == GhostStates.CHASE && _scatter)
            {
                pGhost.StateProp.Value = GhostStates.SCATTER;
            }
            else if (pGhost.State == GhostStates.SCATTER && _chase)
            {
                pGhost.StateProp.Value = GhostStates.CHASE;
            }
            else if (pGhost.State != GhostStates.HOME && pGhost.State != GhostStates.GOT_EATEN && _frighten)
            {
                pGhost.StateProp.Value = GhostStates.FRIGHTEND;
            }
            else if (pGhost.State == GhostStates.FRIGHTEND)
            {
                pGhost.StateProp.Value = GhostStates.CHASE;
            }
        }
    }

    /// <summary>
    /// �G�𓮂����Ԃɂ��邩�ǂ������m�F����
    /// </summary>
    private void GhostRelease()
    {
        foreach (var ghost in _ghostList)
        {
            PathFindings pGhost = ghost.GetComponent<PathFindings>();
            if (_nowGetScore >= pGhost.PointsToCollect && !pGhost.Released)
            {
                pGhost.StateProp.Value = GhostStates.CHASE;
                pGhost.Released = true;
            }
        }
    }

    /// <summary>
    /// ���Z�b�g
    /// </summary>
    private void Reset()
    {
        _currentChaseTimer = 0;
        _currentFrightendTimer = 0;
        _currentScatterTimer = 0;

        _scatter = true;
        _chase = false;
        _frightenProp.Value = false;
        _isAlert = false;
        _nowGetScore = 0;

        foreach(var body in _ghostFrightenCancelBody)
            body.SetActive(false);
    }

    /// <summary>
    /// �G�̏�Ԃ����Z�b�g����
    /// </summary>
    private void GhostReset()
    {
        foreach (GameObject ghost in _ghostList)
        {
            ghost.GetComponent<PathFindings>().Reset();
        }

        Reset();
    }

    /// <summary>
    /// ���݂̃Q�[���Ŏ擾���Ă���_�������Z����
    /// </summary>
    public void AddNowGetScore(int score)
    {
        _nowGetScore += score;

        GhostRelease();
    }
}
