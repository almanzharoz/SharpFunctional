using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SharpFuncExt
{
	public static class TemplateFunc
	{
		public static TResult If<T, TResult>(this T arg, Func<T, bool> check, Func<T, TResult> ifTrue, Func<T, TResult> ifFalse)
		{
			if (check(arg))
				return ifTrue(arg);
			return ifFalse(arg);
		}

		public static T If<T>(this T arg, Func<T, bool> check, Action<T> ifTrue)
		{
			if (check(arg))
				ifTrue(arg);
			return arg;
		}

		public static T If<T, TResult>(this T arg, Func<T, bool> check, Func<T, TResult> ifTrue)
		{
			if (check(arg))
				ifTrue(arg);
			return arg;
		}

		public static Tuple<T1, T2> If<T1, T2, TResult>(this Tuple<T1, T2> arg, Func<T1, T2, bool> check, Func<T1, T2, TResult> ifTrue)
		{
			if (check(arg.Item1, arg.Item2))
				ifTrue(arg.Item1, arg.Item2);
			return arg;
		}
		public static Tuple<T1, T2, T3> If<T1, T2, T3, TResult>(this Tuple<T1, T2, T3> arg, Func<T1, T2, T3, bool> check, Func<T1, T2, T3, TResult> ifTrue)
		{
			if (check(arg.Item1, arg.Item2, arg.Item3))
				ifTrue(arg.Item1, arg.Item2, arg.Item3);
			return arg;
		}

		public static T IfNot<T, TResult>(this T arg, Func<T, bool> check, Func<T, TResult> ifTrue)
		{
			if (!check(arg))
				ifTrue(arg);
			return arg;
		}
		public static T IfNot<T>(this T arg, Func<T, bool> check, Action<T> ifTrue)
		{
			if (!check(arg))
				ifTrue(arg);
			return arg;
		}
		public static Tuple<T1, T2> IfNot<T1, T2, TResult>(this Tuple<T1, T2> arg, Func<T1, T2, bool> check, Func<T1, T2, TResult> ifTrue)
		{
			if (!check(arg.Item1, arg.Item2))
				ifTrue(arg.Item1, arg.Item2);
			return arg;
		}
		public static Tuple<T1, T2, T3> IfNot<T1, T2, T3, TResult>(this Tuple<T1, T2, T3> arg, Func<T1, T2, T3, bool> check, Func<T1, T2, T3, TResult> ifTrue)
		{
			if (!check(arg.Item1, arg.Item2, arg.Item3))
				ifTrue(arg.Item1, arg.Item2, arg.Item3);
			return arg;
		}

		public static TResult If<TResult>(this bool arg, Func<TResult> ifTrue, Func<TResult> ifFalse)
			=> arg ? ifTrue() : ifFalse();
		public static TResult If<TResult>(this bool arg, TResult ifTrue, TResult ifFalse)
			=> arg ? ifTrue : ifFalse;

		public static TResult If<TResult, TArg>(this bool arg, TArg arg2, Func<TArg, TResult> ifTrue, Func<TArg, TResult> ifFalse)
			=> arg ? ifTrue(arg2) : ifFalse(arg2);

		public static TResult If<TResult, TArg1, TArg2>(this bool arg, TArg1 arg1, TArg2 arg2, Func<TArg1, TResult> ifTrue, Func<TArg1, TArg2, TResult> ifFalse)
			=> arg ? ifTrue(arg1) : ifFalse(arg1, arg2);

		public static TResult Complex<TResult, TArg, TFuncResult>(this TArg arg,
				Func<Func<TArg, TFuncResult>, TResult> func1, Func<TArg, TFuncResult> func2)
			=> func1(x => func2(arg));
		public static TResult Complex<TResult, TArg, TFuncResult>(this TArg arg,
				Func<TArg, Func<TArg, TFuncResult>, TResult> func1, Func<TArg, TFuncResult> func2)
			=> func1(arg, func2);
		public static TResult Complex<TResult, TArg1, TArg2, TFuncResult>(this TArg1 arg1, TArg2 arg2,
				Func<TArg1, TArg2, Func<TArg1, TArg2, TFuncResult>, TResult> func1, Func<TArg1, TArg2, TFuncResult> func2)
			=> func1(arg1, arg2, func2);

		public static TResult Lock<T, TResult>(this T arg, Func<T, TResult> func)
		{
			lock (arg)
				return func(arg);
		}

		public static T For<T, TIncrement>(this T arg, TIncrement incVar, Func<TIncrement, bool> incCheck, Func<TIncrement, TIncrement> incFunc,
			Action<T, TIncrement> action)
		{
			for (var i = incVar; incCheck(i); i=incFunc(i))
				action(arg, i);
			return arg;
		}
		public static T For<T, TIncrement, TResult>(this T arg, TIncrement incVar, Func<TIncrement, bool> incCheck, Func<TIncrement, TIncrement> incFunc,
			Func<T, TIncrement, TResult> action)
		{
			for (var i = incVar; incCheck(i); i = incFunc(i))
				action(arg, i);
			return arg;
		}
		public static IEnumerable<TResult> ForToArray<T, TIncrement, TResult>(this T arg, TIncrement incVar, Func<TIncrement, bool> incCheck, Func<TIncrement, TIncrement> incFunc,
			Func<T, TIncrement, TResult> action)
		{
			for (var i = incVar; incCheck(i); i = incFunc(i))
				yield return action(arg, i);
		}
		//public static T For<T, TIncrement>(this TIncrement incVar, T arg, Func<TIncrement, bool> incCheck,
		// Func<TIncrement, TIncrement> incFunc,
		// Action<T, TIncrement> action)
		//{
		// for (var i = incVar; incCheck(i); i = incFunc(i))
		//  action(arg, i);
		// return arg;
		//}

		public static T While<T>(this T arg, Func<T, bool> checkFunc, Func<T, T> func)
		{
			var a = arg;
			while (checkFunc(a)) a = func(a);
			return arg;
		}

		public static TResult While<T, TResult>(this T arg, Func<T, bool> checkFunc, Func<T, T> func, Func<T, TResult, TResult> resultFunc)
		{
			var a = arg;
			TResult result = default(TResult);
			while (checkFunc(a))
			{
				result = resultFunc(a, result);
				a = func(a);
			}
			return result;
		}

		public static T DoWhile<T>(this T arg, Func<T, bool> checkFunc, Func<T, T> func)
		{
			var a = arg;
			while (checkFunc(a = func(a))) ;
			return arg;
		}

		public static TResult Try<T, TResult>(this T arg, Func<T, TResult> func, Action<T, Exception> @catch,
			Func<T, TResult, TResult> @finally)
		{
			TResult result = default(TResult);
			try
			{
				result = func(arg);
			}
			catch (Exception e)
			{
				@catch(arg, e);
			}
			finally
			{
				result = @finally(arg, result);
			}
			return result;
		}

		public static TResult Using<T, TUsing, TResult>(this T arg, Func<T, TUsing> init, Func<T, TUsing, TResult> func) where TUsing : IDisposable
		{
			using (var u = init(arg))
				return func(arg, u);
		}

		public static T Using<T, TUsing>(this T arg, Func<T, TUsing> init, Action<T, TUsing> func) where TUsing : IDisposable
		{
			using (var u = init(arg))
				func(arg, u);
			return arg;
		}

		public static TResult Using<T, TResult>(this T arg, Func<T, TResult> func) where T : IDisposable
		{
			using (arg)
				return func(arg);
		}

		public static T ThrowIf<T, TException>(this T arg, Func<T, bool> check, Func<T, TException> func) where TException : Exception
		{
			if (check(arg))
				throw func(arg);
			return arg;
		}

		public static T Throw<T, TException>(this T arg, Func<T, TException> func) where TException : Exception
		{
			throw func(arg);
		}

		public static SwitchResult<TResult, T> Switch<T, TResult>(this T arg, T checkValue, Func<T, TResult> func)
		{
			if (arg.IsNull() && checkValue.IsNull() || arg.NotNull() && arg.Equals(checkValue))
				return new SwitchResult<TResult, T>(arg, func(arg));
			return new SwitchResult<TResult, T>(arg);
		}
		public static SwitchResult<T> Switch<T>(this T arg, T checkValue, Action<T> func)
		{
			if (arg.IsNull() && checkValue.IsNull() || arg.NotNull() && arg.Equals(checkValue))
				return new SwitchResult<T>(arg.Fluent(func), true);
			return new SwitchResult<T>(arg);
		}

		public static SwitchResult<TResult, T> Switch<T, TResult>(this T arg, Func<T, bool> check, Func<T, TResult> func)
		{
			if (check(arg))
				return new SwitchResult<TResult, T>(arg, func(arg));
			return new SwitchResult<TResult, T>(arg);
		}

		public static SwitchResult<T> Switch<T>(this T arg, Func<T, bool> check, Action<T> func)
		{
			if (check(arg))
			{
				func(arg);
				return new SwitchResult<T>(arg, true);
			}
			return new SwitchResult<T>(arg);
		}

		public static SwitchResult<TResult, T> Switch<T, TResult>(this SwitchResult<TResult, T> arg, T checkValue, Func<T, TResult> func)
		{
			if (!arg.HasValue && (arg.Arg.IsNull() && checkValue.IsNull() || arg.Arg.NotNull() && arg.Arg.Equals(checkValue)))
				return arg.SetValue(func(arg.Arg));
			return arg;
		}

		public static SwitchResult<TResult, T> Switch<T, TResult>(this SwitchResult<TResult, T> arg, Func<T, bool> check, Func<T, TResult> func)
		{
			if (!arg.HasValue && check(arg.Arg))
				return arg.SetValue(func(arg.Arg));
			return arg;
		}

		public static SwitchResult<T> Switch<T>(this SwitchResult<T> arg, Func<T, bool> check, Action<T> func)
		{
			if (!arg.HasValue && check(arg.Arg))
				return arg.Done(func);
			return arg;
		}

		public static SwitchResult<TResult, IEnumerable<T>> Switch<T, TResult>(this IEnumerable<T> arg, IEnumerable<T> checkValue, Func<IEnumerable<T>, TResult> func)
		{
			if (arg.IsNull() && checkValue.IsNull() || arg.NotNull() && arg.SeqEquals(checkValue))
				return new SwitchResult<TResult, IEnumerable<T>>(arg, func(arg));
			return new SwitchResult<TResult, IEnumerable<T>>(arg);
		}

		public static SwitchResult<TResult, IEnumerable<T>> Switch<T, TResult>(this SwitchResult<TResult, IEnumerable<T>> arg, IEnumerable<T> checkValue, Func<IEnumerable<T>, TResult> func)
		{
			if (!arg.HasValue && (arg.Arg.IsNull() && checkValue.IsNull() || arg.Arg.NotNull() && arg.Arg.SeqEquals(checkValue)))
				return arg.SetValue(func(arg.Arg));
			return arg;
		}

		public static SwitchResult<TResult, T> SwitchNotNull<T, TMiddle, TResult>(this T arg, Func<T, TMiddle> getValue, Func<TMiddle, TResult> func)
		{
			TMiddle value = getValue(arg);
			if (value.NotNull())
				return new SwitchResult<TResult, T>(arg, func(value));
			return new SwitchResult<TResult, T>(arg);
		}
		public static SwitchResult<TResult, T> SwitchNotNull<T, TMiddle, TResult>(this SwitchResult<TResult, T> arg, Func<T, TMiddle> getValue, Func<TMiddle, TResult> func)
		{
			if (arg.HasValue)
				return arg;
			TMiddle value = getValue(arg.Arg);
			if (value.NotNull())
				return arg.SetValue(func(value));
			return arg;
		}

		public static T Stopwatch<T>(this T arg, Stopwatch sw, Func<T, T> func)
		{
			sw.Start();
			func(arg);
			sw.Stop();
			return arg;
		}

		public static TResult Stopwatch<T, TResult>(this T arg, Stopwatch sw, Func<T, TResult> func)
		{
			sw.Start();
			var r = func(arg);
			sw.Stop();
			return r;
		}
	}

	public class SwitchResult<T, TArg>
	{
		public TArg Arg { get; }
		public T Value { get; private set; }
		public bool HasValue { get; private set; }

		public SwitchResult(TArg arg) { Arg = arg; }
		public SwitchResult(TArg arg, T value)
		{
			Arg = arg;
			Value = value;
			HasValue = true;
		}
		public SwitchResult<T, TArg> SetValue(T value)
		{
			if (Value.NotNull())
				throw new Exception("Value is not null");
			Value = value;
			HasValue = true;
			return this;
		}

		public T SwitchDefault(Func<TArg, T> func) => !this.HasValue ? func(this.Arg) : this.Value;
	}

	public class SwitchResult<TArg>
	{
		public TArg Arg { get; }
		public bool HasValue { get; private set; }

		public SwitchResult(TArg arg, bool done = false)
		{
			Arg = arg;
			HasValue = done;
		}

		public void SwitchDefault(Action<TArg> func)
		{
			if(!this.HasValue) func(this.Arg);
		}

		public SwitchResult<TArg> Done(Action<TArg> func)
		{
			func(Arg);
			HasValue = true;
			return this;
		}
	}
}
