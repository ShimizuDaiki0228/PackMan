using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEditor;

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
    /// ������
    /// </summary>
    public void Initialize()
    {
        ResetView();

        SetEvent();
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
