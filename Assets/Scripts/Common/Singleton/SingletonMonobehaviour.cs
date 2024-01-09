using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonobehaviour : MonoBehaviour
{
    /// <summary>
    /// MonoBehaviourを継承したSingletonクラス
    /// </summary>
    public abstract class SingletonMonoBehaviour<T>
                            : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        private static T _instance;

        /// <summary>
        /// シングルトンインスタンス取得
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        Debug.Log(typeof(T)
                            + "をアタッチしているGameObjectが存在しないため新しくシングルトンインスタンスを作成します");

                        //新しいGameObjectを作成し、インスタンスをアタッチ
                        var singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + "(Singleton)";


                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Awake
        /// </summary>
        protected virtual void Awake()
        {
            if (this != Instance)
            {
                Destroy(gameObject); //不要なGameObjectも破棄
            }
        }
    }
}
