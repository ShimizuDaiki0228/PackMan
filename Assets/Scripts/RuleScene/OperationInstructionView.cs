using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class OperationInstructionView : MonoBehaviour
{

    private readonly Vector3 _firstCanvasPosition = new Vector3(3000, 0, 0);

    /// <summary>
    /// �����������
    /// </summary>
    private readonly Vector3 UpDirection = new Vector3(-90, 0, 0);

    /// <summary>
    /// ������������
    /// </summary>
    private readonly Vector3 DownDirection = new Vector3(90, 90, -90);

    /// <summary>
    /// �E����������
    /// </summary>
    private readonly Vector3 RightDirection = new Vector3(0, 90, -90);

    /// <summary>
    /// ������������
    /// </summary>
    private readonly Vector3 LeftDirection = new Vector3(0, -90, 90);

    /// <summary>
    /// ������Ɉړ�����
    /// </summary>
    private readonly Vector3 UpPositionOffset = new Vector3(0, 3, 0);

    /// <summary>
    /// �������Ɉړ�����
    /// </summary>
    private readonly Vector3 DownPositionOffset = new Vector3(0, -3, 0);

    /// <summary>
    /// �E�����Ɉړ�����
    /// </summary>
    private readonly Vector3 RightPositionOffset = new Vector3(3, 0, 0);

    /// <summary>
    /// �������Ɉړ�����
    /// </summary>
    private readonly Vector3 LeftPositionOffset = new Vector3(-3, 0, 0);

    /// <summary>
    /// �p�b�N�}��
    /// </summary>
    [SerializeField]
    private RuleScenePackManController _packMan;

    /// <summary>
    /// ��������Ɋւ���canvas
    /// </summary>
    [SerializeField]
    private RectTransform _canvas;

    /// <summary>
    /// ����Canvas�ŉ���\�����Ă��邩
    /// </summary>
    public DisplayCanvasType CanvasType;

    /// <summary>
    /// ������
    /// </summary>
    public void Initialize()
    {
        _packMan.Reset();
        Reset();
    }

    /// <summary>
    /// �蓮Update
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
    /// 3D�I�u�W�F�N�g��UI���A�j���[�V����������
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
