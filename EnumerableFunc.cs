using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpFuncExt
{
	public static class EnumerableFunc
	{
		public static IEnumerable<T> NullToEmpty<T>(this IEnumerable<T> arg)
		{
			return arg ?? new T[0];
		}
		public static TValue Add<TValue>(this TValue value, ref TValue[] list)
		{
			list = (list ?? new TValue[0]).Union(new[] { value }).ToArray();
			return value;
		}

		public static TValue Add<TValue>(this TValue value, IList<TValue> list)
		{
			list.Add(value);
			return value;
		}

		public static TValue Add<TValue>(this TValue value, ref IEnumerable<TValue> list)
		{
			if (list is ICollection<TValue> && !list.GetType().IsArray)
				((ICollection<TValue>)list).Add(value);
			else
				list = (list ?? new TValue[0]).Union(new [] { value });
			return value;
		}
		public static IEnumerable<TValue> Add<TValue>(this IEnumerable<TValue> list, TValue value)
		{
			if (list is ICollection<TValue> && !list.GetType().IsArray)
				((ICollection<TValue>)list).Add(value);
			else
				return (list ?? new TValue[0]).Union(new[] { value });
			return list;
		}

		public static TValue Remove<TValue>(this TValue value, ref TValue[] list)
		{
			list = (list ?? new TValue[0]).Except(new[] { value }).ToArray();
			return value;
		}

		public static TValue Remove<TValue>(this TValue value, IList<TValue> list)
		{
			list.Remove(value);
			return value;
		}

		public static TValue Remove<TValue>(this TValue value, ref IEnumerable<TValue> list)
		{
			if (list is ICollection<TValue> && !list.GetType().IsArray)
				((ICollection<TValue>)list).Remove(value);
			else
				list = (list ?? new TValue[0]).Except(new[] { value });
			return value;
		}
		public static IEnumerable<TValue> Remove<TValue>(this IEnumerable<TValue> list, TValue value)
		{
			if (list is ICollection<TValue> && !list.GetType().IsArray)
				((ICollection<TValue>)list).Remove(value);
			else
				return (list ?? new TValue[0]).Except(new[] { value });
			return list;
		}

		//public static IEnumerable<TResult> Each<T, TResult>(this IEnumerable<T> arg, Func<T, TResult> func)
		//{
		//	var result = new TResult[arg.Count()];
		//	var i = 0;
		//	foreach (var v in arg)
		//		result[i++] = func(v);
		//	return result;
		//}

		public static IEnumerable<T> Each<T, TResult>(this IEnumerable<T> arg, Func<T, TResult> func)
		{
			foreach (var v in arg)
				func(v);
			return arg;
		}
		public static IEnumerable<T> Each<T>(this IEnumerable<T> arg, Action<T> func)
		{
			foreach (var v in arg)
				func(v);
			return arg;
		}

		public static bool In<T>(this T arg, IEnumerable<T> array) => arg.IfNotNull(array.Contains, ()=>false);
		public static bool In<T>(this IEnumerable<T> array, T arg) => arg.IfNotNull(array.Contains, ()=>false);

		public static TResult IfIn<T, TResult, TArray>(this T arg, IEnumerable<TArray> array, TArray element, Func<T, TArray, IEnumerable<TArray>, TResult> func, Func<T, TArray, IEnumerable<TArray>, TResult> ifNot)
		{
			return array.NotNull() && array.Contains(element) ? func(arg, element, array) : ifNot(arg, element, array);
		}
		public static TResult IfIn<T, TResult, TArray>(this T arg, IEnumerable<TArray> array, Func<T, TArray, bool> check, Func<T, TArray, IEnumerable<TArray>, TResult> func, Func<T, IEnumerable<TArray>, TResult> ifNot)
		{
			if (array.IsNull())
				return ifNot(arg, array);
			bool hasNext;
			var enumerator = array.GetEnumerator();
			while ((hasNext = enumerator.MoveNext()) && !check(arg, enumerator.Current)) ;
			if (hasNext)
				return func(arg, enumerator.Current, array);
			return ifNot(arg, array);
		}
		public static TResult IfIn<TResult, TArray>(this IEnumerable<TArray> array, TArray element, Func<TArray, IEnumerable<TArray>, TResult> func, Func<TArray, IEnumerable<TArray>, TResult> ifNotContains)
		{
			return array.NotNull() && array.Contains(element) ? func(element, array) : ifNotContains(element, array);
		}
		public static TResult IfIn<TResult, TArray>(this TArray element, IEnumerable<TArray> array, Func<TArray, IEnumerable<TArray>, TResult> func, Func<TArray, IEnumerable<TArray>, TResult> ifNotContains)
		{
			return array.NotNull() && array.Contains(element) ? func(element, array) : ifNotContains(element, array);
		}

		#region Fluent-version
		public static T IfIn<T, TArray, TResult>(this T arg, IEnumerable<TArray> array, TArray element, Func<T, TArray, IEnumerable<TArray>, TResult> func)
		{
			if (array.NotNull() && array.Contains(element))
				func(arg, element, array);
			return arg;
		}
		public static T IfNotIn<T, TArray, TResult>(this T arg, IEnumerable<TArray> array, TArray element, Func<T, TArray, IEnumerable<TArray>, TResult> func)
		{
			if (array.IsNull() || !array.Contains(element))
				func(arg, element, array);
			return arg;
		}

		public static IEnumerable<TArray> IfIn<TArray, TResult>(this IEnumerable<TArray> array, TArray element, Func<TArray, IEnumerable<TArray>, TResult> func)
		{
			if (array.NotNull() && array.Contains(element))
				func(element, array);
			return array;
		}
		public static IEnumerable<TArray> IfNotIn<TArray, TResult>(this IEnumerable<TArray> array, TArray element, Func<TArray, IEnumerable<TArray>, TResult> func)
		{
			if (array.IsNull() || !array.Contains(element))
				func(element, array);
			return array;
		}

		public static TArray IfIn<TArray, TResult>(this TArray element, IEnumerable<TArray> array, Func<TArray, IEnumerable<TArray>, TResult> func)
		{
			if(array.NotNull() && array.Contains(element))
				func(element, array);
			return element;
		}
		public static TArray IfNotIn<TArray, TResult>(this TArray element, IEnumerable<TArray> array, Func<TArray, IEnumerable<TArray>, TResult> func)
		{
			if (array.IsNull() || !array.Contains(element))
				func(element, array);
			return element;
		}
		#endregion

		public static bool SeqEquals<T>(this IEnumerable<T> arg, IEnumerable<T> value)
		{
			if (arg == null && value == null || ReferenceEquals(arg, value))
				return true;
			if (arg == null && !value.Any() || value == null && !arg.Any())
				return true;
			if (arg == null || value == null)
				return false;
			var valueEnumenator = value.GetEnumerator();
			foreach (var a in arg)
				if (!valueEnumenator.MoveNext() || !a.Equals(valueEnumenator.Current))
					return false;
			return !valueEnumenator.MoveNext();
		}

		public static TypedCollection AddTyped<T>(this T value)
		{
			var result = new TypedCollection();
			result.Add(value);
			return result;
		}

	}
}

