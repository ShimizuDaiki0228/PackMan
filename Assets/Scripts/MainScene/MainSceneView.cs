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
using UnityEngine.SceneManagement;

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

    /// <summary>
    /// プレイボタンのアニメーションシーケンス
    /// </summary>
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
    /// ルールボタンのアニメーションシーケンス
    /// </summary>
    Sequence _ruleButtonOutlineSequence;

    /// <summary>
    /// プレイボタンがクリックされたかどうか
    /// </summary>
    private readonly ReactiveProperty<bool> _isPlayButtonClickedProp = new ReactiveProperty<bool>(false);
    public ReactiveProperty<bool> IsPlayButtonClickedProp => _isPlayButtonClickedProp;
    public bool IsPlayButtonClicked => _isPlayButtonClickedProp.Value;

    /// <summary>
    /// ボタンクリック時のエフェクト
    /// </summary>
    [SerializeField]
    private GameObject _clickEffect;

    /// <summary>
    /// クリックエフェクトの時間
    /// </summary>
    private float _clickEffectDuration; 

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

        var mainModule = _clickEffect.GetComponent<ParticleSystem>().main;
        _clickEffectDuration = mainModule.startLifetime.constant;

        ButtonSetup();

        TextSetup();
        RotateSelectPlayerText();

        SetEvent();
    }

    /// <summary>
    /// イベント設定
    /// </summary>
    private async void SetEvent()
    {
        PlayButton.onClick.AsObservable()
            .Subscribe(async _ =>
            {
                UndoButton(_playButtonOutlineSequence, _playButtonOutline, PlayButton.transform);
                MonobehaviourUtility.Instance.EffectCreate(_clickEffect, PlayButton.transform.position);

                await UniTask.WaitForSeconds(_clickEffectDuration);

                _isPlayButtonClickedProp.Value = true;
                ClickPlayButton();

                await UniTask.WaitForSeconds(6f);

                SceneManager.LoadScene("GameScene");
            }
            ).AddTo(this);

        _ruleButton.onClick.AsObservable()
            .Subscribe(async _ =>
            {
                UndoButton(_ruleButtonOutlineSequence, _ruleButtonOutline, _ruleButton.transform);
                MonobehaviourUtility.Instance.EffectCreate(_clickEffect, _ruleButton.transform.position);

                await UniTask.WaitForSeconds(_clickEffectDuration);

                SceneManager.LoadScene("RuleScene");
            }).AddTo(this);
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
        _ruleButtonOutlineSequence = DOTween.Sequence();

        _playButtonOutlineSequence = AnimationUtility.OutlineColorChangeSequence(_playButtonOutline);
        _playButtonOutlineSequence.Pause();
        _ruleButtonOutlineSequence = AnimationUtility.OutlineColorChangeSequence(_ruleButtonOutline);
        _ruleButtonOutlineSequence.Pause();

        //ボタンにマウスオーバーしたときのイベント購読
        playButtonEventTrigger.OnPointerEnterAsObservable()
            .Subscribe(_ => MouseEnterButton(_playButtonOutlineSequence, PlayButton.transform))
            .AddTo(this);

        ruleButtonEventTrigger.OnPointerEnterAsObservable()
            .Subscribe(_ => MouseEnterButton(_ruleButtonOutlineSequence, _ruleButton.transform))
            .AddTo(this);

        //ボタンからマウスが外れた時のイベント購読
        playButtonEventTrigger.OnPointerExitAsObservable()
            .Subscribe(_ => MouseExitButton(_playButtonOutlineSequence, _playButtonOutline, PlayButton.transform))
        .AddTo(this);

        ruleButtonEventTrigger.OnPointerExitAsObservable()
            .Subscribe(_ => MouseExitButton(_ruleButtonOutlineSequence, _ruleButtonOutline, _ruleButton.transform))
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
            UndoButton(outlineSequence, outline, button);
        }
    }

    /// <summary>
    /// ボタンを元の状態に戻す
    /// </summary>
    /// <param name="sequence"></param>
    /// <param name="outline"></param>
    /// <param name="transform"></param>
    private void UndoButton(Sequence sequence, Outline outline, Transform transform)
    {
        AnimationUtility.OnPointerExitButtonTween(transform);

        sequence.Pause();
        outline.effectColor = Color.white;
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
        text.rectTransform.DOAnchorPosX(text.rectTransform.anchoredPosition.x + 1.3f, Time.deltaTime)
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
        button.transform.DOMoveX(button.transform.position.x + 0.4f, Time.deltaTime)
            .SetEase(Ease.Linear) // 線形の動き
            .SetLoops(-1, LoopType.Incremental)
            .SetLink(gameObject);
    }
}
