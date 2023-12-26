using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _levelText;
    [SerializeField]
    private Text _lifesText;

    private void Start()
    {
        GameManager.Instance.ScoreProp
            .Subscribe(score =>
            {
                _scoreText.text = "Score : " + score.ToString();
            }
            ).AddTo(this);

        GameManager.Instance.LevelProp
            .Subscribe(level =>
                _levelText.text = "Level : " + level.ToString()
            ).AddTo(this);

        GameManager.Instance.LifeProp
            .Subscribe(life =>
                _lifesText.text = "Life : " + life.ToString()
            ).AddTo(this);
    }

    /// <summary>
    /// シーンが更新された時に行う処理
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameManager.Instance.ScoreProp
            .Subscribe(score =>
            {
                _scoreText.text = "Score : " + score.ToString();
            }
            ).AddTo(this);

        GameManager.Instance.LevelProp
            .Subscribe(level =>
                _levelText.text = "Level : " + level.ToString()
            ).AddTo(this);

        GameManager.Instance.LifeProp
            .Subscribe(life =>
                _lifesText.text = "Life : " + life.ToString()
            ).AddTo(this);
    }
}
