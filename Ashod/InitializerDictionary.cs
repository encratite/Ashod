using System;
using System.Collections.Generic;

namespace Ashod
{
	public class InitializerDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		public TValue Get(TKey key, Func<TValue> initializer)
		{
			if (initializer == null)
				throw new ArgumentException("No initializer has been set.");
			TValue value;
			if (!TryGetValue(key, out value))
			{
				value = initializer();
				this[key] = value;
			}
			return value;
		}
	}
}
