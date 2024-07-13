using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            if ((object)instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).ToString());
                    instance = singletonObject.AddComponent<T>();
                    DontDestroyOnLoad(instance.gameObject);
                }
            }

            return instance;
        }
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this);
        }
    }
    
    private void OnDestroy() {
        if (instance == this) {
            instance = null;
        }
    }
}