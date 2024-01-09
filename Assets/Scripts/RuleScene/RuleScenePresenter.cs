using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RuleScenePresenter : MonoBehaviour
{
    /// <summary>
    /// View
    /// </summary>
    [SerializeField]
    private RuleSceneView _view;

    private void Start()
    {
        _view.Initialize();

        AudioManager.Instance.PlayBGM((int)BGM.RULE);
    }

    private async void Update()
    {
        _view.ManualUpdate();

        if(Input.GetKeyDown(KeyCode.Space) && !_view.IsChanged)
        {
            AudioManager.Instance.PlaySFX((int)SFX.SPACEKEYCLICK);

            await UniTask.WaitForSeconds(1f);

            SceneManager.LoadScene("MainScene");
        }
    }
}
