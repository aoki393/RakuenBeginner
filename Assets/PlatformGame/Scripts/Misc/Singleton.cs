using UnityEngine;

namespace PlatformGame
{
    /// <summary>
    /// 单例模式基类，用于创建单例 MonoBehaviour 组件
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        protected static T m_instance;

        public static T instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindFirstObjectByType<T>();
                }

                return m_instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}


