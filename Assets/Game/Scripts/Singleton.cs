using UnityEngine;

public class Singleton<TSingleton> : MonoBehaviour where TSingleton : Singleton<TSingleton>
{
    static TSingleton _instance;
    public static TSingleton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TSingleton>();
            }
            return _instance;
        }
        private set
        {
            _instance = value;
        }
    }

}