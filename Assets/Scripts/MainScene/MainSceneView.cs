using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainSceneView : MonoBehaviour
{
    /// <summary>
    /// �X�y�[�X�L�[���N���b�N����悤�ɗU������e�L�X�g
    /// </summary>
    [SerializeField]
    private CanvasGroup _instructionText;

    /// <summary>
    /// �e�L�X�g�̓����x���ς�鎞��
    /// </summary>
    private const float TEXT_FADE_DURATION = 1.0f;

    /// <summary>
    /// �e�L�X�g�̓����x�̃A�j���[�V�������s���V�[�P���X
    /// </summary>
    private Sequence _instructionTextFadeSequence;

    /// <summary>
    /// ������
    /// </summary>
    public void Initialize()
    {
        _instructionTextFadeSequence = DOTween.Sequence();

        _instructionTextFadeSequence.Append(_instructionText.DOFade(0, TEXT_FADE_DURATION)).SetEase(Ease.InOutQuad);
        _instructionTextFadeSequence.Append(_instructionText.DOFade(1, TEXT_FADE_DURATION)).SetEase(Ease.InOutQuad);

        _instructionTextFadeSequence.SetLoops(-1).SetLink(gameObject);
    }
}
