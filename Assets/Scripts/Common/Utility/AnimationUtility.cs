using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;

public static class AnimationUtility
{
    /// <summary>
    /// �ʏ펞�̃{�^���̃T�C�Y
    /// </summary>
    private static readonly float NormalButtonMaxSize = 1f;

    /// <summary>
    /// �}�E�X�|�C���^����ɂ��鎞�̃{�^���̃T�C�Y
    /// </summary>
    private static readonly float NormalButtonMinSize = 0.8f;

    /// <summary>
    /// �}�E�X�|�C���^�ɂ���ă{�^���̃T�C�Y���ύX����Ƃ��ɂ����鎞��
    /// </summary>
    private static readonly float ButtonScaleChangeTime = 0.3f;

    /// <summary>
    /// �e�L�X�g���ꕶ�����\������
    /// </summary>
    /// <param name="text">�e�L�X�g</param>
    /// <param name="duration">�\���ɂ����鎞��</param>
    /// <param name="isRichText">���b�`�e�L�X�g�̗��p�������邩�ǂ���</param>
    /// <param name="scrambleMode">�������\������܂łɃ����_���ŕ\�����镶���̃^�C�v</param>
    /// <returns></returns>
    public static Tween TextDisplayInOrder(TextMeshProUGUI text, float duration, bool isRichText, ScrambleMode scrambleMode)
    {
        return text.DOText(text.text.ToString(), duration, false, ScrambleMode.Numerals).SetEase(Ease.Linear);
    }

    /// <summary>
    /// �e�L�X�g���X���C�h�ŕ\������A�j���[�V����
    /// </summary>
    /// <param name="destinationPosition">�ŏI�\���ʒu</param>
    /// <param name="duration">�A�j���[�V��������</param>
    /// <returns></returns>
    public static Tween TextSlideAnimation(TextMeshProUGUI text, Vector3 destinationPosition, float duration)
    {
        return text.gameObject.transform.DOMove(destinationPosition, duration);
    }

    /// <summary>
    /// �e�L�X�g����]������A�j���[�V����
    /// </summary>
    /// <param name="rotateVector3">�ǂ��܂ŉ�]�����邩</param>
    /// <param name="duration">�ǂꂾ�����Ԃ������ĉ�]�����邩</param>
    /// <returns></returns>
    public static Tween RotateTextAnimation(Text text,
                                            Vector3 rotateVector3,
                                            float duration)
    {
        return text.transform.DOLocalRotate(rotateVector3, duration, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart);
    }




    /// <summary>
    /// �e�L�X�g���t�F�[�h������A�j���[�V����
    /// </summary>
    /// <param name="canvasGroup">�e�L�X�g��CanvasGroup</param>
    /// <param name="duration">�t�F�[�h�����鎞��</param>
    /// <param name="linkObject">���[�v���������邽�߂̃����N������I�u�W�F�N�g�A���̃I�u�W�F�N�g���j�󂳂��ƃ��[�v�𔲂���</param>
    /// <returns></returns>
    public static Sequence TextFadeAnimation(CanvasGroup canvasGroup, float duration, GameObject linkObject)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(canvasGroup.DOFade(0, duration)).SetEase(Ease.InOutQuad);
        sequence.Append(canvasGroup.DOFade(1, duration)).SetEase(Ease.InOutQuad);

        sequence.SetLoops(-1).SetLink(linkObject);

