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
    /// ライフ
    /// </summary>
    private ReactiveProperty<int> _lifesProp;
    public ReactiveProperty<int> LifeProp => _lifesProp;
    public int Life => _lifesProp.Value;

    /// <summary>
    /// スコア
    /// </summary>
    private ReactiveProperty<int> _scoreProp;
    public ReactiveProperty<int> ScoreProp => _scoreProp;
    public int Score => _scoreProp.Value;

    /// <summary>
    /// レベル
    /// </summary>
    private ReactiveProperty<int> _levelProp;
    public ReactiveProperty<int> LevelProp => _levelProp;
    private int Level => _levelProp.Value;

    /// <summary>
    /// 餌の合計数
    /// </summary>
    private int _pelletAmount;

    /// <summary>
    /// Frighten状態かどうかを監視するオブザーバー
    /// PowerPelletスクリプトより変更を受けるためpublicにしている
    /// </summary>
    public Subject<Unit> OnFrightenSubject = new Subject<Unit>();
    public IObservable<Unit> OnFrightenAsObservable => OnFrightenSubject.AsObservable();

    /// <summary>
    /// 敵の情報をリセットする
    /// 敵と衝突した場合に発火される
    /// InGameViewで表示しているパックマンのライフを減らす
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
    /// 餌を追加する
    /// ステージ生成時に呼び出される
    /// </summary>
    public void AddPellet()
    {
        _pelletAmount++;
    }

    /// <summary>
    /// 餌を回収した時に呼ばれる
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
    /// 餌をすべて回収し終わり次のレベルに遷移する
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
    /// ライフを減らす
    /// 0以下の場合はシーン遷移、終了画面に移動する
    /// 敵を初期状態に戻す
    /// パックマンに関してはPackManControllerで初期状態に戻している
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
    /// スコアを加点する
    /// </summary>
    /// <param name="addScore"></param>
    public void AddScore(int addScore)
    {
        _scoreProp.Value += addScore;
    }

    /// <summary>
    /// リセット
    /// </summary>
    public void Reset()
    {
        _scoreProp.Value = 0;
        _levelProp.Value = 0;
        _lifesProp.Value = 2;
    }
}
