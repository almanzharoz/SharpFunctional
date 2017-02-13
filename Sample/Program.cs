using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpFuncExt;

namespace SharpFunc.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
	        args.HasNotNullArg(nameof(args))
		        .Using(x => new FileDb(x[0]), (a, f) => a
			        .Switch(x => x[1].ToLower() == "read", x => Read(f, x[2].ToInt64(), x[3].ToInt32()))
			        .Switch(x => x[1].ToLower() == "write", x => Write(f, x[2], x[3].ToInt64())))
		        .Fluent(x => Console.ReadLine());
        }

	    private static void Write(FileDb file, string s, long pos) => file.WriteBytes(pos, Encoding.UTF8.GetBytes(s));

	    private static void Read(FileDb file, long pos, int length)
		    => Console.WriteLine(Encoding.UTF8.GetString(file.ReadBytes(pos, length)));

    }
}