        return sequence;
    }

    /// <summary>
    /// �e�L�X�g�𒵂˂点��A�j���[�V�����V�[�P���X
    /// </summary>
    /// <param name="firstTextSize">�ŏ��̃e�L�X�g�̃T�C�Y</param>
    /// <param name="bounceOffset">�ǂꂾ�����˂点�邩</param>
    /// <param name="delayTime">���̕����̃A�j���[�V�������s���܂łɂ����鎞��</param>
    /// <returns></returns>
    public static Sequence TextBounceAnimationSequence(DOTweenTMPAnimator tmproAnimator,
                                               float firstTextSize,
                                               Vector3 bounceOffset,
                                               float delayTime)
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < tmproAnimator.textInfo.characterCount; i++)
        {
            tmproAnimator.DOScaleChar(i, firstTextSize, 0);
            Vector3 currCharOffset = tmproAnimator.GetCharOffset(i);
            sequence = DOTween.Sequence()
                .Append(tmproAnimator.DOOffsetChar(i, currCharOffset + bounceOffset, 0.6f).SetEase(Ease.OutFlash, 2))
                .Join(tmproAnimator.DOFadeChar(i, 1, 0.4f))
                .Join(tmproAnimator.DOScaleChar(i, 1, 0.4f).SetEase(Ease.OutBack))
                .SetDelay(delayTime * i);
        }

        return sequence;
    }

    /// <summary>
    /// �e�L�X�g�̐F���ꎞ�I�ɈႤ�F�ɕύX����V�[�P���X
    /// ���������点��Ƃ��ȂǂɎg�p
    /// </summary>
    /// <param name="color">�ύX�������F</param>
    /// <param name="delayTime">���̕����̃A�j���[�V�������s���܂łɂ����鎞��</param>
    /// <returns></returns>
    public static Sequence TextChangeColorSequence(DOTweenTMPAnimator tmproAnimator,
                                                   Color color,
                                                   float delayTime)
    {
        Sequence sequence = DOTween.Sequence();

        for (int i = 0; i < tmproAnimator.textInfo.characterCount; ++i)
        {
            sequence.Append(tmproAnimator.DOColorChar(i, color, 0.2f).SetLoops(2, LoopType.Yoyo))
                .SetDelay(delayTime * i);
        }

        return sequence;
    }

    /// <summary>
    /// �{�^���̃A�E�g���C���̐F��ύX����
    /// </summary>
    /// <param name="outline"></param>
    /// <returns></returns>
    public static Sequence OutlineColorChangeSequence(Outline outline)
    {
        Sequence sequence = DOTween.Sequence();

        sequence
            .Append(DOTween.To(() => outline.effectColor, x => outline.effectColor = x, Color.red, 1f))
            .Append(DOTween.To(() => outline.effectColor, x => outline.effectColor = x, Color.yellow, 1f))
            .Append(DOTween.To(() => outline.effectColor, x => outline.effectColor = x, Color.green, 1f))
            .Append(DOTween.To(() => outline.effectColor, x => outline.effectColor = x, Color.blue, 1f))
            .Append(DOTween.To(() => outline.effectColor, x => outline.effectColor = x, Color.red, 1f))
            .SetLoops(-1) 
            .Play();

        return sequence;
    }

    /// <summary>
    /// �{�^���̏�Ƀ}�E�X�|�C���g����������Tween
    /// </summary>
    public static void OnPointerEnterButtonTween(Transform buttonTransform)
    {
        float tweenTime =
            ButtonScaleChangeTime * (buttonTransform.localScale.x / NormalButtonMaxSize);

        buttonTransform.DOScale(NormalButtonMinSize, tweenTime).SetEase(Ease.OutQuint);
    }

    /// <summary>
    /// �{�^���̏ォ��}�E�X�|�C���g���O�ꂽ����Tween
    /// </summary>
    public static void OnPointerExitButtonTween(Transform buttonTransform)
    {
        float tweenTime =
            ButtonScaleChangeTime * (NormalButtonMinSize / buttonTransform.localScale.x);

        buttonTransform.DOScale(NormalButtonMaxSize, tweenTime).SetEase(Ease.OutQuint);
    }

    /// <summary>
    /// �e�L�X�g�����I�ɉ�]������A�j���[�V�����V�[�P���X
    /// </summary>
    /// <param name="tmproAnimator"></param>
    /// <param name="duration"></param>
    public static void RotateTextMeshProAnimationSequence(DOTweenTMPAnimator tmproAnimator,
                                                              float duration)
    {
        const float EACH_DELAY_RATIO = 0.01f;
        var eachDelay = duration * EACH_DELAY_RATIO;
        var eachDuration = duration - eachDelay;

        Sequence sequence = DOTween.Sequence();

        for (var i = 0; i < tmproAnimator.textInfo.characterCount; i++)
        {
            DOTween.Sequence()
                .Append(tmproAnimator.DORotateChar(i, Vector3.right * 360, eachDuration / 2, RotateMode.FastBeyond360))
                .AppendInterval(eachDuration / 4)

                .SetDelay((eachDelay / 1) * i);
        }
    }
}
