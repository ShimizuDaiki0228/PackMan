using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostControllerMain : MonoBehaviour
{
    /// <summary>
    /// 通常時のゴーストの見た目
    /// </summary>
    [SerializeField]
    private GameObject _normalGhostSkin;

    private bool _isFrighten;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void ManualUpdate()
    {
        if(!_isFrighten)
            transform.position += new Vector3(-4, 0, 0) * Time.deltaTime;
        else
            transform.position += new Vector3(2, 0, 0) * Time.deltaTime;
    }

    /// <summary>
    /// 見た目を怯え状態にする
    /// </summary>
    public void ChangeFrighterMode()
    {
        _normalGhostSkin.SetActive(false);
        _isFrighten = true;
        transform.localEulerAngles = new Vector3(0, 90, 0);
    }
}
