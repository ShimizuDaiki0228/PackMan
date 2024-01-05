using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DisplayCanvasType
{
    CHARACTER,
    OPERATION,
    RULE
}

public class RuleSceneView : MonoBehaviour
{
    /// <summary>
    /// CharacterExplanationView
    /// </summary>
    [SerializeField]
    private CharacterExplanationView _characterExplanationView;

    /// <summary>
    /// OperationInstructionView
    /// </summary>
    [SerializeField]
    private OperationInstructionView _operationInstructionView;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        _characterExplanationView.Initialize();
        _operationInstructionView.Initialize();
    }

    /// <summary>
    /// 手動Update
    /// </summary>
    public void ManualUpdate()
    {
        _characterExplanationView.ManualUpdate();
        _operationInstructionView.ManualUpdate();
    }

    /// <summary>
    /// 背景をスライドさせる
    /// </summary>
    public async UniTask CanvasSlide()
    {
        _characterExplanationView.SlideAnimation(RuleSceneAnimationUtility.Direction.LEFT,
                                                 RuleSceneAnimationUtility.LeftObjectDisplaySlide,
                                                 RuleSceneAnimationUtility.LeftPanelDisplaySlide);

        _operationInstructionView.SlideAnimation(RuleSceneAnimationUtility.Direction.LEFT,
                                                 RuleSceneAnimationUtility.LeftObjectDisplaySlide,
                                                 RuleSceneAnimationUtility.LeftPanelDisplaySlide);

        await UniTask.WaitForSeconds(1);

        _operationInstructionView.CanvasType = DisplayCanvasType.OPERATION;
    }
}
