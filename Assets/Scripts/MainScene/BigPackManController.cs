using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPackManController : MonoBehaviour
{
    /// <summary>
    /// �蓮Update
    /// </summary>
    public void ManualUpdate()
    {
        gameObject.transform.position += new Vector3(7, 0, 0) * Time.deltaTime;
    }
}
