using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int initial = 3;
    public int max = 3;
    protected int m_currentHealth;
    public UnityEvent onChange;
    public int current
    {
        get { return m_currentHealth; }

        protected set
        {
            var last = m_currentHealth;

            if (value != last) // 只有在值发生改变时才更新
            {
                // 确保生命值不超过最大值，也不小于 0
                m_currentHealth = Mathf.Clamp(value, 0, max);

                // 通知所有订阅了 onChange 的事件（比如 UI 刷新）
                onChange?.Invoke();
            }
        }
    }
    
    void Awake() => current = initial;
}
