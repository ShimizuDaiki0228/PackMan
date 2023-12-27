using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PowerPellet : MonoBehaviour
{
    int _score = 10;

    private void Start()
    {
        GameManager.Instance.AddPellet();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PackMan")
        {
            GameManager.Instance.ReducePellet(_score);
            GameManager.Instance.OnFrightenSubject.OnNext(Unit.Default);
            Destroy(gameObject);
        }
    }
}
