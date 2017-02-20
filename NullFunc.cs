using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SharpFuncExt
{
	public static class NullFunc
	{
		public static TResult IfNull<T, TResult>(this T arg, Func<TResult> ifNull, Func<T, TResult> ifNotNull)
		{
			if (arg.IsNull())
				return ifNull();
			return ifNotNull(arg);
		}

		public static TResult IfNotNull<T, TResult>(this T arg, Func<T, TResult> ifNotNull, Func<TResult> ifNull)
		{
			if (arg.NotNull())
				return ifNotNull(arg);
			return ifNull();
		}
		public static TResult IfNotNull<T, TResult>(this T arg, Func<T, TResult> ifNotNull, TResult ifNull)
		{
			if (arg.NotNull())
				return ifNotNull(arg);
			return ifNull;
		}

		public static TResult IfNull<T, TResult>(this IEnumerable<T> arg, Func<TResult> ifNull, Func<IEnumerable<T>, TResult> ifNotNull)
		{
			if (arg.IsNull())
				return ifNull();
			return ifNotNull(arg);
		}

		public static TResult IfNotNull<T, TResult>(this IEnumerable<T> arg, Func<IEnumerable<T>, TResult> ifNotNull, Func<TResult> ifNull)
		{
			if (arg.NotNull())
				return ifNotNull(arg);
			return ifNull();
		}

		public static TResult IfNull<T, TResult>(this T arg, TResult ifNull, TResult ifNotNull)
		{
			if (arg.IsNull())
				return ifNull;
			return ifNotNull;
		}

		public static TResult IfNotNull<T, TResult>(this T arg, TResult ifNotNull, TResult ifNull)
		{
			if (arg.NotNull())
				return ifNotNull;
			return ifNull;
		}

		public static TResult NotNullOrDefault<T, TResult>(this T arg, Func<T, TResult> ifNotNull)
		{
			if (arg.NotNull())
				return ifNotNull(arg);
			return default(TResult);
		}

		public static T IfNull<T, TResult>(this T arg, Func<T, TResult> func)
		{
			if (arg.IsNull())
				func(arg);
			return arg;
		}

		public static T IfNotNull<T, TResult>(this T arg, Func<T, TResult> func)
		{
			if (arg.NotNull())
				func(arg);
			return arg;
		}

		public static TResult IfNotNullOrDefault<T, TResult>(this T arg, Func<T, TResult> func)
		{
			if (arg.NotNull())
				return func(arg);
			return default(TResult);
		}

		public static T HasNotNullArg<T>(this T arg, string argName) => arg.ThrowIfNull(() => new ArgumentNullException(argName));

		public static T ThrowIfNull<T, TException>(this T arg, Func<TException> func) where TException : Exception
		{
			if (arg.IsNull())
				throw func();
			return arg;
		}

		//public static bool IsNull<T>(this T arg) => EqualityComparer<T>.Default.Equals(arg, default(T)) || (arg is DateTime && arg.Equals(DateTime.MinValue));
		//public static bool NotNull<T>(this T arg) => !EqualityComparer<T>.Default.Equals(arg, default(T)) && !(arg is DateTime && arg.Equals(DateTime.MinValue));
		public static bool IsNull<T>(this T arg) => EqualityComparer<T>.Default.Equals(arg, default(T));
		public static bool NotNull<T>(this T arg) => !EqualityComparer<T>.Default.Equals(arg, default(T));

		public static bool IsNull<T>(this IEnumerable<T> arg) => arg == null || !arg.Any();
		public static bool NotNull<T>(this IEnumerable<T> arg) => arg != null && arg.Any();
		public static bool IsNull<T>(this T[] arg) => arg == null || !arg.Any();
		public static bool NotNull<T>(this T[] arg) => arg != null && arg.Any();

		public static bool IsNull(this object arg, Type type) => arg == null || type.GetTypeInfo().IsValueType && GetTypedNull(type).Equals(arg);

		private static Dictionary<Type, Delegate> lambdasMap = new Dictionary<Type, Delegate>();
		private static object GetTypedNull(Type type)
		{
			Delegate func;
			if (!lambdasMap.TryGetValue(type, out func))
			{
				var body = Expression.Default(type);
				var lambda = Expression.Lambda(body);
				func = lambda.Compile();
				lambdasMap[type] = func;
			}
			return func.DynamicInvoke();
		}
	}
}
