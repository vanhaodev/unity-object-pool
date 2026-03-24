using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.vanhaodev.objectpool
{
	public class ObjectPool<T>
	{
		#region Private properties

		private readonly Stack<T> _stack;
		private readonly Func<T> _factory;
		private readonly Action<T> _onGet;
		private readonly Action<T> _onRelease;
		private readonly Action<T> _onDestroy;
		private HashSet<T> _actives = new HashSet<T>();

		#endregion

		#region Public properties

		public int CountAll { get; private set; }
		public int ActiveCount => _actives.Count;
		public int InactiveCount => _stack.Count;

		#endregion

		#region Public methods

		public ObjectPool(
			Func<T> factory,
			int initialSize = 0,
			Action<T> onGet = null,
			Action<T> onRelease = null,
			Action<T> onDestroy = null)
		{
			_factory = factory;
			_onGet = onGet;
			_onRelease = onRelease;
			_onDestroy = onDestroy;
			_stack = new Stack<T>(initialSize);
			Warm(initialSize);
		}

		public void Warm(int size)
		{
			for (int i = 0; i < size; i++)
			{
				var item = Create();
				_onRelease?.Invoke(item);
				_stack.Push(item);
			}
		}

		public T Get()
		{
			T item = _stack.Count > 0 ? _stack.Pop() : Create();
			_actives.Add(item);
			_onGet?.Invoke(item);
			return item;
		}

		public void Release(T item)
		{
			if (_actives.Remove(item))
			{
				_onRelease?.Invoke(item);
				_stack.Push(item);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="includeActive">Destroy running/active object</param>
		public void Clear(bool includeActive = false)
		{
			// clear idle
			while (_stack.Count > 0)
			{
				var item = _stack.Pop();
				_onDestroy?.Invoke(item);
			}

			// clear active
			if (includeActive)
			{
				foreach (var item in _actives)
				{
					_onDestroy?.Invoke(item);
				}

				_actives.Clear();
			}

			CountAll = 0;
		}

		#endregion

		#region Private methods

		private T Create()
		{
			var item = _factory();
			CountAll++;
			return item;
		}

		#endregion
	}
}