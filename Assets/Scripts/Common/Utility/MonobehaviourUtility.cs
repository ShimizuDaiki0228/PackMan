using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UniRx;
using UnityEngine;


public class MonobehaviourUtility : MonoBehaviour
{
    public static MonobehaviourUtility Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void EffectCreate(GameObject effectPrefab, Vector3 position)
    {
        var effect = Instantiate(effectPrefab, position, Quaternion.identity);
        var mainModule = effectPrefab.GetComponent<ParticleSystem>().main;
        float lifeTime = mainModule.startLifetime.constant;

        Destroy(effect, lifeTime);
    }
}
