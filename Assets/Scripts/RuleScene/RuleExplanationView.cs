using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class RuleExplanationView : MonoBehaviour
{
    /// <summary>
    /// 最初のCanvasの位置
    /// </summary>
    private readonly Vector3 _firstCanvasPosition = new Vector3(6000, 0, 0);

    /// <summary>
    /// ルール説明に関するCanvas
    /// </summary>
    [SerializeField]
    private RectTransform _canvas;

    public void Initialize()
    {
        Reset();
    }

    private void Reset()
    {
        _canvas.anchoredPosition = _firstCanvasPosition;
    }

    /// <summary>
    /// UIをアニメーションさせる
    /// </summary>
    public void SlideAnimation(RuleSceneAnimationUtility.Direction direction,
                               Vector3 objectDestinationOffset,
                               Vector2 UIDestinationOffset)
    {
        RuleSceneAnimationUtility.SlideUIAnimationDirection(_canvas,
                                                            _canvas.anchoredPosition + UIDestinationOffset,
                                                            direction);
    }
}
