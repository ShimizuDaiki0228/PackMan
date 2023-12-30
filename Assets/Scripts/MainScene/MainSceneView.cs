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
    /// テキストの透明度のアニメーションを行うシーケンス
    /// </summary>
    private Sequence _instructionTextFadeSequence;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        Sequence sequence = AnimationUtility.TextFadeAnimation(_instructionText,
                                                               InGameConst.TEXT_FADE_DURATION,
                                                               gameObject);
    }
}
