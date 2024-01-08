using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverSceneManager : MonoBehaviour
{
    /// <summary>
    /// タイトルテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _titleText;

    /// <summary>
    /// スコアの「Score」と表示させる文字
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _scoreStringText;

    /// <summary>
    /// スコアの数字テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _scoreText;

    /// <summary>
    /// ハイスコアの「High Score」と表示させる文字
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _highScoreStringText;

    /// <summary>
    /// ハイスコアの数字テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _highScoreText;

    /// <summary>
    /// レベルの「Level」と表示させる文字
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _levelStringText;

    /// <summary>
    /// レベルの数字テキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _levelText;

    /// <summary>
    /// ハイスコアを更新したときに表示されるテキスト
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _newHighScoreText;

    /// <summary>
    /// スペースキーをクリックするように誘導するテキスト
    /// </summary>
    [SerializeField]
    private CanvasGroup _instructionText;

    /// <summary>
    /// ハイスコアを更新したときに生成する光るエフェクト
    /// </summary>
    [SerializeField]
    private GameObject _kirakiraEffect;

    /// <summary>
    /// ハイスコアを更新したときに生成するエフェクト
    /// </summary>
    [SerializeField]
    private GameObject _newHighScoreEffect;

    /// <summary>
    /// テキストを画面内に表示するまでにかかる時間
    /// </summary>
    private const float TEXT_DISPLAY_DURATION = 3f;

    /// <summary>
    /// スペースキーを押してメイン画面に戻れるかどうか
    /// </summary>
    private bool _isNextReady;

    private void Awake()
    {
        _titleText.transform.position += InGameConst.GameOverSceneTextOffset;
        _scoreStringText.transform.position += InGameConst.GameOverSceneTextOffset;
        _scoreText.transform.position += InGameConst.GameOverSceneTextOffset;
        _levelStringText.transform.position += InGameConst.GameOverSceneTextOffset;
        _levelText.transform.position += InGameConst.GameOverSceneTextOffset;

        _highScoreStringText.gameObject.SetActive(false);
        _highScoreText.gameObject.SetActive(false);

        _highScoreText.text = GameManager.Instance.HighScore.ToString();

        _newHighScoreText.DOFade(0, 0);

        UIUtility.ChangeTextTransparent(_instructionText, 0f);
    }

    private async void Start()
    {
        Tween tween1 = AnimationUtility.TextSlideAnimation(_titleText, _titleText.transform.position -InGameConst.GameOverSceneTextOffset, TEXT_DISPLAY_DURATION);
        Tween tween2 = AnimationUtility.TextSlideAnimation(_scoreStringText, _scoreStringText.transform.position -InGameConst.GameOverSceneTextOffset, TEXT_DISPLAY_DURATION);
        Tween tween3 = AnimationUtility.TextSlideAnimation(_scoreText, _scoreText.transform.position - InGameConst.GameOverSceneTextOffset, TEXT_DISPLAY_DURATION);
        Tween tween4 = AnimationUtility.TextSlideAnimation(_levelStringText, _levelStringText.transform.position - InGameConst.GameOverSceneTextOffset, TEXT_DISPLAY_DURATION);
        Tween tween5 = AnimationUtility.TextSlideAnimation(_levelText, _levelText.transform.position - InGameConst.GameOverSceneTextOffset, TEXT_DISPLAY_DURATION);

        // すべてのTweenが完了するまで待機
        await UniTask.WhenAll(tween1.ToUniTask(), tween2.ToUniTask(), tween3.ToUniTask(), tween4.ToUniTask(), tween5.ToUniTask());

        //スコアとレベルを順番に表示する
        //値は1文字ずつ表示するように
        _scoreText.text = GameManager.Instance.Score.ToString();
        Tween tween = AnimationUtility.TextDisplayInOrder(_scoreText,
                                            _scoreText.text.Length / 2,
                                            false,
                                            ScrambleMode.Numerals);

        await tween.ToUniTask();

        _levelText.text = GameManager.Instance.Level.ToString();
        tween = AnimationUtility.TextDisplayInOrder(_levelText,
                                                    _levelText.text.Length / 2,
                                                    false,
                                                    ScrambleMode.Numerals);

        await tween.ToUniTask();

        //メインシーンに戻る指示テキストを表示
        UIUtility.ChangeTextTransparent(_instructionText, 1f);

        _highScoreStringText.gameObject.SetActive(true);
        _highScoreText.gameObject.SetActive(true);

        Sequence sequence = AnimationUtility.TextFadeAnimation(_instructionText,
                                                               InGameConst.TEXT_FADE_DURATION,
                                                               gameObject);

        if (GameManager.Instance.IsHighScoreUpdate)
        {
            NewHighScore().Forget();

            MonobehaviourUtility.Instance.EffectCreate(_kirakiraEffect, _highScoreText.transform.position);
            MonobehaviourUtility.Instance.EffectCreate(_newHighScoreEffect, _highScoreText.transform.position);
        }

        _isNextReady = true;
    }

    /// <summary>
    /// newというテキストを表示する
    /// テキストの表示とともにエフェクトを出す
    /// </summary>
    /// <returns></returns>
    private async UniTask NewHighScore()
    {
        DOTweenTMPAnimator newHighScoreTextAnimator
                    = new DOTweenTMPAnimator(_newHighScoreText);
        Sequence TextBounceSequence = DOTween.Sequence();

        TextBounceSequence =
            AnimationUtility.TextBounceAnimationSequence(newHighScoreTextAnimator,
                                                         0.7f,
                                                         new Vector3(0, 10, 0),
                                                         0.07f);

        await TextBounceSequence.ToUniTask();

        Sequence TextShineSequence = DOTween.Sequence();
        TextShineSequence =
            AnimationUtility.TextChangeColorSequence(newHighScoreTextAnimator,
                                                     new Color(1, 1, 0.8f),
                                                     0.07f);

        TextShineSequence.SetDelay(1f).SetLoops(-1).SetLink(gameObject); // 無限にループ
        TextShineSequence.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isNextReady)
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
