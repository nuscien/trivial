using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Trivial.Data;
using Trivial.Text;

namespace Trivial.Sample
{
    class CsvVerb : Trivial.Console.Verb
    {
        public override string Description => "CSV";

        public override void Process()
        {
            var text = "ab,cd,\"efg\",56789,!!!\nhijk,l,mn,43210";
            var parser = new CsvParser(text);
            foreach (var line in parser)
            {
                ConsoleLine.Write(ConsoleColor.Blue, line.Count.ToString());
                ConsoleLine.WriteLine(" \t{0} \t{1} \t{2} \t{3}", line[0], line[1], line[2], line[3]);
            }

            foreach (var m in parser.ConvertTo<CsvModel>(new[] { "A", "B", "C", "Num" }))
            {
                ConsoleLine.WriteLine("Model \t{0} \t{1} \t{2} \t{3}", m.A, m.B, m.C, m.Num);
            }

            var reader = new StringTableDataReader(parser, new[] { "o", "p", "q", "i" });
            while (reader.Read())
            {
                ConsoleLine.Write(ConsoleColor.Blue, reader.FieldCount.ToString());
                ConsoleLine.WriteLine(" \t{0} \t{1} \t{2} \t{3}", reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3));
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
}
