using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEditor;

public class InGameView : MonoBehaviour
{
    /// <summary>
    /// パックマンのライフをパックマンで表示するためのコンテナ
    /// ここにライフ分のパックマンを生成する
    /// </summary>
    [SerializeField]
    private Transform _packManLifeContainer;

    /// <summary>
    /// パックマンのライフを表示するためのPrefab
    /// </summary>
    [SerializeField]
    private GameObject _packManLifePrefab;

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
    public void Initialize()
    {
        ResetView();

        SetEvent();
    }

    /// <summary>
    /// イベント設定
    /// </summary>
    private void SetEvent()
    {
        GameManager.Instance.OnLoseLifeAsObservable
            .Subscribe(_ =>
                LoseLife()
            ).AddTo(this);
    }

    public async void ResetView()
    {
        for(int i = 0; i < GameManager.Instance.Life - 1; i++)
        {
            var packMan = Instantiate(_packManLifePrefab,
                          _packManLifeContainer.position + new Vector3(2, 0, 0) * i,
                          Quaternion.Euler(0, 90, 0),
                          _packManLifeContainer);

            packMan.GetComponent<SphereCollider>().enabled = false;

            Animator animator = packMan.GetComponent<Animator>();
            animator.speed = 0;
        }

        _canStartProp.Value = false;
        UIExtension.TextVisibleSetting(_readyText, 1.0f);

        await UniTask.WaitForSeconds(2.0f);

        _canStartProp.Value = true;
        UIExtension.TextVisibleSetting(_readyText, 0);
    }

    public void LoseLife()
    {
        if(_packManLifeContainer.childCount > 0)
        {
            Transform lastChild = _packManLifeContainer.GetChild(_packManLifeContainer.childCount - 1);

            Destroy(lastChild.gameObject);
        }
    }
}
