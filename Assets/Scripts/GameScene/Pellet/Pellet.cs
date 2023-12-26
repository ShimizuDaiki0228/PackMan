using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{

    int _score = 3;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.AddPellet();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PackMan")
        {
            GameManager.Instance.ReducePellet(_score);
            Destroy(gameObject);
        }
    }
}
