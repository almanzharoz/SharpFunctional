using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SharpFuncExt
{
	public static class ObjectFunc
	{
		public static Stopwatch sw_set = new Stopwatch();
		internal static ConcurrentDictionary<string, Delegate> _lamdas = new ConcurrentDictionary<string, Delegate>();

		internal static Action<T, TValue> GetOrAddSetLambda<T, TValue>(Expression<Func<T, TValue>> expression)
		{
			Delegate c = null;
			Expression targetExpression = expression.Body is UnaryExpression
				? ((UnaryExpression)expression.Body).Operand
				: expression.Body;
			var s = targetExpression.ToString();
			s = s.Substring(s.IndexOf(".")+1);
			if (!_lamdas.TryGetValue(String.Concat(typeof(T).Name, ".", s), out c))
			{
				ParameterExpression valueParameterExpression = Expression.Parameter(typeof(TValue));

				//var newValue = Expression.Parameter(expression.Body.Type);
				var assign = Expression.Lambda<Action<T, TValue>>
				(
					Expression.Assign(targetExpression, valueParameterExpression),
					expression.Parameters.Single(), valueParameterExpression
				);

				var c1 = assign.Compile();
				_lamdas.TryAdd(String.Concat(typeof(T).Name, ".", s), c1);
				return c1;
			}
			return (Action<T, TValue>)c;
		}
		public static T Set<T, TValue>(this T arg, Expression<Func<T, TValue>> expression, TValue value)
		{
			GetOrAddSetLambda(expression).Invoke(arg, value);
			return arg;
		}

		public static T Set<T, TValue>(this T arg, Expression<Func<T, TValue>> expression, Func<T, TValue> func)
		{
			GetOrAddSetLambda(expression).Invoke(arg, func(arg));
			return arg;
		}

		public static TResult Convert<T, TResult>(this T arg, Func<T, TResult> func) => func(arg);
		public static TResult Convert<T, TResult>(this T arg, TResult value) => value;

		public static Tuple<TResult1, TResult2> Convert<T, TResult1, TResult2>(this T arg, Func<T, TResult1> func1, Func<T, TResult2> func2)
			=> Tuple.Create(func1(arg), func2(arg));

		public static Tuple<TResult1, TResult2> Convert<T, TResult1, TResult2>(this T arg, Func<T, TResult1> func1,
			Func<T, TResult1, TResult2> func2)
		{
			var value1 = func1(arg);
			return Tuple.Create(value1, func2(arg, value1));
		}

		public static Tuple<TResult1, TResult2> Convert<T, TResult1, TResult2>(this T arg, TResult1 value1, TResult2 value2)
			=> Tuple.Create(value1, value2);

		public static Tuple<TResult1, TResult2, TResult3> Convert<T, TResult1, TResult2, TResult3>(this T arg, Func<T, TResult1> func1,
			Func<T, TResult2> func2, Func<T, TResult3> func3)
			=> Tuple.Create(func1(arg), func2(arg), func3(arg));
		public static Tuple<TResult1, TResult2, TResult3> Convert<T, TResult1, TResult2, TResult3>(this T arg, Func<T, TResult1> func1,
			Func<T, TResult1, TResult2> func2, Func<T, TResult1, TResult2, TResult3> func3)
		{
			var value1 = func1(arg);
			var value2 = func2(arg, value1);
			return Tuple.Create(value1, value2, func3(arg, value1, value2));
		}
		public static Tuple<TResult1, TResult2, TResult3> Convert<T, TResult1, TResult2, TResult3>(this T arg, TResult1 value1, TResult2 value2, TResult3 value3)
			=> Tuple.Create(value1, value2, value3);

		public static Tuple<TResult1, TResult2, TResult3, TResult4> Convert<T, TResult1, TResult2, TResult3, TResult4>(this T arg, Func<T, TResult1> func1,
			Func<T, TResult2> func2, Func<T, TResult3> func3, Func<T, TResult4> func4)
			=> Tuple.Create(func1(arg), func2(arg), func3(arg), func4(arg));
		public static Tuple<TResult1, TResult2, TResult3, TResult4> Convert<T, TResult1, TResult2, TResult3, TResult4>(this T arg, Func<T, TResult1> func1,
			Func<T, TResult1, TResult2> func2, Func<T, TResult1, TResult2, TResult3> func3, Func<T, TResult1, TResult2, TResult3, TResult4> func4)
		{
			var value1 = func1(arg);
			var value2 = func2(arg, value1);
			var value3 = func3(arg, value1, value2);
			return Tuple.Create(value1, value2, value3, func4(arg, value1, value2, value3));
		}
		public static Tuple<TResult1, TResult2, TResult3, TResult4> Convert<T, TResult1, TResult2, TResult3, TResult4>(this T arg, TResult1 value1, TResult2 value2, TResult3 value3, TResult4 value4)
			=> Tuple.Create(value1, value2, value3, value4);

		public static T Deep<T, TResult>(this T arg, Func<T, TResult> func)
		{
			func(arg);
			return arg;
		}
		public static T Deep<T>(this T arg, Action<T> func)
		{
			func(arg);
			return arg;
		}

		public static TResult Is<TArg, T, TResult>(this TArg arg, Func<T, TResult> func, Func<TArg, TResult> funcNot) where T : TArg
		{
			if (arg is T)
				return func((T)arg);
			return funcNot(arg);
		}
		public static TArg Is<TArg, T, TResult>(this TArg arg, Func<T, TResult> func) where T : TArg
		{
			if (arg is T)
				func((T)arg);
			return arg;
		}
		public static TArg Is<TArg, T>(this TArg arg, Action<T> func) where T : TArg
		{
			if (arg is T)
				func((T)arg);
			return arg;
		}

		public static TResult As<T, TResult>(this T arg) where TResult : class, T => arg as TResult;

		public static Tuple<T, T1> Extend<T, T1>(this T arg, Func<T, T1> func) => Tuple.Create(arg, func(arg));
		public static Tuple<T1, T2, T3> Extend<T1, T2, T3>(this Tuple<T1, T2> arg, Func<Tuple<T1, T2>, T3> func) => Tuple.Create(arg.Item1, arg.Item2, func(arg));
		public static Tuple<T1, T2, T3, T4> Extend<T1, T2, T3, T4>(this Tuple<T1, T2, T3> arg, Func<Tuple<T1, T2, T3>, T4> func) => Tuple.Create(arg.Item1, arg.Item2, arg.Item3, func(arg));
		public static Tuple<T1, T2, T3, T4, T5> Extend<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4> arg, Func<Tuple<T1, T2, T3, T4>, T5> func) => Tuple.Create(arg.Item1, arg.Item2, arg.Item3, arg.Item4, func(arg));

		public static Tuple<T1, T2, T3> Extend<T1, T2, T3>(this Tuple<T1, T2> arg, Func<T1, T2, T3> func) => Tuple.Create(arg.Item1, arg.Item2, func(arg.Item1, arg.Item2));

		public static Tuple<T1, T2, T3, T4> Extend<T1, T2, T3, T4>(this Tuple<T1, T2, T3> arg, Func<T1, T2, T3, T4> func) => Tuple.Create(arg.Item1, arg.Item2, arg.Item3, func(arg.Item1, arg.Item2, arg.Item3));
		public static Tuple<T1, T2, T3, T4> Extend<T1, T2, T3, T4>(this Tuple<T1, T2, T3> arg, Func<T1, T2, T4> func) => Tuple.Create(arg.Item1, arg.Item2, arg.Item3, func(arg.Item1, arg.Item2));
		public static Tuple<T1, T2, T3, T4> Extend<T1, T2, T3, T4>(this Tuple<T1, T2, T3> arg, Func<T1, T3, T4> func) => Tuple.Create(arg.Item1, arg.Item2, arg.Item3, func(arg.Item1, arg.Item3));
		public static Tuple<T1, T2, T3, T4> Extend<T1, T2, T3, T4>(this Tuple<T1, T2, T3> arg, Func<T2, T3, T4> func) => Tuple.Create(arg.Item1, arg.Item2, arg.Item3, func(arg.Item2, arg.Item3));

		public static Tuple<T1, T2, T3, T4, T5> Extend<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4> arg, Func<T1, T2, T3, T4, T5> func) => Tuple.Create(arg.Item1, arg.Item2, arg.Item3, arg.Item4, func(arg.Item1, arg.Item2, arg.Item3, arg.Item4));

		public static Tuple<T, T1> Extend<T, T1>(this T arg, T1 value) => Tuple.Create(arg, value);
		public static Tuple<T1, T2, T3> Extend<T1, T2, T3>(this Tuple<T1, T2> arg, T3 value) => Tuple.Create(arg.Item1, arg.Item2, value);
		public static Tuple<T1, T2, T3, T4> Extend<T1, T2, T3, T4>(this Tuple<T1, T2, T3> arg, T4 value) => Tuple.Create(arg.Item1, arg.Item2, arg.Item3, value);
		public static Tuple<T1, T2, T3, T4, T5> Extend<T1, T2, T3, T4, T5>(this Tuple<T1, T2, T3, T4> arg, T5 value) => Tuple.Create(arg.Item1, arg.Item2, arg.Item3, arg.Item4, value);
	}
}
