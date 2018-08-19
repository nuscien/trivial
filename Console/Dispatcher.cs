using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Trivial.Console
{
    public enum BackModes
    {
        Default = 0,
        Exit = 1,
        Current = 2,
        Parent = 3,
        ParentAndKeep = 4,
        Top = 5,
        Reload = 6
    }

    public class Dispatcher
    {
        public class Item
        {
            public Item(Func<string, bool> match, Verb verb, string matchDesc = null)
            {
                Match = match;
                Verb = verb;
                MatchDescription = matchDesc;
            }

            public Func<string, bool> Match { get; }

            public Verb Verb { get; }

            public string MatchDescription { get; }
        }

        public class ProcessEventArgs: EventArgs
        {
            public ProcessEventArgs(Arguments args)
            {
                Arguments = args;
            }

            public Arguments Arguments { get; }
        }

        private List<Item> items = new List<Item>();

        private bool noArgs = false;

        public Verb DefaultVerb { get; set; }

        public Verb UnhandledVerb { get; set; }

        public Verb ProcessingVerb { get; private set; }

        public BackModes BackMode { get; set; } = BackModes.Default;

        public event EventHandler<ProcessEventArgs> Processing;

        public event EventHandler<ProcessEventArgs> Processed;

        public IList<Item> VerbsRegistered()
        {
            return items.ToList();
        }

        public void Process(IEnumerable<string> args)
        {
            var arguments = new Arguments(args);
            noArgs = arguments.Count == 0;
            Processing?.Invoke(this, new ProcessEventArgs(arguments));
            var verb = arguments.Verb != null ? arguments.Verb.ToString() : string.Empty;
            var processed = false;
            if (string.IsNullOrEmpty(verb)) processed = Process(DefaultVerb, arguments);
            if (!processed) foreach (var item in items)
            {
                if (!item.Match(arguments.Verb.ToString()) || !Process(item.Verb, arguments)) continue;
                break;
            }

            if (!processed) processed = Process(UnhandledVerb, arguments);
            Processed?.Invoke(this, new ProcessEventArgs(arguments));
        }

        public void Cancel()
        {
            var v = ProcessingVerb;
            if (v == null) return;
            v.Cancel();
        }

        public void Register(Regex match, Verb verb, string matchDesc = null)
        {
            if (match == null || verb == null) return;
            items.Add(new Item(match.IsMatch, verb, string.IsNullOrWhiteSpace(matchDesc) ? matchDesc : match.ToString()));
        }

        public void Register(Func<string, bool> match, Verb verb, string matchDesc = null)
        {
            if (match == null || verb == null) return;
            items.Add(new Item(match, verb));
        }

        public void Register(string match, Verb verb)
        {
            Register(new[] { match }, verb);
        }

        public void Register(IEnumerable<string> match, Verb verb)
        {
            if (match == null) return;
            var list = match.Where(item =>
            {
                return !string.IsNullOrWhiteSpace(item);
            }).Select(item =>
            {
                return Parameter.FormatKey(item, false);
            }).ToList();
            if (list.Count == 0) return;
            items.Add(new Item(key =>
            {
                foreach (var item in list)
                {
                    var v = verb.Arguments.Verb;
                    if (v == null) return false;
                    if (v.Key == item) return true;
                    var verbStr = v.ToString();
                    if (verbStr == item) return true;
                    return verbStr.IndexOf(match + " ") == 0;
                }

                return false;
            }, verb, list[0]));
        }

        public bool Process(Verb verb, Arguments args)
        {
            if (verb == null) return false;
            verb.Arguments = args ?? throw new ArgumentNullException("args");
            args.Deserialize(verb);
            if (!verb.IsValid()) return false;
            ProcessingVerb = verb;
            try
            {
                verb.Process();
            }
            finally
            {
                ProcessingVerb = null;
            }

            return true;
        }
    }
}
