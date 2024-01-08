using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ���[���V�[����p�̃A�j���[�V����Utility
/// ���G�ɂȂ肻���ȋC�����邽��
/// </summary>
public static class RuleSceneAnimationUtility
{
    /// <summary>
    /// �ړ��������
    /// </summary>
    public enum Direction
    {
        DOWN,
        RIGHT,
        LEFT
    }

    /// <summary>
    /// �����̌��̏ꏊ���瓮������
    /// </summary>
    public static readonly Vector2 LeftArrowMovePositionOffset = new Vector2(-50, 0);

    /// <summary>
    /// �E���̌��̏ꏊ���瓮������
    /// </summary>
    public static readonly Vector2 RightArrowMovePositionOffset = new Vector2(50, 0);

    /// <summary>
    /// ���[�������V�[���̃p�l���̍ŏ��̈ʒu
    /// </summary>
    public static readonly Vector2 FirstPanelDisplayPosition = new Vector2(0, 1500);

    /// <summary>
    /// �p�l�������Ɉړ�������
    /// </summary>
    public static readonly Vector2 LeftPanelDisplaySlide = new Vector2(-3000, 0);

    /// <summary>
    /// UI�����ɏ����ړ�������
    /// </summary>
    public static readonly Vector2 UIPositionOffsetDown = new Vector2(0, -50);

    /// <summary>
    /// UI���E�ɏ����ړ�������
    /// </summary>
    public static readonly Vector2 UIPositionOffsetRight = new Vector2(50, 0);

    /// <summary>
    /// UI�����ɏ����ړ�������
    /// </summary>
    public static readonly Vector2 UIPositionOffsetLeft = new Vector2(-50, 0);

    /// <summary>
    /// ���[�������V�[���̃I�u�W�F�N�g�̍ŏ��̈ʒu
    /// </summary>
    public static readonly Vector3 FirstObjectDisplayPosition = new Vector3(0, 9, 0);

    /// <summary>
    /// �I�u�W�F�N�g�����Ɉړ�������
    /// </summary>
    public static readonly Vector3 LeftObjectDisplaySlide = new Vector3(-18, 0, 0);

    /// <summary>
    /// 3D�I�u�W�F�N�g�����ɏ����ړ�������
    /// </summary>
    public static readonly Vector3 ObjectPositionOffsetDown = new Vector3(0, -1, 0);

    /// <summary>
    /// 3D�I�u�W�F�N�g�����ɏ����ړ�������
    /// </summary>
    public static readonly Vector3 ObjectPositionOffsetLeft = new Vector3(-1, 0, 0);

    /// <summary>
    /// 3D�I�u�W�F�N�g���E�ɏ����ړ�������
    /// </summary>
    public static readonly Vector3 ObjectPositionOffsetRight = new Vector3(1, 0, 0);

    /// <summary>
    /// �\���܂łɂ����鎞��
    /// </summary>
    public const float DISPLAY_DURATION = 0.5f;

    /// <summary>
    /// ���̉摜���A�j���[�V����������V�[�P���X
    /// </summary>
    /// <param name="transform">�摜��rectTransform</param>
    /// <param name="canvasGroup">�摜��canvasGroup</param>
    /// <param name="positionOffset">�ǂꂾ���ړ������邩</param>
    /// <param name="linkObject">�����N������I�u�W�F�N�g</param>
    /// <returns></returns>
    public static Sequence ArrowImageAnimation(RectTransform transform,
                                               CanvasGroup canvasGroup,
                                               Vector2 positionOffset,
                                               GameObject linkObject)
    {

        canvasGroup.alpha = 0f;

        Sequence sequence = DOTween.Sequence();

        Tween moveTween = transform.DOLocalMove(transform.anchoredPosition + positionOffset, 1f);

        Tween fadeTween = canvasGroup.DOFade(1f, 1f);

        sequence
            .Append(moveTween)
            .Join(fadeTween)
            .AppendCallback(() => canvasGroup.alpha = 0)
            .SetLoops(-1, LoopType.Restart)
            .SetLink(linkObject);

        return sequence;
    }

    public static Tween InstructionTextAnimation(TextMeshProUGUI text,
                                                    Vector2 positionOffset)
    {
        Tween tween = text.rectTransform.DOLocalMove(text.rectTransform.anchoredPosition + positionOffset, 1f)
            .SetEase(Ease.OutCirc)
            .SetLoops(-1, LoopType.Yoyo);

        return tween;
    }

    /// <summary>
    /// UI�X���C�h�A�j���[�V����
    /// ��������\���ʒu�𒴂��Ė߂��Ă��銴��
    /// </summary>
    /// <param name="rectTransform">�A�j���[�V����������Transform</param>
    /// <param name="destinationPosition">�ŏI�\���ʒu</param>
    /// <param name="destinationPositionOffset">�ŏI�\���ʒu�̏�����</param>
    /// <param name="duration">�����鎞��</param>
    /// <returns></returns>
    private static Sequence SlideUIAnimation(RectTransform rectTransform,
                                            Vector2 destinationPosition,
                                            Vector2 destinationPositionOffset)
    {
        Sequence sequence = DOTween.Sequence();

        sequence
            .Append(rectTransform.DOLocalMove(destinationPositionOffset, DISPLAY_DURATION).SetEase(Ease.OutCirc))
            .Append(rectTransform.DOLocalMove(destinationPosition, 0.4f).SetEase(Ease.OutCirc));

        return sequence;
    }

    /// <summary>
    /// UI���ǂ̕����Ɉړ������邩�����肷��
    /// </summary>
    /// <param name="rectTransform">�A�j���[�V����������UI��RectTransform</param>
    /// <param name="destinationPosition">�ŏI�\���ʒu</param>
    /// <param name="direction">����</param>
    public static void SlideUIAnimationDirection(RectTransform rectTransform,
                                                  Vector2 destinationPosition,
                                                  Direction direction)
    {
        switch(direction)
        {
            case Direction.DOWN:
                SlideUIAnimation(rectTransform,
                                 destinationPosition,
                                 destinationPosition + UIPositionOffsetDown);
                break;

            case Direction.RIGHT:
                SlideUIAnimation(rectTransform,
                                 destinationPosition,
                                 destinationPosition + UIPositionOffsetRight);
                break;

            case Direction.LEFT:
                SlideUIAnimation(rectTransform,
                                 destinationPosition,
                                 destinationPosition + UIPositionOffsetLeft);
                break;
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g�X���C�h�A�j���[�V����
    /// </summary>
    /// <param name="rectTransform"></param>
    /// <param name="destinationPosition"></param>
    /// <param name="destinationPositionOffset"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private static Sequence SlideObjectAnimation(Transform transform,
                                                 Vector3 destinationPosition,
                                                 Vector3 destinationPositionOffset)
    {
        Sequence sequence = DOTween.Sequence();

        sequence
            .Append(transform.DOLocalMove(destinationPositionOffset, DISPLAY_DURATION).SetEase(Ease.OutCirc))
            .Append(transform.DOLocalMove(destinationPosition, 0.4f).SetEase(Ease.OutCirc));

        return sequence;
    }

    /// <summary>
    /// �I�u�W�F�N�g���ǂ̕����Ɉړ������邩���߂�
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="destinationPosition"></param>
    /// <param name="direction"></param>
    public static void SlideObjectAnimationDirection(Transform transform,
                                                     Vector3 destinationPosition,
                                                     Direction direction)
    {
        switch(direction)
        {
            case Direction.DOWN:
                SlideObjectAnimation(transform,
                                     destinationPosition,
                                     destinationPosition + ObjectPositionOffsetDown);
                break;

            case Direction.RIGHT:
                SlideObjectAnimation(transform,
                                     destinationPosition,
                                     destinationPosition + ObjectPositionOffsetRight);
                break;

            case Direction.LEFT:
                SlideObjectAnimation(transform,
                                     destinationPosition,
                                     destinationPosition + ObjectPositionOffsetLeft);
                break;
        }
    }
}
