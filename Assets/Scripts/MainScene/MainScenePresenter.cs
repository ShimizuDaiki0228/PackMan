using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using Cysharp.Threading.Tasks;

public class MainScenePresenter : MonoBehaviour
{
    /// <summary>
    /// view
    /// </summary>
    [SerializeField]
    private MainSceneView _view;

    /// <summary>
    /// メインシーンのパックマン
    /// </summary>
    [SerializeField]
    private PackManControllerMain _packManMain;

    /// <summary>
    /// メインシーンの敵
    /// </summary>
    [SerializeField]
    private GhostControllerMain[] _ghostControllerMainList;

    /// <summary>
    /// 餌
    /// </summary>
    [SerializeField]
    private GameObject _pellet;

    /// <summary>
    /// でかいパックマン
    /// プレイボタンをクリックした後に出てくる
    /// </summary>
    [SerializeField]
    private BigPackManController _bigPackMan;

    private void Start()
    {
        _view.Initialize();

        SetEvent();
    }

    private void SetEvent()
    {
        _packManMain.IsPowerPelletHitProp
            .Where(isHit => isHit)
            .Subscribe(_ =>
                ChangeEnemyFrightenMode()
            ).AddTo(this);

        _view.IsPlayButtonClickedProp
            .Where(isClicked => isClicked)
            .Subscribe(_ =>
                ChangeScene()
            ).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (_packManMain.IsEnemyHit)
            return;

        _packManMain.ManualUpdate();

        foreach(var ghost in _ghostControllerMainList)
        {
            ghost.ManualUpdate();
        }

        if(_view.IsPlayButtonClicked)
        {
            _bigPackMan.ManualUpdate();
        }
    }

    /// <summary>
    /// ゲームシーンに遷移する
    /// </summary>
    private void ChangeScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings; // ループするためにモジュロ演算子を使用

        _packManMain.gameObject.SetActive(false);
        foreach(var ghost in _ghostControllerMainList)
        {
            if(ghost.gameObject.activeSelf)
                ghost.gameObject.SetActive(false);
        }

        if(_pellet != null)
        {
            if(_pellet.gameObject.activeSelf)
            {
                _pellet.SetActive(false);
            }
        }



        UniTask.WaitForSeconds(5);
        //SceneManager.LoadScene(nextSceneIndex);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.Reset();
        }
    }

    /// <summary>
    /// 敵を怯え状態にさせる
    /// </summary>
    private void ChangeEnemyFrightenMode()
    {
        foreach(var ghost in _ghostControllerMainList)
        {
            ghost.ChangeFrighterMode();
        }
    }
}
