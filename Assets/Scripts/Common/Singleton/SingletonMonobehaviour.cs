using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonobehaviour : MonoBehaviour
{
    /// <summary>
    /// MonoBehaviour���p������Singleton�N���X
    /// </summary>
    public abstract class SingletonMonoBehaviour<T>
                            : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// �C���X�^���X
        /// </summary>
        private static T _instance;

        /// <summary>
        /// �V���O���g���C���X�^���X�擾
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
                            + "���A�^�b�`���Ă���GameObject�����݂��Ȃ����ߐV�����V���O���g���C���X�^���X���쐬���܂�");

                        //�V����GameObject���쐬���A�C���X�^���X���A�^�b�`
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
                Destroy(gameObject); //�s�v��GameObject���j��
            }
        }
    }
}
