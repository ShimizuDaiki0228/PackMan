using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    private void Update()
    {
        _view.ManualUpdate();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            _view.CanvasSlide();
        }
    }
}
