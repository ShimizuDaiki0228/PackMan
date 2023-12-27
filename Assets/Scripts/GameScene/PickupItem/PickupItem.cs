using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    /// <summary>
    /// 表示されてから消えるまでにかかるまでの時間
    /// </summary>
    private const float DESTROY_TIME = 10;

    /// <summary>
    /// 取得したときに入るスコア
    /// </summary>
    public ItemData ItemData;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, DESTROY_TIME);
    }
}
