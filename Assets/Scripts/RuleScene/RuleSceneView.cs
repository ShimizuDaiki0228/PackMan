using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UniRx.Triggers;
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
    public enum SlideDirection
    {
        RIGHT,
        LEFT
    }

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
    public bool IsChanged;

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
    /// 右矢印の何をクリックしたらいいかを表示するテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _rightArrowInstructionText;

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
    /// 左矢印の何をクリックしたらいいかを表示するテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _leftArrowInstructionText;

    /// <summary>
    /// メインシーンに戻る指示テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _returnToMainSceneInstructionText;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        _characterExplanationView.Initialize();
        _operationInstructionView.Initialize();
        _ruleExplanationView.Initialize();

        
        ArrowImageSetUp();
        InstructionTextSetUp();
        SetEvent();
    }

    /// <summary>
    /// 矢印画像の初期設定
    /// </summary>
    private void ArrowImageSetUp()
    {
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

        RuleSceneAnimationUtility.InstructionTextAnimation(_rightArrowInstructionText,
                                                           new Vector2(0, 50));

        RuleSceneAnimationUtility.InstructionTextAnimation(_leftArrowInstructionText,
                                                           new Vector2(0, 50));
    }

    /// <summary>
    /// 指示テキストの初期設定
    /// </summary>
    private async UniTask InstructionTextSetUp()
    {
        DOTweenTMPAnimator tmproAnimator = new DOTweenTMPAnimator(_returnToMainSceneInstructionText);
        Sequence completeSequence = DOTween.Sequence();

        for (int i = 0; i < tmproAnimator.textInfo.characterCount; i++)
        {
            Vector3 curCharOffset = tmproAnimator.GetCharOffset(i);

            Sequence charSequence = DOTween.Sequence()
                .Append(tmproAnimator.DOOffsetChar(i, curCharOffset + new Vector3(0, 30, 0), 0.4f)
                .SetEase(Ease.OutFlash, 2))
                .SetDelay(0.07f * i);

            completeSequence.Insert(0, charSequence); // 各文字のアニメーションを全体のシーケンスに追加
        }
        completeSequence.SetLink(gameObject);

        await completeSequence.OnComplete(async () =>
        {

            await UniTask.WaitForSeconds(2);
            InstructionTextSetUp().Forget();
        });
    }

    /// <summary>
    /// イベント設定
    /// </summary>
    private void SetEvent()
    {
        //右シフトキーをクリックしたときの処理
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.RightShift))
            .Subscribe(_ => CanvasSlide(SlideDirection.RIGHT).Forget())
            .AddTo(this);

        //左シフトキーをクリックしたときの処理
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.LeftShift))
            .Subscribe(_ => CanvasSlide(SlideDirection.LEFT).Forget())
            .AddTo(this);

        //右矢印画像をマウスでクリックしたときの処理
        _rightArrowImage.GetComponent<Button>().OnClickAsObservable()
            .Subscribe(_ => CanvasSlide(SlideDirection.RIGHT).Forget())
            .AddTo(this);

        //左矢印画像をマウスでクリックしたときの処理
        _leftArrowImage.GetComponent<Button>().OnClickAsObservable()
            .Subscribe(_ => CanvasSlide(SlideDirection.LEFT).Forget())
            .AddTo(this);
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
    public async UniTask CanvasSlide(SlideDirection slideDirection)
    {
        AudioManager.Instance.PlaySFX((int)SFX.SLIDEANIMATION);

        if(slideDirection == SlideDirection.RIGHT
            && _operationInstructionView.CanvasType != DisplayCanvasType.RULE
            && !IsChanged)
        {
            IsChanged = true;

            ArrowActiveChange(false, false);
            _returnToMainSceneInstructionText.gameObject.SetActive(false);

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
            _returnToMainSceneInstructionText.gameObject.SetActive(true);

            IsChanged = false;
        }

        else if(slideDirection == SlideDirection.LEFT
            && _operationInstructionView.CanvasType != DisplayCanvasType.CHARACTER
            && !IsChanged)
        {
            IsChanged = true;

            ArrowActiveChange(false, false);
            _returnToMainSceneInstructionText.gameObject.SetActive(false);

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
            _returnToMainSceneInstructionText.gameObject.SetActive(true);

            IsChanged = false;
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
        _rightArrowInstructionText.gameObject.SetActive(rightActive);

        _leftArrowImage.gameObject.SetActive(leftActive);
        _leftArrowInstructionText.gameObject.SetActive(leftActive);
    }
}
