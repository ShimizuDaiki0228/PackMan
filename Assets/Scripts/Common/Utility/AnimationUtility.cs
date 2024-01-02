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
    /// 通常時のボタンのサイズ
    /// </summary>
    private static readonly float NormalButtonMaxSize = 1f;

    /// <summary>
    /// マウスポインタが上にある時のボタンのサイズ
    /// </summary>
    private static readonly float NormalButtonMinSize = 0.8f;

    /// <summary>
    /// マウスポインタによってボタンのサイズが変更するときにかかる時間
    /// </summary>
    private static readonly float ButtonScaleChangeTime = 0.3f;

    /// <summary>
    /// テキストを一文字ずつ表示する
    /// </summary>
    /// <param name="text">テキスト</param>
    /// <param name="duration">表示にかける時間</param>
    /// <param name="isRichText">リッチテキストの利用を許可するかどうか</param>
    /// <param name="scrambleMode">文字が表示するまでにランダムで表示する文字のタイプ</param>
    /// <returns></returns>
    public static Tween TextDisplayInOrder(TextMeshProUGUI text, float duration, bool isRichText, ScrambleMode scrambleMode)
    {
        return text.DOText(text.text.ToString(), duration, false, ScrambleMode.Numerals).SetEase(Ease.Linear);
    }

    /// <summary>
    /// テキストをスライドで表示するアニメーション
    /// </summary>
    /// <param name="destinationPosition">最終表示位置</param>
    /// <param name="duration">アニメーション時間</param>
    /// <returns></returns>
    public static Tween TextSlideAnimation(TextMeshProUGUI text, Vector3 destinationPosition, float duration)
    {
        return text.gameObject.transform.DOMove(destinationPosition, duration);
    }

    /// <summary>
    /// テキストを回転させるアニメーション
    /// </summary>
    /// <param name="rotateVector3">どこまで回転させるか</param>
    /// <param name="duration">どれだけ時間をかけて回転させるか</param>
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
    /// テキストをフェードさせるアニメーション
    /// </summary>
    /// <param name="canvasGroup">テキストのCanvasGroup</param>
    /// <param name="duration">フェードさせる時間</param>
    /// <param name="linkObject">ループを解除するためのリンクさせるオブジェクト、このオブジェクトが破壊されるとループを抜ける</param>
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
    /// テキストを跳ねらせるアニメーションシーケンス
    /// </summary>
    /// <param name="firstTextSize">最初のテキストのサイズ</param>
    /// <param name="bounceOffset">どれだけ跳ねらせるか</param>
    /// <param name="delayTime">次の文字のアニメーションを行うまでにかかる時間</param>
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
    /// テキストの色を一時的に違う色に変更するシーケンス
    /// 文字を光らせるときなどに使用
    /// </summary>
    /// <param name="color">変更したい色</param>
    /// <param name="delayTime">次の文字のアニメーションを行うまでにかかる時間</param>
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
    /// ボタンのアウトラインの色を変更する
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
    /// ボタンの上にマウスポイントが来た時のTween
    /// </summary>
    public static void OnPointerEnterButtonTween(Transform buttonTransform)
    {
        float tweenTime =
            ButtonScaleChangeTime * (buttonTransform.localScale.x / NormalButtonMaxSize);

        buttonTransform.DOScale(NormalButtonMinSize, tweenTime).SetEase(Ease.OutQuint);
    }

    /// <summary>
    /// ボタンの上からマウスポイントが外れた時のTween
    /// </summary>
    public static void OnPointerExitButtonTween(Transform buttonTransform)
    {
        float tweenTime =
            ButtonScaleChangeTime * (NormalButtonMinSize / buttonTransform.localScale.x);

        buttonTransform.DOScale(NormalButtonMaxSize, tweenTime).SetEase(Ease.OutQuint);
    }

    /// <summary>
    /// テキストを定期的に回転させるアニメーションシーケンス
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
