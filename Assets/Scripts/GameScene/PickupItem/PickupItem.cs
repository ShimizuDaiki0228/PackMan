using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    /// <summary>
    /// �\������Ă��������܂łɂ�����܂ł̎���
    /// </summary>
    private const float DESTROY_TIME = 10;

    /// <summary>
    /// �擾�����Ƃ��ɓ���X�R�A
    /// </summary>
    public ItemData ItemData;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, DESTROY_TIME);
    }
}
