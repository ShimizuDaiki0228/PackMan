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
    [SerializeField]
    private ItemData _itemData;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, DESTROY_TIME);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PackMan")
        {
            
            GameManager.Instance.AddScore(_itemData.Score);
            Destroy(gameObject);
        }
    }
}
