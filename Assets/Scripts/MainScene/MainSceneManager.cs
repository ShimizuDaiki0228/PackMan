using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManager : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings; // ループするためにモジュロ演算子を使用

            SceneManager.LoadScene(nextSceneIndex);

            if(GameManager.Instance != null)
            {
                GameManager.Instance.Reset();
            }
        }
    }
}
