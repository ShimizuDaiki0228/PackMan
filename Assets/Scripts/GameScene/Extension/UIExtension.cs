using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIExtension
{
    /// <summary>
    /// テキストの透明度を設定する
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <param name="alpha"></param>
    public static void TextVisibleSetting(CanvasGroup canvasGroup, float alpha)
    {
        canvasGroup.alpha = alpha;
    }
}
