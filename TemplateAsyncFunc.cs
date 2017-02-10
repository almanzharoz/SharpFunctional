using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SharpFuncExt
{
	public static class TemplateAsyncFunc
	{
		public static async Task<TResult> If<T, TResult>(this T arg, Func<T, bool> check, Func<T, Task<TResult>> ifTrue,
				Func<T, Task<TResult>> ifFalse)
			=> await (check(arg) ? ifTrue(arg) : ifFalse(arg));

		public static async Task<TResult> If<T, TResult>(this Task<T> arg, Func<T, bool> check,
			Func<T, Task<TResult>> ifTrue,
			Func<T, Task<TResult>> ifFalse)
		{
			var a = await arg;
			return await (check(a) ? ifTrue(a) : ifFalse(a));
		}

		public static async Task<TResult> If<T, TResult>(this T arg, Func<T, Task<bool>> check,
				Func<T, Task<TResult>> ifTrue,
				Func<T, Task<TResult>> ifFalse)
			=> await (await check(arg) ? ifTrue(arg) : ifFalse(arg));

		public static async Task<TResult> If<T, TResult>(this Task<T> arg, Func<T, Task<bool>> check,
			Func<T, Task<TResult>> ifTrue,
			Func<T, Task<TResult>> ifFalse)
		{
			var a = await arg;
			return await (await check(a) ? ifTrue(a) : ifFalse(a));
		}

		public static async Task<TResult> If<T, TResult>(this Task<T> arg, Func<T, bool> check,
			Func<T, TResult> ifTrue,
			Func<T, TResult> ifFalse)
		{
			var a = await arg;
			return (check(a) ? ifTrue(a) : ifFalse(a));
		}

		public static async Task<TResult> If<T, TResult>(this T arg, Func<T, Task<bool>> check,
				Func<T, TResult> ifTrue,
				Func<T, TResult> ifFalse)
			=> (await check(arg) ? ifTrue(arg) : ifFalse(arg));

		public static async Task<TResult> If<T, TResult>(this Task<T> arg, Func<T, Task<bool>> check,
			Func<T, TResult> ifTrue,
			Func<T, TResult> ifFalse)
		{
			var a = await arg;
			return (await check(a) ? ifTrue(a) : ifFalse(a));
		}

		public static async Task<T> If<T, TResult>(this T arg, Func<T, bool> check, Func<T, Task<TResult>> ifTrue)
		{
			if (check(arg))
				await ifTrue(arg);
			return arg;
		}

		public static async Task<T> ThrowIf<T, TException>(this Task<T> arg, Func<T, bool> check, Func<T, TException> func) where TException : Exception
		{
			var a = await arg;
			if (check(a))
				throw func(a);
			return a;
		}

		public static async Task<T> Stopwatch<T>(this Task<T> arg, Stopwatch sw, Func<T, T> func)
		{
			var a = await arg;
			sw.Start();
			func(a);
			sw.Stop();
			return a;
		}

		public static async Task<TResult> Stopwatch<T, TResult>(this Task<T> arg, Stopwatch sw, Func<T, TResult> func)
		{
			var a = await arg;
			sw.Start();
			var r = func(a);
			sw.Stop();
			return r;
		}

		public static async Task<TResult> Stopwatch<T, TResult>(this Task<T> arg, Stopwatch sw, Func<T, Task<TResult>> func)
		{
			var a = await arg;
			sw.Start();
			var r = await func(a);
			sw.Stop();
			return r;
		}

		public static async Task<TResult> Stopwatch<T, TResult>(this T arg, Stopwatch sw, Func<T, Task<TResult>> func)
		{
			sw.Start();
			var r = await func(arg);
			sw.Stop();
			return r;
		}
	}
}
