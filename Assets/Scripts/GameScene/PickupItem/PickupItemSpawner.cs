using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickupItemSpawner : MonoBehaviour
{
    /// <summary>
    /// アイテムをスポーンさせるまでにかかる時間
    /// </summary>
    [SerializeField]
    private float _spawnRate;

    /// <summary>
    /// アイテムのリスト
    /// </summary>
    private List<GameObject> _pickupItemList = new List<GameObject>();

    private CancellationTokenSource _cancellationTokenSource;


    // Start is called before the first frame update
    void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        SpawnItem(_cancellationTokenSource.Token).Forget();
        ItemSet();
        
    }

    private async UniTask SpawnItem(CancellationToken token)
    {
        while(true)
        {
            await UniTask.WaitForSeconds(_spawnRate);

            if (token.IsCancellationRequested)
                break;

            if (_pickupItemList == null || _pickupItemList.Count == 0)
                return;
            

            int num = Random.Range(0, _pickupItemList.Count);
            Instantiate(_pickupItemList[num], transform.position, Quaternion.identity);
        }
    }

    private void ItemSet()
    {
        _pickupItemList.Clear();
        _pickupItemList = ItemManager.Instance.GetItemList();
    }

    private void OnDestroy()
    {
        // シーンがアンロードされたときにキャンセルトークンをキャンセルする
        _cancellationTokenSource.Cancel();
    }
}
