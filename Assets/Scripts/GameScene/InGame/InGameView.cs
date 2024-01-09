using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEditor;
using TMPro;
using System;
using DG.Tweening;

public class InGameView : MonoBehaviour
{
    /// <summary>
    /// �p�b�N�}���̃��C�t���p�b�N�}���ŕ\�����邽�߂̃R���e�i
    /// �����Ƀ��C�t���̃p�b�N�}���𐶐�����
    /// </summary>
    [SerializeField]
    private Transform _packManLifeContainer;

    /// <summary>
    /// �p�b�N�}���̃��C�t��\�����邽�߂�Prefab
    /// </summary>
    [SerializeField]
    private GameObject _packManLifePrefab;

    /// <summary>
    /// �������ɕ\�������e�L�X�g
    /// </summary>
    [SerializeField]
    private CanvasGroup _readyText;

    /// <summary>
    /// �Q�[�����J�n���Ă��������ǂ����̃v���p�e�B
    /// </summary>
    private readonly ReactiveProperty<bool> _canStartProp = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> CanStartProp => _canStartProp;

    /// <summary>
    /// ���[�f�B���OCanvasGroup
    /// </summary>
    [SerializeField]
    private CanvasGroup _loadingCanvas;

    /// <summary>
    /// ���[�f�B���O���̃e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _loadingText;

    /// <summary>
    /// ���[�f�B���O���I���ăQ�[����ʂɈڂ��Ă��邩�ǂ���
    /// </summary>
    private bool _isReady;

    /// <summary>
    /// ������
    /// </summary>
    public async UniTask Initialize()
    {
        await LoadDisplay();

        _isReady = true;
        ResetView();

        SetEvent();
    }

    /// <summary>
    /// �Q�[�����n�܂�O�Ƀ��[�h��ʂ�\������
    /// </summary>
    private async UniTask LoadDisplay()
    {
        _loadingCanvas.gameObject.SetActive(true);

        StartTextAnimation().Forget();

        await UniTask.WaitForSeconds(3f);

        _loadingText.gameObject.SetActive(false);

        await _loadingCanvas.DOFade(0, 1.0f).ToUniTask();
    }

    /// <summary>
    /// "Now Loading..."��"..."�̐��𓮓I�ɕύX����
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid StartTextAnimation()
    {
        int dotsCount = 0;
        while (!_isReady) // �������[�v
        {
            _loadingText.text = "Now Loading";
            _loadingText.text += new string('.', dotsCount);
            await UniTask.Delay(TimeSpan.FromSeconds(0.3));


            dotsCount = (dotsCount + 1) % 4;
        }
    }

    /// <summary>
    /// �C�x���g�ݒ�
    /// </summary>
    private void SetEvent()
    {
        GameManager.Instance.OnLoseLifeAsObservable
            .Subscribe(_ =>
                LoseLife()
            ).AddTo(this);
    }

    public async void ResetView()
    {
        for(int i = 0; i < GameManager.Instance.Life - 1; i++)
        {
            var packMan = Instantiate(_packManLifePrefab,
                          _packManLifeContainer.position + new Vector3(2, 0, 0) * i,
                          Quaternion.Euler(0, 90, 0),
                          _packManLifeContainer);

            packMan.GetComponent<SphereCollider>().enabled = false;

            Animator animator = packMan.GetComponent<Animator>();
            animator.speed = 0;
        }

        _canStartProp.Value = false;
        UIExtension.TextVisibleSetting(_readyText, 1.0f);

        await UniTask.WaitForSeconds(2.0f);

        _canStartProp.Value = true;
        UIExtension.TextVisibleSetting(_readyText, 0);
    }

    public void LoseLife()
    {
        if(_packManLifeContainer.childCount > 0)
        {
            Transform lastChild = _packManLifeContainer.GetChild(_packManLifeContainer.childCount - 1);

            Destroy(lastChild.gameObject);
        }
    }
}
