using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SharpFuncExt
{
	public static class ObjectAsyncFunc
	{
		public static async Task<T> Set<T, TValue>(this Task<T> arg, Expression<Func<T, TValue>> expression, TValue value)
		{
			var a = await arg;
			ObjectFunc.GetOrAddSetLambda(expression).Invoke(a, value);
			return a;
		}

		public static async Task<T> Set<T, TValue>(this Task<T> arg, Expression<Func<T, TValue>> expression, Task<TValue> value)
		{
			var a = await arg;
			ObjectFunc.GetOrAddSetLambda(expression).Invoke(a, await value);
			return a;
		}

		public static async Task<T> Set<T, TValue>(this T arg, Expression<Func<T, TValue>> expression, Task<TValue> value)
		{
			ObjectFunc.GetOrAddSetLambda(expression).Invoke(arg, await value);
			return arg;
		}

		public static async Task<T> Set<T, TValue>(this Task<T> arg, Expression<Func<T, TValue>> expression, Func<T, TValue> value)
		{
			var a = await arg;
			ObjectFunc.GetOrAddSetLambda(expression).Invoke(a, value(a));
			return a;
		}
		public static async Task<T> Deep<T, TResult>(this Task<T> arg, Func<T, TResult> func)
		{
			var a = await arg;
			func(a);
			return a;
		}
		public static async Task<T> Deep<T, TResult>(this Task<T> arg, Func<T, Task<TResult>> func)
		{
			var a = await arg;
			await func(a);
			return a;
		}
		public static async Task<T> Deep<T, TResult>(this T arg, Func<T, Task<TResult>> func)
		{
			await func(arg);
			return arg;
		}

		public static async Task<TResult> Convert<T, TResult>(this Task<T> arg, Func<T, TResult> func)
			=> func(await arg);
		public static Task<TResult> Convert<T, TResult>(this T arg, Func<T, Task<TResult>> func)
			=> func(arg);
		public static Task<TResult> Convert<T, TResult>(this Task<T> arg, Func<Task<T>, Task<TResult>> func)
			=> func(arg);

		public static async Task<Tuple<TResult1, TResult2>> Convert<T, TResult1, TResult2>(this Task<T> arg,
			Func<T, TResult1> func1, Func<T, TResult2> func2)
		{
			var a = await arg;
			return Tuple.Create(func1(a), func2(a));
		}

		public static async Task<Tuple<TResult1, TResult2>> Convert<T, TResult1, TResult2>(this T arg, Func<T, Task<TResult1>> func1, Func<T, Task<TResult2>> func2)
			=> Tuple.Create(await func1(arg), await func2(arg));

		public static async Task<Tuple<TResult1, TResult2>> Convert<T, TResult1, TResult2>(this Task<T> arg,
			Func<Task<T>, Task<TResult1>> func1, Func<Task<T>, Task<TResult2>> func2)
			=> Tuple.Create(await func1(arg), await func2(arg));
		public static async Task<Tuple<TResult1, TResult2>> Convert<T, TResult1, TResult2>(this Task<T> arg,
			Func<T, TResult1> func1, Func<T, TResult1, TResult2> func2)
		{
			var a = await arg;
			var v1 = func1(a);
			return Tuple.Create(v1, func2(a, v1));
		}

		public static async Task<Tuple<TResult1, TResult2>> Convert<T, TResult1, TResult2>(this T arg,
			Func<T, Task<TResult1>> func1, Func<T, TResult1, Task<TResult2>> func2)
		{
			var v = await func1(arg);
			return Tuple.Create(v, await func2(arg, v));
		}


		public static async Task<Tuple<T, T1>> Extend<T, T1>(this Task<T> arg, Func<T, T1> func)
		{
			var a = await arg;
			return Tuple.Create(a, func(a));
		}
		public static async Task<Tuple<T, T1>> Extend<T, T1>(this Task<T> arg, Func<T, Task<T1>> func)
		{
			var a = await arg;
			return Tuple.Create(a, await func(a));
		}
		public static async Task<Tuple<T, T1>> Extend<T, T1>(this Task<T> arg, Task<T1> value)
		{
			var a = await arg;
			return Tuple.Create(a, await value);
		}
		public static async Task<Tuple<T, T1>> Extend<T, T1>(this T arg, Task<T1> value)
			=> Tuple.Create(arg, await value);
		public static async Task<Tuple<T, T1>> Extend<T, T1>(this T arg, Func<T, Task<T1>> func)
			=> Tuple.Create(arg, await func(arg));

		public static async Task<Tuple<T, T1, T2>> Extend<T, T1, T2>(this Task<Tuple<T, T1>> arg, Func<T, T1, T2> func)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, func(a.Item1, a.Item2));
		}
		public static async Task<Tuple<T, T1, T2>> Extend<T, T1, T2>(this Task<Tuple<T, T1>> arg, Func<Tuple<T, T1>, T2> func)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, func(a));
		}
		public static async Task<Tuple<T, T1, T2>> Extend<T, T1, T2>(this Task<Tuple<T, T1>> arg, Func<T, T1, Task<T2>> func)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, await func(a.Item1, a.Item2));
		}
		public static async Task<Tuple<T, T1, T2>> Extend<T, T1, T2>(this Task<Tuple<T, T1>> arg, Task<T2> value)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, await value);
		}
		public static async Task<Tuple<T, T1, T2>> Extend<T, T1, T2>(this Tuple<T, T1> arg, Task<T2> value)
			=> Tuple.Create(arg.Item1, arg.Item2, await value);
		public static async Task<Tuple<T, T1, T2>> Extend<T, T1, T2>(this Tuple<T, T1> arg, Func<T, T1, Task<T2>> func)
			=> Tuple.Create(arg.Item1, arg.Item2, await func(arg.Item1, arg.Item2));

		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Task<Tuple<T, T1, T2>> arg, Func<T, T1, T2, T3> func)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, a.Item3, func(a.Item1, a.Item2, a.Item3));
		}
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Task<Tuple<T, T1, T2>> arg, Func<Tuple<T, T1, T2>, T3> func)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, a.Item3, func(a));
		}
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Task<Tuple<T, T1, T2>> arg, Func<T, T1, T3> func)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, a.Item3, func(a.Item1, a.Item2));
		}
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Task<Tuple<T, T1, T2>> arg, Func<T1, T2, T3> func)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, a.Item3, func(a.Item2, a.Item3));
		}
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Task<Tuple<T, T1, T2>> arg, Func<T, T2, T3> func)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, a.Item3, func(a.Item1, a.Item3));
		}
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Task<Tuple<T, T1, T2>> arg, Func<T, T3> func)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, a.Item3, func(a.Item1));
		}
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Task<Tuple<T, T1, T2>> arg, Func<T, T1, T2, Task<T3>> func)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, a.Item3, await func(a.Item1, a.Item2, a.Item3));
		}
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Task<Tuple<T, T1, T2>> arg, Func<T, T1, Task<T3>> func)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, a.Item3, await func(a.Item1, a.Item2));
		}
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Task<Tuple<T, T1, T2>> arg, Func<T, T2, Task<T3>> func)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, a.Item3, await func(a.Item1, a.Item3));
		}
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Task<Tuple<T, T1, T2>> arg, Func<T1, T2, Task<T3>> func)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, a.Item3, await func(a.Item2, a.Item3));
		}
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Task<Tuple<T, T1, T2>> arg, Task<T3> value)
		{
			var a = await arg;
			return Tuple.Create(a.Item1, a.Item2, a.Item3, await value);
		}
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Tuple<T, T1, T2> arg, Task<T3> value)
			=> Tuple.Create(arg.Item1, arg.Item2, arg.Item3, await value);
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Tuple<T, T1, T2> arg, Func<T, T1, T2, Task<T3>> func)
			=> Tuple.Create(arg.Item1, arg.Item2, arg.Item3, await func(arg.Item1, arg.Item2, arg.Item3));
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Tuple<T, T1, T2> arg, Func<T, T1, Task<T3>> func)
			=> Tuple.Create(arg.Item1, arg.Item2, arg.Item3, await func(arg.Item1, arg.Item2));
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Tuple<T, T1, T2> arg, Func<T, T2, Task<T3>> func)
			=> Tuple.Create(arg.Item1, arg.Item2, arg.Item3, await func(arg.Item1, arg.Item3));
		public static async Task<Tuple<T, T1, T2, T3>> Extend<T, T1, T2, T3>(this Tuple<T, T1, T2> arg, Func<T1, T2, Task<T3>> func)
			=> Tuple.Create(arg.Item1, arg.Item2, arg.Item3, await func(arg.Item2, arg.Item3));
	}
}
