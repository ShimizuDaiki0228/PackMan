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
    /// ハイスコアの文字テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _highScoreStringText;

    /// <summary>
    /// ハイスコアテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _highScoreText;

    /// <summary>
    /// タイトルテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _titleText;

    /// <summary>
    /// プレイヤーを選択する矢印テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _playerSelectArrowText;

    /// <summary>
    /// プレイヤーを選択するテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _playerSelectText;

    /// <summary>
    /// 制作者のテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _nameText;

    /// <summary>
    /// プレイヤー選択テキストのアニメーター
    /// </summary>
    private DOTweenTMPAnimator _playerSelectTextAnimator;

    /// <summary>
    /// プレイボタン
    /// </summary>
    public Button PlayButton;

    /// <summary>
    /// プレイボタンのアウトライン
    /// </summary>
    private Outline _playButtonOutline;

    /// <summary>
    /// プレイボタンのテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _playButtonText;

    private Sequence _playButtonOutlineSequence;

    /// <summary>
    /// ルール確認ボタン
    /// </summary>
    [SerializeField]
    private Button _ruleButton;

    /// <summary>
    /// ルール確認ボタンのアウトライン
    /// </summary>
    private Outline _ruleButtonOutline;

    /// <summary>
    /// ルール確認ボタンのテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _ruleButtonText;

    /// <summary>
    /// プレイボタンがクリックされたかどうか
    /// </summary>
    public ReactiveProperty<bool> IsPlayButtonClickedProp;
    public bool IsPlayButtonClicked => IsPlayButtonClickedProp.Value;

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

        SetEvent();
    }

    /// <summary>
    /// イベント設定
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
    /// ボタンの初期設定
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

        //ボタンにマウスオーバーしたときのイベント購読
        playButtonEventTrigger.OnPointerEnterAsObservable()
            .Subscribe(_ => MouseEnterButton(_playButtonOutlineSequence, PlayButton.transform))
            .AddTo(this);

        ruleButtonEventTrigger.OnPointerEnterAsObservable()
            .Subscribe(_ => MouseEnterButton(ruleButtonOutlineSequence, _ruleButton.transform))
            .AddTo(this);

        //ボタンからマウスが外れた時のイベント購読
        playButtonEventTrigger.OnPointerExitAsObservable()
            .Subscribe(_ => MouseExitButton(_playButtonOutlineSequence, _playButtonOutline, PlayButton.transform))
        .AddTo(this);

        ruleButtonEventTrigger.OnPointerExitAsObservable()
            .Subscribe(_ => MouseExitButton(ruleButtonOutlineSequence, _ruleButtonOutline, _ruleButton.transform))
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
        if(!IsPlayButtonClicked)
        {
            AnimationUtility.OnPointerEnterButtonTween(button);

            outlineSequence.Play();
        }
    }

    /// <summary>
    /// ボタンの上からマウスが外れた時
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
    /// Playボタンを押した後に色や大きさを戻すように
    /// </summary>
    private void AfterPlayButtonClick()
    {
        AnimationUtility.OnPointerExitButtonTween(PlayButton.transform);

        _playButtonOutlineSequence.Pause();
    }

    /// <summary>
    /// プレイボタンがクリックされたとき
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
    /// テキストがパックマンから逃げるように
    /// </summary>
    private void EscapeText(TextMeshProUGUI text)
    {
        text.color = new Color(1, 0, 1);
        text.rectTransform.DOAnchorPosX(text.rectTransform.anchoredPosition.x + 1.5f, Time.deltaTime)
            .SetEase(Ease.Linear) // 線形の動き
            .SetLoops(-1, LoopType.Incremental)
            .SetLink(gameObject);
    }
    
    /// <summary>
    /// テキストがパックマンから逃げるように
    /// </summary>
    private void EscapeButton(Button button, Outline outline, TextMeshProUGUI text)
    {
        outline.effectColor = new Color(1, 0, 1);

        text.color = new Color(1, 0, 1);
        button.transform.DOMoveX(button.transform.position.x + 3.5f, Time.deltaTime)
            .SetEase(Ease.Linear) // 線形の動き
            .SetLoops(-1, LoopType.Incremental)
            .SetLink(gameObject);
    }
}
