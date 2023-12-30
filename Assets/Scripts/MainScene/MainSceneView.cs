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
    /// �e�L�X�g�̓����x�̃A�j���[�V�������s���V�[�P���X
    /// </summary>
    private Sequence _instructionTextFadeSequence;

    /// <summary>
    /// ������
    /// </summary>
    public void Initialize()
    {
        Sequence sequence = AnimationUtility.TextFadeAnimation(_instructionText,
                                                               InGameConst.TEXT_FADE_DURATION,
                                                               gameObject);
    }
}
