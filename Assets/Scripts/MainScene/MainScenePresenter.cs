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

        _view.PlayButton.OnClickAsObservable()
            .Subscribe(_ =>
                ChangeScene()
            ).AddTo(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            UniTask.WaitForSeconds(1);

            ChangeScene();
        }

        if (_packManMain.IsEnemyHit)
            return;

        _packManMain.ManualUpdate();

        foreach(var ghost in _ghostControllerMainList)
        {
            ghost.ManualUpdate();
        }
    }

    /// <summary>
    /// �Q�[���V�[���ɑJ�ڂ���
    /// </summary>
    private void ChangeScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings; // ���[�v���邽�߂Ƀ��W�������Z�q���g�p

        SceneManager.LoadScene(nextSceneIndex);

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
