using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SharpFuncExt;

namespace SharpFunc.ConsoleApp
{
    public class FileDb : IDisposable
    {
	    private readonly FileStream _file;
	    public FileDb(string filename)
	    {
		    _file = new FileStream(filename, FileMode.OpenOrCreate);
	    }

	    public void Dispose() => _file.Dispose();

	    public byte[] ReadBytes(long pos, int length) =>
		    pos.Extend(length)
			    .ThrowIf(x => x.Item1 < 0 || x.Item2 <= 0 || x.Item1 + x.Item2 > _file.Length, x => new ArgumentOutOfRangeException($"Pos: {x.Item1}, Length: {x.Item2}, FileLength: {_file.Length}"))
			    .Fluent(x => _file.Seek(x.Item1, SeekOrigin.Begin))
			    .Convert(x => new byte[x.Item2].Fluent(y => _file.Read(y, 0, y.Length)));

	    public void WriteBytes(long pos, byte[] buffer) =>
		    _file.Write(buffer.HasNotNullArg(nameof(buffer))
			    .Fluent(x => _file.Seek(pos.ThrowIf(y => y < 0, e => new ArgumentOutOfRangeException()),
					    SeekOrigin.Begin)), 0, buffer.Length);
    }
}
