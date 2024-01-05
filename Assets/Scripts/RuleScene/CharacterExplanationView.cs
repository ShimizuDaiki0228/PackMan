using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterExplanationView : MonoBehaviour
{
    /// <summary>
    /// �I�u�W�F�N�g�̐������s���e�L�X�g�̈ʒu����
    /// </summary>
    private readonly Vector2 _textPositionOffset = new Vector2(450, 60);

    /// <summary>
    /// �J����
    /// </summary>
    [SerializeField]
    private Camera _camera;

    /// <summary>
    /// �p�b�N�}��
    /// </summary>
    [SerializeField]
    private Transform _packMan;

    /// <summary>
    /// �p�b�N�}����\������ʒu
    /// </summary>
    private Vector3 _packManDisplayPosition;

    /// <summary>
    /// �u�����L�[
    /// </summary>
    [SerializeField]
    private Transform _blinky;

    /// <summary>
    /// �u�����L�[��\������ʒu
    /// </summary>
    private Vector3 _blinkyDisplayPosition;

    /// <summary>
    /// �C���L�[
    /// </summary>
    [SerializeField]
    private Transform _inky;

    /// <summary>
    /// �C���L�[��\������ʒu
    /// </summary>
    private Vector3 _inkyDisplayPosition;

    /// <summary>
    /// �N���C�h
    /// </summary>
    [SerializeField]
    private Transform _clyde;

    /// <summary>
    /// �N���C�h��\������ʒu
    /// </summary>
    private Vector3 _clydeDisplayPosition;

    /// <summary>
    /// �s���L�[
    /// </summary>
    [SerializeField]
    private Transform _pinky;

    /// <summary>
    /// �s���L�[��\������ʒu
    /// </summary>
    private Vector3 _pinkyDisplayPosition;

    /// <summary>
    /// �a
    /// </summary>
    [SerializeField]
    private Transform _pellet;

    /// <summary>
    /// �a��\������ʒu
    /// </summary>
    private Vector3 _pelletDisplayPosition;

    /// <summary>
    /// �p���[�a
    /// </summary>
    [SerializeField]
    private Transform _powerPellet;

    /// <summary>
    /// �p���[�a��\������ʒu
    /// </summary>
    private Vector3 _powerPelletDisplayPosition;

    /// <summary>
    /// �A�C�e��
    /// </summary>
    [SerializeField]
    private Transform _item;

    /// <summary>
    /// �A�C�e����\������ʒu
    /// </summary>
    private Vector3 _itemDisplayPosition;

    /// <summary>
    /// �p�b�N�}���̐����e�L�X�g
    /// </summary>
    [SerializeField]
    private RectTransform _packManText;

    /// <summary>
    /// �u�����L�[�̐����e�L�X�g
    /// </summary>
    [SerializeField]
    private RectTransform _blinkyText;

    /// <summary>
    /// �C���L�[�̐����e�L�X�g
    /// </summary>
    [SerializeField]
    private RectTransform _inkyText;

    /// <summary>
    /// �N���C�h�̐����e�L�X�g
    /// </summary>
    [SerializeField]
    private RectTransform _clydeText;

    /// <summary>
    /// �s���L�[�̐����e�L�X�g
    /// </summary>
    [SerializeField]
    private RectTransform _pinkyText;

    /// <summary>
    /// �G�T�̐����e�L�X�g
    /// </summary>
    [SerializeField]
    private RectTransform _pelletText;

    /// <summary>
    /// �p���[�G�T�̐����e�L�X�g
    /// </summary>
    [SerializeField]
    private RectTransform _powerPelletText;

    /// <summary>
    /// �A�C�e���̐����e�L�X�g
    /// </summary>
    [SerializeField]
    private RectTransform _itemText;

    /// <summary>
    /// �w�i�p�l��
    /// </summary>
    [SerializeField]
    private RectTransform _panel;

    /// <summary>
    /// �p�l����\������ꏊ
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
    /// 3D�I�u�W�F�N�g�̐����e�L�X�g�̈ʒu�������s��
    /// </summary>
    private void TextPositionAdjustment(Transform objectTransform, RectTransform textTransform)
    {
        Vector2 screenPoint = _camera.WorldToScreenPoint(objectTransform.position);

        // UI�v�f���X�N���[�����W�ɍ��킹��
        textTransform.anchoredPosition = screenPoint
                                        - new Vector2(Screen.width / 2, Screen.height / 2)
                                        + _textPositionOffset;
    }

    /// <summary>
    /// 3D�I�u�W�F�N�g��UI���A�j���[�V����������
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
