using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Data;

namespace Trivial.Text;

class CsvVerb : CommandLine.BaseCommandVerb
{
    public static string Description => "CSV";

    protected override async Task OnProcessAsync(CancellationToken cancellationToken)
    {
        var text = "ab,cd,\"efg\",56789,!!!\nhijk,l,mn,43210";
        var parser = new CsvParser(text);
        await RunAsync(null, cancellationToken);
        foreach (var line in parser)
        {
            Console.Write(line.Count.ToString());
            Console.WriteLine(" \t{0} \t{1} \t{2} \t{3}", line[0], line[1], line[2], line[3]);
        }

        foreach (var m in parser.ConvertTo<CsvModel>(new[] { "A", "B", "C", "Num" }))
        {
            Console.WriteLine("Model \t{0} \t{1} \t{2} \t{3}", m.A, m.B, m.C, m.Num);
        }

        var reader = new StringTableDataReader(parser, new[] { "o", "p", "q", "i" });
        while (reader.Read())
        {
            Console.Write(reader.FieldCount.ToString());
            Console.WriteLine(" \t{0} \t{1} \t{2} \t{3}", reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3));
        }
    }
}

class CsvModel
{
    public string A { get; set; }

    public string B { get; set; }

    public string C { get; set; }

    public int Num { get; set; }
}
