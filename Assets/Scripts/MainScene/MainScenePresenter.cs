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
    /// ���C���V�[���̃p�b�N�}��
    /// </summary>
    [SerializeField]
    private PackManControllerMain _packManMain;

    /// <summary>
    /// ���C���V�[���̓G
    /// </summary>
    [SerializeField]
    private GhostControllerMain[] _ghostControllerMainList;

    /// <summary>
    /// �a
    /// </summary>
    [SerializeField]
    private GameObject _pellet;

    /// <summary>
    /// �ł����p�b�N�}��
    /// �v���C�{�^�����N���b�N������ɏo�Ă���
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
    /// �Q�[���V�[���ɑJ�ڂ���
    /// </summary>
    private void ChangeScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings; // ���[�v���邽�߂Ƀ��W�������Z�q���g�p

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
    /// �G��������Ԃɂ�����
    /// </summary>
    private void ChangeEnemyFrightenMode()
    {
        foreach(var ghost in _ghostControllerMainList)
        {
            ghost.ChangeFrighterMode();
        }
    }
}
