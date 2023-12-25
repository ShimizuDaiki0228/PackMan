using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    [SerializeField]
    private List<GameObject> _pickUpItemList = new List<GameObject>();
    public static List<GameObject> PickupItemList { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(this);
        }

        PickupItemList = _pickUpItemList;
    }

    public List<GameObject> GetItemList()
    {
        return PickupItemList;
    }
}
