using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainSceneView : MonoBehaviour
{
    /// <summary>
    /// �n�C�X�R�A�̕����e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _highScoreStringText;

    /// <summary>
    /// �n�C�X�R�A�e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _highScoreText;

    /// <summary>
    /// �^�C�g���e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _titleText;

    /// <summary>
    /// �v���C���[��I��������e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _playerSelectArrowText;

    /// <summary>
    /// �v���C���[��I������e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _playerSelectText;

    /// <summary>
    /// ����҂̃e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _nameText;

    /// <summary>
    /// �v���C���[�I���e�L�X�g�̃A�j���[�^�[
    /// </summary>
    private DOTweenTMPAnimator _playerSelectTextAnimator;

    /// <summary>
    /// �v���C�{�^��
    /// </summary>
    public Button PlayButton;

    /// <summary>
    /// �v���C�{�^���̃A�E�g���C��
    /// </summary>
    private Outline _playButtonOutline;

    /// <summary>
    /// �v���C�{�^���̃e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _playButtonText;

    private Sequence _playButtonOutlineSequence;

    /// <summary>
    /// ���[���m�F�{�^��
    /// </summary>
    [SerializeField]
    private Button _ruleButton;

    /// <summary>
    /// ���[���m�F�{�^���̃A�E�g���C��
    /// </summary>
    private Outline _ruleButtonOutline;

    /// <summary>
    /// ���[���m�F�{�^���̃e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _ruleButtonText;

    /// <summary>
    /// �v���C�{�^�����N���b�N���ꂽ���ǂ���
    /// </summary>
    public ReactiveProperty<bool> IsPlayButtonClickedProp;
    public bool IsPlayButtonClicked => IsPlayButtonClickedProp.Value;

    /// <summary>
    /// ������
    /// </summary>
    public void Initialize()
    {
        AnimationUtility.RotateTextAnimation(_playerSelectArrowText,
                                             new Vector3(360f, 0, 0),
                                             3f);

        _playerSelectTextAnimator = new DOTweenTMPAnimator(_playerSelectText);

        _highScoreText.text = GameManager.Instance.HighScore.ToString();

        ButtonSetup();

        TextSetup();
        RotateSelectPlayerText();

        SetEvent();
    }

    /// <summary>
    /// �C�x���g�ݒ�
    /// </summary>
    private void SetEvent()
    {
        PlayButton.onClick.AsObservable()
            .Subscribe(_ =>
                ClickPlayButton()
            ).AddTo(this);

        IsPlayButtonClickedProp
            .Where(isClicked => isClicked)
            .Subscribe(_ =>
                AfterPlayButtonClick()
            ).AddTo(this);
    }

    /// <summary>
    /// �{�^���̏����ݒ�
    /// </summary>
    private void ButtonSetup()
    {
        var playButtonEventTrigger = PlayButton.gameObject.AddComponent<ObservableEventTrigger>();
        _playButtonOutline = PlayButton.gameObject.GetComponent<Outline>();
        var ruleButtonEventTrigger = _ruleButton.gameObject.AddComponent<ObservableEventTrigger>();
        _ruleButtonOutline = _ruleButton.gameObject.GetComponent<Outline>();

        _playButtonOutlineSequence = DOTween.Sequence();
        Sequence ruleButtonOutlineSequence = DOTween.Sequence();

        _playButtonOutlineSequence = AnimationUtility.OutlineColorChangeSequence(_playButtonOutline);
        _playButtonOutlineSequence.Pause();
        ruleButtonOutlineSequence = AnimationUtility.OutlineColorChangeSequence(_ruleButtonOutline);
        ruleButtonOutlineSequence.Pause();

        //�{�^���Ƀ}�E�X�I�[�o�[�����Ƃ��̃C�x���g�w��
        playButtonEventTrigger.OnPointerEnterAsObservable()
            .Subscribe(_ => MouseEnterButton(_playButtonOutlineSequence, PlayButton.transform))
            .AddTo(this);

        ruleButtonEventTrigger.OnPointerEnterAsObservable()
            .Subscribe(_ => MouseEnterButton(ruleButtonOutlineSequence, _ruleButton.transform))
            .AddTo(this);

        //�{�^������}�E�X���O�ꂽ���̃C�x���g�w��
        playButtonEventTrigger.OnPointerExitAsObservable()
            .Subscribe(_ => MouseExitButton(_playButtonOutlineSequence, _playButtonOutline, PlayButton.transform))
        .AddTo(this);

        ruleButtonEventTrigger.OnPointerExitAsObservable()
            .Subscribe(_ => MouseExitButton(ruleButtonOutlineSequence, _ruleButtonOutline, _ruleButton.transform))
            .AddTo(this);
    }

    /// <summary>
    /// �v���C���[�I���e�L�X�g�����I�ɉ�]������
    /// </summary>
    private async void RotateSelectPlayerText()
    {
        while (true)
        {
            AnimationUtility.RotateTextMeshProAnimationSequence(_playerSelectTextAnimator,
                                                            2.5f);

            await UniTask.WaitForSeconds(3.5f);
        }
    }


    /// <summary>
    /// �e�L�X�g�̏����ݒ�
    /// </summary>
    private void TextSetup()
    {
        for (var i = 0; i < _playerSelectTextAnimator.textInfo.characterCount; i++)
        {
            _playerSelectTextAnimator.DORotateChar(i, Vector3.right * 90, 0);
            _playerSelectTextAnimator.DOOffsetChar(i, Vector3.zero, 0);
            _playerSelectTextAnimator.DOFadeChar(i, 1, 0);
        }
    }

    /// <summary>
    /// �{�^���̏�Ƀ}�E�X���������
    /// </summary>
    private void MouseEnterButton(Sequence outlineSequence, Transform button)
    {
        if(!IsPlayButtonClicked)
        {
            AnimationUtility.OnPointerEnterButtonTween(button);

            outlineSequence.Play();
        }
    }

    /// <summary>
    /// �{�^���̏ォ��}�E�X���O�ꂽ��
    /// </summary>
    private void MouseExitButton(Sequence outlineSequence, Outline outline, Transform button)
    {
        if(!IsPlayButtonClicked)
        {
            AnimationUtility.OnPointerExitButtonTween(button);

            outlineSequence.Pause();
            outline.effectColor = Color.white;
        }
    }

    /// <summary>
    /// Play�{�^������������ɐF��傫����߂��悤��
    /// </summary>
    private void AfterPlayButtonClick()
    {
        AnimationUtility.OnPointerExitButtonTween(PlayButton.transform);

        _playButtonOutlineSequence.Pause();
    }

    /// <summary>
    /// �v���C�{�^�����N���b�N���ꂽ�Ƃ�
    /// </summary>
    private void ClickPlayButton()
    {
        EscapeText(_highScoreStringText);
        EscapeText(_highScoreText);
        EscapeText(_titleText);
        EscapeText(_playerSelectText);
        EscapeText(_playerSelectArrowText);
        EscapeText(_nameText);
        EscapeButton(PlayButton, _playButtonOutline, _playButtonText);
        EscapeButton(_ruleButton, _ruleButtonOutline, _ruleButtonText);
    }

    /// <summary>
    /// �e�L�X�g���p�b�N�}�����瓦����悤��
    /// </summary>
    private void EscapeText(TextMeshProUGUI text)
    {
        text.color = new Color(1, 0, 1);
        text.rectTransform.DOAnchorPosX(text.rectTransform.anchoredPosition.x + 1.5f, Time.deltaTime)
            .SetEase(Ease.Linear) // ���`�̓���
            .SetLoops(-1, LoopType.Incremental)
            .SetLink(gameObject);
    }
    
    /// <summary>
    /// �e�L�X�g���p�b�N�}�����瓦����悤��
    /// </summary>
    private void EscapeButton(Button button, Outline outline, TextMeshProUGUI text)
    {
        outline.effectColor = new Color(1, 0, 1);

        text.color = new Color(1, 0, 1);
        button.transform.DOMoveX(button.transform.position.x + 3.5f, Time.deltaTime)
            .SetEase(Ease.Linear) // ���`�̓���
            .SetLoops(-1, LoopType.Incremental)
            .SetLink(gameObject);
    }
}
