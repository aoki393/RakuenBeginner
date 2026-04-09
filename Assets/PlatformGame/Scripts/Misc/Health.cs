using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public int initial = 3;
    public int max = 3;
    protected int m_currentHealth;
	protected float m_lastDamageTime; // 上一次受到伤害的时间（用于计算冷却）
    public float coolDown = 1f; // 伤害冷却时间（单位：秒）
    public UnityEvent onChange;
    public UnityEvent onDamage;
    public virtual bool IsEmpty => Current == 0;
    public int Current
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
    public virtual bool Recovering => Time.time < m_lastDamageTime + coolDown;
    void Awake() => Current = initial;
    public virtual void Reset() => Current = initial; // 重置关卡时对生命值重置

    public virtual void Damage(int amount)
	{
        if(amount <= 0)
            return;
            
		if (!Recovering) // 不在冷却时间内
		{
			Current -= amount;
			m_lastDamageTime = Time.time;
			
			onDamage?.Invoke(); // 通知受伤事件（比如闪红屏幕、播放受伤音效）
		}
	}
}
