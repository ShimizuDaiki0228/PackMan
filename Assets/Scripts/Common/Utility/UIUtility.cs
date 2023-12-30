using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class UIUtility
{
    /// <summary>
    /// テキストの透明度を変更する
    /// </summary>
    /// <param name="text">テキスト</param>
    /// <param name="alpha">透明度</param>
    public static void ChangeTextTransparent(CanvasGroup text, float alpha)
    {
        text.alpha = alpha;
    }
}
