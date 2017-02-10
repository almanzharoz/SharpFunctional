using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpFuncExt
{
    public static class StringFunc
    {
		public static bool IsNull(this string arg) => String.IsNullOrWhiteSpace(arg);
	}
}
