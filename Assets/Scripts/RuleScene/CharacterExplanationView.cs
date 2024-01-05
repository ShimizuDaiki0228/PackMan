using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterExplanationView : MonoBehaviour
{
    /// <summary>
    /// オブジェクトの説明を行うテキストの位置調整
    /// </summary>
    private readonly Vector2 _textPositionOffset = new Vector2(450, 60);

    /// <summary>
    /// カメラ
    /// </summary>
    [SerializeField]
    private Camera _camera;

    /// <summary>
    /// パックマン
    /// </summary>
    [SerializeField]
    private Transform _packMan;

    /// <summary>
    /// パックマンを表示する位置
    /// </summary>
    private Vector3 _packManDisplayPosition;

    /// <summary>
    /// ブリンキー
    /// </summary>
    [SerializeField]
    private Transform _blinky;

    /// <summary>
    /// ブリンキーを表示する位置
    /// </summary>
    private Vector3 _blinkyDisplayPosition;

    /// <summary>
    /// インキー
    /// </summary>
    [SerializeField]
    private Transform _inky;

    /// <summary>
    /// インキーを表示する位置
    /// </summary>
    private Vector3 _inkyDisplayPosition;

    /// <summary>
    /// クライド
    /// </summary>
    [SerializeField]
    private Transform _clyde;

    /// <summary>
    /// クライドを表示する位置
    /// </summary>
    private Vector3 _clydeDisplayPosition;

    /// <summary>
    /// ピンキー
    /// </summary>
    [SerializeField]
    private Transform _pinky;

    /// <summary>
    /// ピンキーを表示する位置
    /// </summary>
    private Vector3 _pinkyDisplayPosition;

    /// <summary>
    /// 餌
    /// </summary>
    [SerializeField]
    private Transform _pellet;

    /// <summary>
    /// 餌を表示する位置
    /// </summary>
    private Vector3 _pelletDisplayPosition;

    /// <summary>
    /// パワー餌
    /// </summary>
    [SerializeField]
    private Transform _powerPellet;

    /// <summary>
    /// パワー餌を表示する位置
    /// </summary>
    private Vector3 _powerPelletDisplayPosition;

    /// <summary>
    /// アイテム
    /// </summary>
    [SerializeField]
    private Transform _item;

    /// <summary>
    /// アイテムを表示する位置
    /// </summary>
    private Vector3 _itemDisplayPosition;

    /// <summary>
    /// パックマンの説明テキスト
    /// </summary>
    [SerializeField]
    private RectTransform _packManText;

    /// <summary>
    /// ブリンキーの説明テキスト
    /// </summary>
    [SerializeField]
    private RectTransform _blinkyText;

    /// <summary>
    /// インキーの説明テキスト
    /// </summary>
    [SerializeField]
    private RectTransform _inkyText;

    /// <summary>
    /// クライドの説明テキスト
    /// </summary>
    [SerializeField]
    private RectTransform _clydeText;

    /// <summary>
    /// ピンキーの説明テキスト
    /// </summary>
    [SerializeField]
    private RectTransform _pinkyText;

    /// <summary>
    /// エサの説明テキスト
    /// </summary>
    [SerializeField]
    private RectTransform _pelletText;

    /// <summary>
    /// パワーエサの説明テキスト
    /// </summary>
    [SerializeField]
    private RectTransform _powerPelletText;

    /// <summary>
    /// アイテムの説明テキスト
    /// </summary>
    [SerializeField]
    private RectTransform _itemText;

    /// <summary>
    /// 背景パネル
    /// </summary>
    [SerializeField]
    private RectTransform _panel;

    /// <summary>
    /// パネルを表示する場所
    /// </summary>
    private Vector2 _panelDisplayPosition;
    
    public void Initialize()
    {
        _packManDisplayPosition = _packMan.position;
        _blinkyDisplayPosition = _blinky.position;
        _inkyDisplayPosition = _inky.position;
        _clydeDisplayPosition = _clyde.position;
        _pinkyDisplayPosition = _pinky.position;
        _pelletDisplayPosition = _pellet.position;
        _powerPelletDisplayPosition = _powerPellet.position;
        _itemDisplayPosition = _item.position;
        _panelDisplayPosition = _panel.position;

        _packMan.position += RuleSceneAnimationUtility.FirstObjectDisplayPosition;
        _blinky.position += RuleSceneAnimationUtility.FirstObjectDisplayPosition;
        _inky.position += RuleSceneAnimationUtility.FirstObjectDisplayPosition;
        _clyde.position += RuleSceneAnimationUtility.FirstObjectDisplayPosition;
        _pinky.position += RuleSceneAnimationUtility.FirstObjectDisplayPosition;
        _pellet.position += RuleSceneAnimationUtility.FirstObjectDisplayPosition;
        _powerPellet.position += RuleSceneAnimationUtility.FirstObjectDisplayPosition;
        _item.position += RuleSceneAnimationUtility.FirstObjectDisplayPosition;
        _panel.anchoredPosition += RuleSceneAnimationUtility.FirstPanelDisplayPosition;

        SlideAnimation(RuleSceneAnimationUtility.Direction.DOWN,
                       - RuleSceneAnimationUtility.FirstObjectDisplayPosition,
                       - RuleSceneAnimationUtility.FirstPanelDisplayPosition);
    }

    // Update is called once per frame
    public void ManualUpdate()
    {
        TextPositionAdjustment(_packMan, _packManText);
        TextPositionAdjustment(_blinky, _blinkyText);
        TextPositionAdjustment(_inky, _inkyText);
        TextPositionAdjustment(_clyde, _clydeText);
        TextPositionAdjustment(_pinky, _pinkyText);
        TextPositionAdjustment(_pellet, _pelletText);
        TextPositionAdjustment(_powerPellet, _powerPelletText);
        TextPositionAdjustment(_item, _itemText);
    }

    /// <summary>
    /// 3Dオブジェクトの説明テキストの位置調整を行う
    /// </summary>
    private void TextPositionAdjustment(Transform objectTransform, RectTransform textTransform)
    {
        Vector2 screenPoint = _camera.WorldToScreenPoint(objectTransform.position);

        // UI要素をスクリーン座標に合わせる
        textTransform.anchoredPosition = screenPoint
                                        - new Vector2(Screen.width / 2, Screen.height / 2)
                                        + _textPositionOffset;
    }

    /// <summary>
    /// 3DオブジェクトとUIをアニメーションさせる
    /// </summary>
    /// <param name="direction"></param>
    public void SlideAnimation(RuleSceneAnimationUtility.Direction direction, 
                               Vector3 objectDestinationOffset,
                               Vector2 UIDestinationOffset)
    {
        RuleSceneAnimationUtility.SlideObjectAnimationDirection(_packMan,
                                                                _packMan.transform.position + objectDestinationOffset,
                                                                direction);
        RuleSceneAnimationUtility.SlideObjectAnimationDirection(_blinky,
                                                                _blinky.transform.position + objectDestinationOffset,
                                                                direction);
        RuleSceneAnimationUtility.SlideObjectAnimationDirection(_inky,
                                                                _inky.transform.position + objectDestinationOffset,
                                                                direction);
        RuleSceneAnimationUtility.SlideObjectAnimationDirection(_clyde,
                                                                _clyde.transform.position + objectDestinationOffset,
                                                                direction);
        RuleSceneAnimationUtility.SlideObjectAnimationDirection(_pinky,
                                                                _pinky.transform.position + objectDestinationOffset,
                                                                direction);
        RuleSceneAnimationUtility.SlideObjectAnimationDirection(_pellet,
                                                                _pellet.transform.position + objectDestinationOffset,
                                                                direction);
        RuleSceneAnimationUtility.SlideObjectAnimationDirection(_powerPellet,
                                                                _powerPellet.transform.position + objectDestinationOffset,
                                                                direction);
        RuleSceneAnimationUtility.SlideObjectAnimationDirection(_item,
                                                                _item.transform.position + objectDestinationOffset,
                                                                direction);
        RuleSceneAnimationUtility.SlideUIAnimationDirection(_panel,
                                                            _panel.anchoredPosition + UIDestinationOffset,
                                                            direction);
    }
}
