using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trivial.Data
{
    /// <summary>
    /// CSV reader.
    /// </summary>
    public class Csv
    {
        /// <summary>
        /// Parses CSV.
        /// </summary>
        /// <param name="csv">The CSV text.</param>
        /// <returns>Content of CSV.</returns>
        public static IEnumerable<List<string>> Parse(IEnumerable<string> csv)
        {
            foreach (var line in csv)
            {
                yield return ParseLine(line);
            }
        }

        /// <summary>
        /// Parses a line in CSV file.
        /// </summary>
        /// <param name="line">A line in CSV file.</param>
        /// <returns>Values in this line.</returns>
        public static List<string> ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return new List<string>();
            var arr = line.Split(',');
            if (line.IndexOf("\"") < 0) return arr.ToList();
            var list = new List<string>();
            var inScope = false;
            foreach (var item in arr)
            {
                if (!inScope)
                {
                    if (item.Length > 0 && item[0] == '"')
                    {
                        list.Add(item.Substring(1));
                        inScope = true;
                    }
                    else
                    {
                        list.Add(item);
                    }

                    continue;
                }

                if (item.Length > 0 && item[item.Length - 1] == '"' && (item.Length == 1 || item[item.Length - 2] != '\\'))
                {
                    list[list.Count - 1] += "," + item.Substring(0, item.Length - 1);
                    inScope = false;
                }
                else
                {
                    list[list.Count - 1] += "," + item;
                }
            }

            return list;
        }
    }
}
