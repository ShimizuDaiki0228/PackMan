using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    /// <summary>
    /// ���C�t
    /// </summary>
    private ReactiveProperty<int> _lifesProp;
    public ReactiveProperty<int> LifeProp => _lifesProp;
    public int Life => _lifesProp.Value;

    /// <summary>
    /// �X�R�A
    /// </summary>
    private ReactiveProperty<int> _scoreProp;
    public ReactiveProperty<int> ScoreProp => _scoreProp;
    public int Score => _scoreProp.Value;

    /// <summary>
    /// ���x��
    /// </summary>
    private ReactiveProperty<int> _levelProp;
    public ReactiveProperty<int> LevelProp => _levelProp;
    private int Level => _levelProp.Value;

    /// <summary>
    /// �a�̍��v��
    /// </summary>
    private int _pelletAmount;

    /// <summary>
    /// Frighten��Ԃ��ǂ������Ď�����I�u�U�[�o�[
    /// PowerPellet�X�N���v�g���ύX���󂯂邽��public�ɂ��Ă���
    /// </summary>
    public Subject<Unit> OnFrightenSubject = new Subject<Unit>();
    public IObservable<Unit> OnFrightenAsObservable => OnFrightenSubject.AsObservable();

    /// <summary>
    /// �G�̏������Z�b�g����
    /// �G�ƏՓ˂����ꍇ�ɔ��΂����
    /// InGameView�ŕ\�����Ă���p�b�N�}���̃��C�t�����炷
    /// </summary>
    private Subject<Unit> _onLoseLifeSubject = new Subject<Unit>();
    public IObservable<Unit> OnLoseLifeAsObservable
        => _onLoseLifeSubject.AsObservable();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        _scoreProp = new ReactiveProperty<int>(0);
        _levelProp = new ReactiveProperty<int>(1);
        _lifesProp = new ReactiveProperty<int>(3);
    }

    /// <summary>
    /// �a��ǉ�����
    /// �X�e�[�W�������ɌĂяo�����
    /// </summary>
    public void AddPellet()
    {
        _pelletAmount++;
    }

    /// <summary>
    /// �a������������ɌĂ΂��
    /// </summary>
    /// <param name="score"></param>
    public void ReducePellet(int score)
    {
        _pelletAmount--;
        AddScore(score);
        if( _pelletAmount <= 0 )
        {
            WinCondition();
        }
    }

    /// <summary>
    /// �a�����ׂĉ�����I��莟�̃��x���ɑJ�ڂ���
    /// </summary>
    private void WinCondition()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        _levelProp.Value++;

        if (Score >= (Level - 1) * 3000 && Life < 4)
        {
            _lifesProp.Value++;
        }
    }

    /// <summary>
    /// ���C�t�����炷
    /// 0�ȉ��̏ꍇ�̓V�[���J�ځA�I����ʂɈړ�����
    /// �G��������Ԃɖ߂�
    /// �p�b�N�}���Ɋւ��Ă�PackManController�ŏ�����Ԃɖ߂��Ă���
    /// </summary>
    public void LoseLife()
    {
        _onLoseLifeSubject.OnNext(Unit.Default);

        _lifesProp.Value--;
        if (Life <= 0)
        {
            ScoreController.Level = Level;
            ScoreController.Score = Score;

            SceneManager.LoadScene("GameOverScene");

            return;
        }
    }

    /// <summary>
    /// �X�R�A�����_����
    /// </summary>
    /// <param name="addScore"></param>
    public void AddScore(int addScore)
    {
        _scoreProp.Value += addScore;
    }

    /// <summary>
    /// ���Z�b�g
    /// </summary>
    public void Reset()
    {
        _scoreProp.Value = 0;
        _levelProp.Value = 0;
        _lifesProp.Value = 2;
    }
}
