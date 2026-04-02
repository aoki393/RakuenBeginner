using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace PLAYERTWO.PlatformerProject
{
	/// <summary>
	/// 泛型抽象类，代表某种实体(Entity)的状态机中的一个状态。
	/// T 是继承自 Entity<T> 的实体类型。
	/// </summary>
	public abstract class EntityState<T> where T : Entity<T>
	{
		public UnityEvent onEnter;
		public UnityEvent onExit;

		/// <summary>
		/// 记录实体进入该状态后经过的时间
		/// </summary>
		public float timeSinceEntered { get; protected set; }


		public void Enter(T entity)
		{
			// 重置计时
			timeSinceEntered = 0;
			// 触发外部注册的进入事件回调
			onEnter?.Invoke();
			// 调用子类自定义的进入逻辑
			OnEnter(entity);
		}


		public void Exit(T entity)
		{
			onExit?.Invoke();
			OnExit(entity);
		}


		public void Step(T entity)
		{
			OnStep(entity);
			timeSinceEntered += Time.deltaTime;
		}

		#region 抽象方法

		protected abstract void OnEnter(T entity);
		protected abstract void OnExit(T entity);
		protected abstract void OnStep(T entity);
		public abstract void OnContact(T entity, Collider other);

		#endregion

		#region 静态方法

		/// <summary>
		/// 静态方法，通过类型名称字符串创建对应的状态实例。
		/// 例如传入"PLAYERTWO.PlatformerProject.IdleState" 返回该类型的实例。
		/// </summary>
		/// <param name="typeName">状态类的完全限定名称。</param>
		/// <returns>对应的状态实例。</returns>
		public static EntityState<T> CreateFromString(string typeName)
		{
			return (EntityState<T>)System.Activator
				.CreateInstance(System.Type.GetType(typeName));
		}

		/// <summary>
		/// 静态方法，根据字符串数组批量创建状态实例列表。
		/// </summary>
		/// <param name="array">包含多个状态类名的字符串数组。</param>
		/// <returns>包含对应状态实例的列表。</returns>
		public static List<EntityState<T>> CreateListFromStringArray(string[] array)
		{
			var list = new List<EntityState<T>>();

			foreach (var typeName in array)
			{
				list.Add(CreateFromString(typeName));
			}

			return list;
		}
		#endregion
	}
}
