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
    /// 敵のリスト
    /// </summary>
    [SerializeField]
    private List<GameObject> _ghostList = new List<GameObject>();


    /// <summary>
    /// 現在の敵の状態
    /// </summary>
    private bool _scatter;
    private bool _chase;
    private bool _frighten;

    /// <summary>
    /// scatter状態である時間
    /// </summary>
    private const float SCATTER_TIMER = 7f;
    private float _currentScatterTimer = 0f;

    /// <summary>
    /// chase状態である時間
    /// </summary>
    private const float CHASE_TIMER = 20f;
    private float _currentChaseTimer = 0f;

    /// <summary>
    /// frighten状態である時間
    /// </summary>
    private const float FRIGHTEND_TIMER = 5f;
    private float _currentFrightendTimer = 0f;

    private void Start()
    {
        Reset();

        SetEvent(); 
    }

    /// <summary>
    /// イベント設定
    /// </summary>
    private void SetEvent()
    {
        GameManager.Instance.ScoreProp
            .Subscribe(GhostRelease).AddTo(this);

        GameManager.Instance.OnFrightenAsObservable
            .Subscribe(_ =>
            {
                _frighten = true;
            }
            ).AddTo(this);

        GameManager.Instance.OnGhostResetAsObservable
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
            if (_currentFrightendTimer >= FRIGHTEND_TIMER)
            {
                _currentFrightendTimer = 0f;
                _chase = true;
                _scatter = false;
                _frighten = false;
            }
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
    /// 敵を動ける状態にするかどうかを確認する
    /// </summary>
    private void GhostRelease(int score)
    {
        foreach (var ghost in _ghostList)
        {
            PathFindings pGhost = ghost.GetComponent<PathFindings>();
            if (score >= pGhost.PointsToCollect && !pGhost.Released)
            {
                pGhost.StateProp.Value = GhostStates.CHASE;
                pGhost.Released = true;
            }
        }
    }

    /// <summary>
    /// リセット
    /// </summary>
    private void Reset()
    {
        _currentChaseTimer = 0;
        _currentFrightendTimer = 0;
        _currentScatterTimer = 0;

        _scatter = true;
        _chase = false;
        _frighten = false;
    }

    /// <summary>
    /// 敵の状態をリセットする
    /// </summary>
    private void GhostReset()
    {
        foreach (GameObject ghost in _ghostList)
        {
            ghost.GetComponent<PathFindings>().Reset();
        }

        Reset();
    }
}
