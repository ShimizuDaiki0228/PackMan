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
    /// �n�C�X�R�A�e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _highScoreText;

    /// <summary>
    /// �v���C���[��I��������e�L�X�g
    /// </summary>
    [SerializeField]
    private Text _playerSelectArrowText;

    /// <summary>
    /// �v���C���[��I������e�L�X�g
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _playerSelectText;

    /// <summary>
    /// �v���C���[�I���e�L�X�g�̃A�j���[�^�[
    /// </summary>
    private DOTweenTMPAnimator _playerSelectTextAnimator;

    /// <summary>
    /// �v���C�{�^��
    /// </summary>
    public Button PlayButton;

    /// <summary>
    /// ���[���m�F�{�^��
    /// </summary>
    [SerializeField]
    private Button _ruleButton;

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
    }

    /// <summary>
    /// �{�^���̏����ݒ�
    /// </summary>
    private void ButtonSetup()
    {
        var playButtonEventTrigger = PlayButton.gameObject.AddComponent<ObservableEventTrigger>();
        var playButtonOutline = PlayButton.gameObject.GetComponent<Outline>();
        var ruleButtonEventTrigger = _ruleButton.gameObject.AddComponent<ObservableEventTrigger>();
        var ruleButtonOutline = _ruleButton.gameObject.GetComponent<Outline>();

        Sequence playButtonOutlineSequence = DOTween.Sequence();
        Sequence ruleButtonOutlineSequence = DOTween.Sequence();

        playButtonOutlineSequence = AnimationUtility.OutlineColorChangeSequence(playButtonOutline);
        playButtonOutlineSequence.Pause();
        ruleButtonOutlineSequence = AnimationUtility.OutlineColorChangeSequence(ruleButtonOutline);
        ruleButtonOutlineSequence.Pause();

        //�{�^���Ƀ}�E�X�I�[�o�[�����Ƃ��̃C�x���g�w��
        playButtonEventTrigger.OnPointerEnterAsObservable()
            .Subscribe(_ => MouseEnterButton(playButtonOutlineSequence, PlayButton.transform))
            .AddTo(this);

        ruleButtonEventTrigger.OnPointerEnterAsObservable()
            .Subscribe(_ => MouseEnterButton(ruleButtonOutlineSequence, _ruleButton.transform))
            .AddTo(this);

        //�{�^������}�E�X���O�ꂽ���̃C�x���g�w��
        playButtonEventTrigger.OnPointerExitAsObservable()
            .Subscribe(_ => MouseExitButton(playButtonOutlineSequence, playButtonOutline, PlayButton.transform))
        .AddTo(this);

        ruleButtonEventTrigger.OnPointerExitAsObservable()
            .Subscribe(_ => MouseExitButton(ruleButtonOutlineSequence, ruleButtonOutline, _ruleButton.transform))
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
        AnimationUtility.OnPointerEnterButtonTween(button);

        outlineSequence.Play();
    }

    /// <summary>
    /// �{�^���̏ォ��}�E�X���O�ꂽ��
    /// </summary>
    private void MouseExitButton(Sequence outlineSequence, Outline outline, Transform button)
    {
        AnimationUtility.OnPointerExitButtonTween(button);

        outlineSequence.Pause();
        outline.effectColor = Color.white;
    }
}
