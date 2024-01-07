using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    /// RuleExplanationView
    /// </summary>
    [SerializeField]
    private RuleExplanationView _ruleExplanationView;

    /// <summary>
    /// 背景を変更中かどうか
    /// </summary>
    private bool _isChanged;

    /// <summary>
    /// 右方向の矢印画像
    /// </summary>
    [SerializeField]
    private Image _rightArrowImage;

    /// <summary>
    /// 右方向の矢印のCanvasGroup
    /// </summary>
    [SerializeField]
    private CanvasGroup _rightArrowCanvasGroup;

    /// <summary>
    /// 左方向の矢印画像
    /// </summary>
    [SerializeField]
    private Image _leftArrowImage;

    /// <summary>
    /// 左方向の矢印のCanvasGroup
    /// </summary>
    [SerializeField]
    private CanvasGroup _leftArrowCanvasGroup;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        _characterExplanationView.Initialize();
        _operationInstructionView.Initialize();
        _ruleExplanationView.Initialize();

        _rightArrowCanvasGroup.alpha = 0;
        _leftArrowCanvasGroup.alpha = 0;

        ArrowActiveChange(true, false);

        RuleSceneAnimationUtility.ArrowImageAnimation(_rightArrowImage.rectTransform,
                                                      _rightArrowCanvasGroup,
                                                      RuleSceneAnimationUtility.RightArrowMovePositionOffset,
                                                      gameObject);

        RuleSceneAnimationUtility.ArrowImageAnimation(_leftArrowImage.rectTransform,
                                                      _leftArrowCanvasGroup,                                  
                                                      RuleSceneAnimationUtility.LeftArrowMovePositionOffset,
                                                      gameObject);
    }

    /// <summary>
    /// 手動Update
    /// </summary>
    public void ManualUpdate()
    {
        _characterExplanationView.ManualUpdate();
        _operationInstructionView.ManualUpdate();

        Debug.Log(_operationInstructionView.CanvasType);

        CanvasSlide().Forget();
    }

    /// <summary>
    /// 背景をスライドさせる
    /// </summary>
    public async UniTask CanvasSlide()
    {
        if(Input.GetKeyDown(KeyCode.RightShift)
            && _operationInstructionView.CanvasType != DisplayCanvasType.RULE
            && !_isChanged)
        {
            _isChanged = true;

            ArrowActiveChange(false, false);

            _characterExplanationView.SlideAnimation(RuleSceneAnimationUtility.Direction.LEFT,
                                                     RuleSceneAnimationUtility.LeftObjectDisplaySlide,
                                                     RuleSceneAnimationUtility.LeftPanelDisplaySlide);

            _operationInstructionView.SlideAnimation(RuleSceneAnimationUtility.Direction.LEFT,
                                                     RuleSceneAnimationUtility.LeftObjectDisplaySlide,
                                                     RuleSceneAnimationUtility.LeftPanelDisplaySlide);

            _ruleExplanationView.SlideAnimation(RuleSceneAnimationUtility.Direction.LEFT,
                                                     RuleSceneAnimationUtility.LeftObjectDisplaySlide,
                                                     RuleSceneAnimationUtility.LeftPanelDisplaySlide);

            await UniTask.WaitForSeconds(1);

            _operationInstructionView.CanvasType 
                = ChangeCanvasType(_operationInstructionView.CanvasType, 1);

            ArrowActiveChange(_operationInstructionView.CanvasType != DisplayCanvasType.RULE, true);

            _isChanged = false;
        }

        else if(Input.GetKeyDown(KeyCode.LeftShift)
            && _operationInstructionView.CanvasType != DisplayCanvasType.CHARACTER
            && !_isChanged)
        {
            _isChanged = true;

            ArrowActiveChange(false, false);

            _characterExplanationView.SlideAnimation(RuleSceneAnimationUtility.Direction.RIGHT,
                                                     -RuleSceneAnimationUtility.LeftObjectDisplaySlide,
                                                     -RuleSceneAnimationUtility.LeftPanelDisplaySlide);

            _operationInstructionView.SlideAnimation(RuleSceneAnimationUtility.Direction.LEFT,
                                                     -RuleSceneAnimationUtility.LeftObjectDisplaySlide,
                                                     -RuleSceneAnimationUtility.LeftPanelDisplaySlide);

            _ruleExplanationView.SlideAnimation(RuleSceneAnimationUtility.Direction.LEFT,
                                                     -RuleSceneAnimationUtility.LeftObjectDisplaySlide,
                                                     -RuleSceneAnimationUtility.LeftPanelDisplaySlide);

            await UniTask.WaitForSeconds(1);

            _operationInstructionView.CanvasType
                = ChangeCanvasType(_operationInstructionView.CanvasType, -1);

            ArrowActiveChange(true, _operationInstructionView.CanvasType != DisplayCanvasType.CHARACTER);

            _isChanged = false;
        }

    }

    /// <summary>
    /// DisplayCanvasTypeを変更する
    /// </summary>
    /// <param name="type">現在のタイプ</param>
    /// <param name="value">値をどれだけ変更させるか</param>
    /// <returns></returns>
    private DisplayCanvasType ChangeCanvasType(DisplayCanvasType type, int value)
    {
        int nextValue = (int)type + value;
        return (DisplayCanvasType)nextValue;
    }

    /// <summary>
    /// 右矢印画像と左矢印画像のアクティブを変更する
    /// </summary>
    /// <param name="rightActive"></param>
    /// <param name="leftActive"></param>
    private void ArrowActiveChange(bool rightActive, bool leftActive)
    {
        _rightArrowImage.gameObject.SetActive(rightActive);
        _leftArrowImage.gameObject.SetActive(leftActive);
    }
}
