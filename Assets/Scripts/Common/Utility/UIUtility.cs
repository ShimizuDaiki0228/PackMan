using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class UIUtility
{
    /// <summary>
    /// �e�L�X�g�̓����x��ύX����
    /// </summary>
    /// <param name="text">�e�L�X�g</param>
    /// <param name="alpha">�����x</param>
    public static void ChangeTextTransparent(CanvasGroup text, float alpha)
    {
        text.alpha = alpha;
    }
}
