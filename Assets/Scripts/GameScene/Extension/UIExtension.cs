using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIExtension
{
    /// <summary>
    /// �e�L�X�g�̓����x��ݒ肷��
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <param name="alpha"></param>
    public static void TextVisibleSetting(CanvasGroup canvasGroup, float alpha)
    {
        canvasGroup.alpha = alpha;
    }
}
