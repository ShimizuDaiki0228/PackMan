using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PickupItemSpawner : MonoBehaviour
{
    /// <summary>
    /// �A�C�e�����X�|�[��������܂łɂ����鎞��
    /// </summary>
    [SerializeField]
    private float _spawnRate;

    /// <summary>
    /// �A�C�e���̃��X�g
    /// </summary>
    private List<GameObject> _pickupItemList = new List<GameObject>();

    /// <summary>
    /// �C���X�^���X�����ꂽ�A�C�e��
    /// </summary>
    private GameObject _instanceItemObject;

    private CancellationTokenSource _cancellationTokenSource;


    // Start is called before the first frame update
    public void Initialize()
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

            Destroy(_instanceItemObject);
            int num = Random.Range(0, _pickupItemList.Count);
            _instanceItemObject = Instantiate(_pickupItemList[num], transform.position, Quaternion.identity);
        }
    }

    private void ItemSet()
    {
        _pickupItemList.Clear();
        _pickupItemList = ItemManager.Instance.GetItemList();
    }

    private void OnDestroy()
    {
        // �V�[�����A�����[�h���ꂽ�Ƃ��ɃL�����Z���g�[�N�����L�����Z������
        _cancellationTokenSource.Cancel();
    }


    public void Reset()
    {
        Destroy(_instanceItemObject);
    }
}
