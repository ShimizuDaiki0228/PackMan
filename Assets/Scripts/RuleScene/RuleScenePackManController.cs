using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleScenePackManController : MonoBehaviour
{
    private readonly Vector3 _firstPosition = new Vector3(17.5f, 1.5f, 0);

    /// <summary>
    /// �����Ă�������Ɉړ�����悤��
    /// </summary>
    private Vector3 _positionOffset = new Vector3(0, 0, 0);

    /// <summary>
    /// �蓮Update
    /// </summary>
    public void ManualUpdate()
    {
        gameObject.transform.position += _positionOffset * Time.deltaTime;

        Vector3 screenPosition = Camera.main.WorldToViewportPoint(transform.position);

        if (screenPosition.x < 0 || screenPosition.x > 1)
            screenPosition.x = 1 - screenPosition.x;
        
        if (screenPosition.y < 0 || screenPosition.y > 1)
            screenPosition.y = 1 - screenPosition.y;

        transform.position = Camera.main.ViewportToWorldPoint(screenPosition);
    }

    /// <summary>
    /// �i�ޕ�����ύX����
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="positionOffset"></param>
    public void ChangeMoveDirection(Vector3 direction, Vector3 positionOffset)
    {
        gameObject.transform.eulerAngles = direction;
        _positionOffset = positionOffset;
    }

    /// <summary>
    /// ���Z�b�g
    /// </summary>
    public void Reset()
    {
        gameObject.transform.position = _firstPosition;
        gameObject.transform.eulerAngles = new Vector3(-90, 0, 0);
    }
}
