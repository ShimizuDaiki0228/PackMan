using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Cysharp.Threading.Tasks;

public class MainSceneView : MonoBehaviour
{
    /// <summary>
    /// �X�y�[�X�L�[���N���b�N����悤�ɗU������e�L�X�g
    /// </summary>
    [SerializeField]
    private CanvasGroup _instructionText;

    /// <summary>
    /// �e�L�X�g�̓����x�̃A�j���[�V�������s���V�[�P���X
    /// </summary>
    private Sequence _instructionTextFadeSequence;

    /// <summary>
    /// �v���C���[��I��������e�L�X�g
    /// </summary>
    [SerializeField]
    private Text _playerSelectArrowText;

    /// <summary>
    /// �v���C���[��I������e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _playerSelectText;

    /// <summary>
    /// �v���C���[�I���e�L�X�g�̃A�j���[�^�[
    /// </summary>
    private DOTweenTMPAnimator _playerSelectTextAnimator;

    /// <summary>
    /// ������
    /// </summary>
    public void Initialize()
    {
        Sequence instructionTextSequence = AnimationUtility.TextFadeAnimation(_instructionText,
                                                               InGameConst.TEXT_FADE_DURATION,
                                                               gameObject);

        AnimationUtility.RotateTextAnimation(_playerSelectArrowText,
                                             new Vector3(360f, 0, 0),
                                             3f);

        _playerSelectTextAnimator = new DOTweenTMPAnimator(_playerSelectText);

        TextSetup();
        RotateSelectPlayerText();
    }

    /// <summary>
    /// �v���C���[�I���e�L�X�g�����I�ɉ�]������
    /// </summary>
    private async void RotateSelectPlayerText()
    {
        while (true)
        {
            AnimationUtility.RotateTextMeshProAnimationSequence(_playerSelectTextAnimator,
                                                            2.5f);

            await UniTask.WaitForSeconds(3.5f);
        }
    }


    /// <summary>
    /// �e�L�X�g�̏����ݒ�
    /// </summary>
    private void TextSetup()
    {
        for (var i = 0; i < _playerSelectTextAnimator.textInfo.characterCount; i++)
        {
            _playerSelectTextAnimator.DORotateChar(i, Vector3.right * 90, 0);
            _playerSelectTextAnimator.DOOffsetChar(i, Vector3.zero, 0);
            _playerSelectTextAnimator.DOFadeChar(i, 1, 0);
        }
    }
}
