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
    /// �w�i��ύX�����ǂ���
    /// </summary>
    public bool IsChanged;

    /// <summary>
    /// �E�����̖��摜
    /// </summary>
    [SerializeField]
    private Image _rightArrowImage;

    /// <summary>
    /// �E�����̖���CanvasGroup
    /// </summary>
    [SerializeField]
    private CanvasGroup _rightArrowCanvasGroup;

    /// <summary>
    /// �E���̉����N���b�N�����炢������\������e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _rightArrowInstructionText;

    /// <summary>
    /// �������̖��摜
    /// </summary>
    [SerializeField]
    private Image _leftArrowImage;

    /// <summary>
    /// �������̖���CanvasGroup
    /// </summary>
    [SerializeField]
    private CanvasGroup _leftArrowCanvasGroup;

    /// <summary>
    /// �����̉����N���b�N�����炢������\������e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _leftArrowInstructionText;

    /// <summary>
    /// ���C���V�[���ɖ߂�w���e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _returnToMainSceneInstructionText;

    /// <summary>
    /// ������
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
    /// ���摜�̏����ݒ�
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
    /// �w���e�L�X�g�̏����ݒ�
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

            completeSequence.Insert(0, charSequence); // �e�����̃A�j���[�V������S�̂̃V�[�P���X�ɒǉ�
        }
        completeSequence.SetLink(gameObject);

        await completeSequence.OnComplete(async () =>
        {

            await UniTask.WaitForSeconds(2);
            InstructionTextSetUp().Forget();
        });
    }

    /// <summary>
    /// �C�x���g�ݒ�
    /// </summary>
    private void SetEvent()
    {
        //�E�V�t�g�L�[���N���b�N�����Ƃ��̏���
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.RightShift))
            .Subscribe(_ => CanvasSlide(SlideDirection.RIGHT).Forget())
            .AddTo(this);

        //���V�t�g�L�[���N���b�N�����Ƃ��̏���
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.LeftShift))
            .Subscribe(_ => CanvasSlide(SlideDirection.LEFT).Forget())
            .AddTo(this);

        //�E���摜���}�E�X�ŃN���b�N�����Ƃ��̏���
        _rightArrowImage.GetComponent<Button>().OnClickAsObservable()
            .Subscribe(_ => CanvasSlide(SlideDirection.RIGHT).Forget())
            .AddTo(this);

        //�����摜���}�E�X�ŃN���b�N�����Ƃ��̏���
        _leftArrowImage.GetComponent<Button>().OnClickAsObservable()
            .Subscribe(_ => CanvasSlide(SlideDirection.LEFT).Forget())
            .AddTo(this);
    }

    /// <summary>
    /// �蓮Update
    /// </summary>
    public void ManualUpdate()
    {
        _characterExplanationView.ManualUpdate();
        _operationInstructionView.ManualUpdate();
    }

    /// <summary>
    /// �w�i���X���C�h������
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
    /// DisplayCanvasType��ύX����
    /// </summary>
    /// <param name="type">���݂̃^�C�v</param>
    /// <param name="value">�l���ǂꂾ���ύX�����邩</param>
    /// <returns></returns>
    private DisplayCanvasType ChangeCanvasType(DisplayCanvasType type, int value)
    {
        int nextValue = (int)type + value;
        return (DisplayCanvasType)nextValue;
    }

    /// <summary>
    /// �E���摜�ƍ����摜�̃A�N�e�B�u��ύX����
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
