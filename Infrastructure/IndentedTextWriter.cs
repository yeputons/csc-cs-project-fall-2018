using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Infrastructure
{
    public class IndentedTextWriter : IDisposable
    {
	    public static readonly int SPACES = 4;
	    private readonly TextWriter writer;
	    public int Indentation { get; private set; }

	    public IndentedTextWriter(TextWriter writer)
	    {
		    this.writer = writer;
		    Indentation = 0;
	    }

	    public void WriteLine(string line)
	    {
			if (line.Contains("\n")) throw new ArgumentException("A single line expected, newline character found");
			for (var i = 0; i < Indentation; i++) writer.Write(' ');
			writer.WriteLine(line);
	    }

	    public void WriteLineUnindented(string line)
	    {
			if (Indentation < SPACES) throw new ArgumentException("There is no current indentation");
		    Indentation -= SPACES;
		    WriteLine(line);
		    Indentation += SPACES;
	    }

		public void WriteLines(IEnumerable<string> line)
	    {
		    foreach (var s in line)
		    {
			    WriteLine(s);
		    }
	    }

	    public void WriteLines(string lines)
	    {
			WriteLines(lines.Split('\n'));
	    }

	    public void WithIndent(Action body)
	    {
		    Indentation += SPACES;
		    body();
		    Indentation -= SPACES;
	    }

	    public void Dispose()
	    {
		    writer?.Dispose();
	    }
    }
}
