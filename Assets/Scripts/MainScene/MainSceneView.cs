using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Cysharp.Threading.Tasks;

public class MainSceneView : MonoBehaviour
{
    /// <summary>
    /// スペースキーをクリックするように誘導するテキスト
    /// </summary>
    [SerializeField]
    private CanvasGroup _instructionText;

    /// <summary>
    /// テキストの透明度のアニメーションを行うシーケンス
    /// </summary>
    private Sequence _instructionTextFadeSequence;

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
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        Sequence instructionTextSequence = AnimationUtility.TextFadeAnimation(_instructionText,
                                                               InGameConst.TEXT_FADE_DURATION,
                                                               gameObject);

        AnimationUtility.RotateTextAnimation(_playerSelectArrowText,
                                             new Vector3(360f, 0, 0),
                                             3f);

        _playerSelectTextAnimator = new DOTweenTMPAnimator(_playerSelectText);

        TextSetup();
        RotateSelectPlayerText();
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
}
