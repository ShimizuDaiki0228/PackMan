using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class OperationInstructionView : MonoBehaviour
{

    private readonly Vector3 _firstCanvasPosition = new Vector3(3000, 0, 0);

    /// <summary>
    /// 上方向を向く
    /// </summary>
    private readonly Vector3 UpDirection = new Vector3(-90, 0, 0);

    /// <summary>
    /// 下方向を向く
    /// </summary>
    private readonly Vector3 DownDirection = new Vector3(90, 90, -90);

    /// <summary>
    /// 右方向を向く
    /// </summary>
    private readonly Vector3 RightDirection = new Vector3(0, 90, -90);

    /// <summary>
    /// 左方向を向く
    /// </summary>
    private readonly Vector3 LeftDirection = new Vector3(0, -90, 90);

    /// <summary>
    /// 上方向に移動する
    /// </summary>
    private readonly Vector3 UpPositionOffset = new Vector3(0, 3, 0);

    /// <summary>
    /// 下方向に移動する
    /// </summary>
    private readonly Vector3 DownPositionOffset = new Vector3(0, -3, 0);

    /// <summary>
    /// 右方向に移動する
    /// </summary>
    private readonly Vector3 RightPositionOffset = new Vector3(3, 0, 0);

    /// <summary>
    /// 左方向に移動する
    /// </summary>
    private readonly Vector3 LeftPositionOffset = new Vector3(-3, 0, 0);

    /// <summary>
    /// パックマン
    /// </summary>
    [SerializeField]
    private RuleScenePackManController _packMan;

    /// <summary>
    /// 操作説明に関するcanvas
    /// </summary>
    [SerializeField]
    private RectTransform _canvas;

    /// <summary>
    /// 現在Canvasで何を表示しているか
    /// </summary>
    public DisplayCanvasType CanvasType;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        _packMan.Reset();
        Reset();
    }

    /// <summary>
    /// 手動Update
    /// </summary>
    public void ManualUpdate()
    {
        if(CanvasType == DisplayCanvasType.OPERATION)
        {
            _packMan.ManualUpdate();
            ChangePackmanMoveDirection();
        }
    }

    private void ChangePackmanMoveDirection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            _packMan.ChangeMoveDirection(UpDirection, UpPositionOffset);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            _packMan.ChangeMoveDirection(DownDirection, DownPositionOffset);
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            _packMan.ChangeMoveDirection(RightDirection, RightPositionOffset);
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            _packMan.ChangeMoveDirection(LeftDirection, LeftPositionOffset);
    }

    /// <summary>
    /// 3DオブジェクトとUIをアニメーションさせる
    /// </summary>
    public void SlideAnimation(RuleSceneAnimationUtility.Direction direction,
                               Vector3 objectDestinationOffset,
                               Vector2 UIDestinationOffset)
    {
        RuleSceneAnimationUtility.SlideObjectAnimationDirection(_packMan.transform,
                                                                _packMan.transform.position + objectDestinationOffset,
                                                                direction);

        RuleSceneAnimationUtility.SlideUIAnimationDirection(_canvas,
                                                            _canvas.anchoredPosition + UIDestinationOffset,
                                                            direction);
    }

    private void Reset()
    {
        _canvas.anchoredPosition = _firstCanvasPosition;
    }
}
