using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;

public class InGameView : MonoBehaviour
{
    /// <summary>
    /// 準備中に表示されるテキスト
    /// </summary>
    [SerializeField]
    private CanvasGroup _readyText;

    /// <summary>
    /// ゲームを開始してもいいかどうかのプロパティ
    /// </summary>
    private readonly ReactiveProperty<bool> _canStartProp = new ReactiveProperty<bool>(false);
    public IReadOnlyReactiveProperty<bool> CanStartProp => _canStartProp;

    /// <summary>
    /// 初期化
    /// </summary>
    public async void Initialize()
    {
        ResetView();
    }

    public async void ResetView()
    {
        _canStartProp.Value = false;
        UIExtension.TextVisibleSetting(_readyText, 1.0f);

        await UniTask.WaitForSeconds(2.0f);

        _canStartProp.Value = true;
        UIExtension.TextVisibleSetting(_readyText, 0);
    }
}
