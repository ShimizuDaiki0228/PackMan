using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ルールシーン専用のアニメーションUtility
/// 複雑になりそうな気がするため
/// </summary>
public static class RuleSceneAnimationUtility
{
    /// <summary>
    /// 移動する方向
    /// </summary>
    public enum Direction
    {
        DOWN,
        RIGHT,
        LEFT
    }

    /// <summary>
    /// 左矢印の元の場所から動く距離
    /// </summary>
    public static readonly Vector2 LeftArrowMovePositionOffset = new Vector2(-50, 0);

    /// <summary>
    /// 右矢印の元の場所から動く距離
    /// </summary>
    public static readonly Vector2 RightArrowMovePositionOffset = new Vector2(50, 0);

    /// <summary>
    /// ルール説明シーンのパネルの最初の位置
    /// </summary>
    public static readonly Vector2 FirstPanelDisplayPosition = new Vector2(0, 1500);

    /// <summary>
    /// パネルを左に移動させる
    /// </summary>
    public static readonly Vector2 LeftPanelDisplaySlide = new Vector2(-3000, 0);

    /// <summary>
    /// UIを下に少し移動させる
    /// </summary>
    public static readonly Vector2 UIPositionOffsetDown = new Vector2(0, -50);

    /// <summary>
    /// UIを右に少し移動させる
    /// </summary>
    public static readonly Vector2 UIPositionOffsetRight = new Vector2(50, 0);

    /// <summary>
    /// UIを左に少し移動させる
    /// </summary>
    public static readonly Vector2 UIPositionOffsetLeft = new Vector2(-50, 0);

    /// <summary>
    /// ルール説明シーンのオブジェクトの最初の位置
    /// </summary>
    public static readonly Vector3 FirstObjectDisplayPosition = new Vector3(0, 9, 0);

    /// <summary>
    /// オブジェクトを左に移動させる
    /// </summary>
    public static readonly Vector3 LeftObjectDisplaySlide = new Vector3(-18, 0, 0);

    /// <summary>
    /// 3Dオブジェクトを下に少し移動させる
    /// </summary>
    public static readonly Vector3 ObjectPositionOffsetDown = new Vector3(0, -1, 0);

    /// <summary>
    /// 3Dオブジェクトを左に少し移動させる
    /// </summary>
    public static readonly Vector3 ObjectPositionOffsetLeft = new Vector3(-1, 0, 0);

    /// <summary>
    /// 3Dオブジェクトを右に少し移動させる
    /// </summary>
    public static readonly Vector3 ObjectPositionOffsetRight = new Vector3(1, 0, 0);

    /// <summary>
    /// 表示までにかける時間
    /// </summary>
    public const float DISPLAY_DURATION = 0.5f;

    /// <summary>
    /// 矢印の画像をアニメーションさせるシーケンス
    /// </summary>
    /// <param name="transform">画像のrectTransform</param>
    /// <param name="canvasGroup">画像のcanvasGroup</param>
    /// <param name="positionOffset">どれだけ移動させるか</param>
    /// <param name="linkObject">リンクさせるオブジェクト</param>
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
    /// UIスライドアニメーション
    /// いったん表示位置を超えて戻ってくる感じ
    /// </summary>
    /// <param name="rectTransform">アニメーションさせるTransform</param>
    /// <param name="destinationPosition">最終表示位置</param>
    /// <param name="destinationPositionOffset">最終表示位置の少し奥</param>
    /// <param name="duration">かける時間</param>
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
    /// UIをどの方向に移動させるかを決定する
    /// </summary>
    /// <param name="rectTransform">アニメーションさせるUIのRectTransform</param>
    /// <param name="destinationPosition">最終表示位置</param>
    /// <param name="direction">方向</param>
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
    /// オブジェクトスライドアニメーション
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
    /// オブジェクトをどの方向に移動させるか決める
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
