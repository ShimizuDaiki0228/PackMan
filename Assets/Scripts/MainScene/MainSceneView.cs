using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainSceneView : MonoBehaviour
{
    /// <summary>
    /// スペースキーをクリックするように誘導するテキスト
    /// </summary>
    [SerializeField]
    private CanvasGroup _instructionText;

    /// <summary>
    /// テキストの透明度が変わる時間
    /// </summary>
    private const float TEXT_FADE_DURATION = 1.0f;

    /// <summary>
    /// テキストの透明度のアニメーションを行うシーケンス
    /// </summary>
    private Sequence _instructionTextFadeSequence;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        _instructionTextFadeSequence = DOTween.Sequence();

        _instructionTextFadeSequence.Append(_instructionText.DOFade(0, TEXT_FADE_DURATION)).SetEase(Ease.InOutQuad);
        _instructionTextFadeSequence.Append(_instructionText.DOFade(1, TEXT_FADE_DURATION)).SetEase(Ease.InOutQuad);

        _instructionTextFadeSequence.SetLoops(-1).SetLink(gameObject);
    }
}
