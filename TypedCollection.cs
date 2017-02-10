using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpFuncExt
{
	public class TypedCollection
	{
		private readonly IDictionary<Type, object> _types = new Dictionary<Type, object>();

		public TypedCollection Add<T>(T value)
		{
			_types.Add(typeof(T), value);
			return this;
		}
		public T Get<T>() => (T)_types[typeof(T)];
	}
}
