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
    /// ハイスコアテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _highScoreText;

    /// <summary>
    /// プレイヤーを選択する矢印テキスト
    /// </summary>
    [SerializeField]
    private Text _playerSelectArrowText;

    /// <summary>
    /// プレイヤーを選択するテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _playerSelectText;

    /// <summary>
    /// プレイヤー選択テキストのアニメーター
    /// </summary>
    private DOTweenTMPAnimator _playerSelectTextAnimator;

    /// <summary>
    /// プレイボタン
    /// </summary>
    public Button PlayButton;

    /// <summary>
    /// ルール確認ボタン
    /// </summary>
    [SerializeField]
    private Button _ruleButton;

    /// <summary>
    /// 初期化
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
    /// ボタンの初期設定
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

        //ボタンにマウスオーバーしたときのイベント購読
        playButtonEventTrigger.OnPointerEnterAsObservable()
            .Subscribe(_ => MouseEnterButton(playButtonOutlineSequence, PlayButton.transform))
            .AddTo(this);

        ruleButtonEventTrigger.OnPointerEnterAsObservable()
            .Subscribe(_ => MouseEnterButton(ruleButtonOutlineSequence, _ruleButton.transform))
            .AddTo(this);

        //ボタンからマウスが外れた時のイベント購読
        playButtonEventTrigger.OnPointerExitAsObservable()
            .Subscribe(_ => MouseExitButton(playButtonOutlineSequence, playButtonOutline, PlayButton.transform))
        .AddTo(this);

        ruleButtonEventTrigger.OnPointerExitAsObservable()
            .Subscribe(_ => MouseExitButton(ruleButtonOutlineSequence, ruleButtonOutline, _ruleButton.transform))
            .AddTo(this);
    }

    /// <summary>
    /// プレイヤー選択テキストを定期的に回転させる
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
    /// テキストの初期設定
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
    /// ボタンの上にマウスが乗った時
    /// </summary>
    private void MouseEnterButton(Sequence outlineSequence, Transform button)
    {
        AnimationUtility.OnPointerEnterButtonTween(button);

        outlineSequence.Play();
    }

    /// <summary>
    /// ボタンの上からマウスが外れた時
    /// </summary>
    private void MouseExitButton(Sequence outlineSequence, Outline outline, Transform button)
    {
        AnimationUtility.OnPointerExitButtonTween(button);

        outlineSequence.Pause();
        outline.effectColor = Color.white;
    }
}
