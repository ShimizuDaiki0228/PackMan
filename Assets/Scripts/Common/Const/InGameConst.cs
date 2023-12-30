using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InGameConst
{
    public static readonly Vector3 Up = Vector3.zero;
    public static readonly Vector3 Right = new Vector3(0, 90, 0);
    public static readonly Vector3 Down = new Vector3(0, 180, 0);
    public static readonly Vector3 Left = new Vector3(0, 270, 0);

    public static readonly Vector3 GameOverSceneTextOffset
        = new Vector3(0, 300, 0);

    /// <summary>
    /// テキストの透明度が変わる時間
    /// </summary>
    public const float TEXT_FADE_DURATION = 1.0f;
}
