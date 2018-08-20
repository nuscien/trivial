using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Trivial.Console
{
    /// <summary>
    /// Verb dispatcher.
    /// </summary>
    public class Dispatcher
    {
        /// <summary>
        /// The dispatcher item information.
        /// </summary>
        public class Item
        {
            /// <summary>
            /// Initializes a new instance of the Dispatcher.Item class.
            /// </summary>
            /// <param name="match">The match function.</param>
            /// <param name="verb">The verb handler.</param>
            /// <param name="matchDesc">The title of the match function.</param>
            public Item(Func<string, bool> match, Verb verb, string matchDesc = null)
            {
                Match = match;
                Verb = verb;
                MatchDescription = matchDesc;
            }

            /// <summary>
            /// Gets the match handler.
            /// </summary>
            public Func<string, bool> Match { get; }

            /// <summary>
            /// Gets the verb handler.
            /// </summary>
            public Verb Verb { get; }

            /// <summary>
            /// Gets the match description.
            /// </summary>
            public string MatchDescription { get; }
        }

        /// <summary>
        /// The event arguments of verb dispatcher.
        /// </summary>
        public class ProcessEventArgs: EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the Dispatcher.ProcessEventArgs class.
            /// </summary>
            /// <param name="args">The arguments.</param>
            public ProcessEventArgs(Arguments args, Verb verb)
            {
                Arguments = args;
                Verb = verb;
            }

            /// <summary>
            /// Gets the arguments.
            /// </summary>
            public Arguments Arguments { get; }

            /// <summary>
            /// Gets the verb handler processing.
            /// </summary>
            public Verb Verb { get; }
        }

        /// <summary>
        /// The verb handlers match table.
        /// </summary>
        private List<Item> items = new List<Item>();

        /// <summary>
        /// A value inidcating whether the processing is cancelled.
        /// </summary>
        private bool isCanceled = false;

        /// <summary>
        /// Gets or sets the default verb handler.
        /// </summary>
        public Verb DefaultVerb { get; set; }

        /// <summary>
        /// Gets or sets the unhandled verb handler.
        /// </summary>
        public Verb UnhandledVerb { get; set; }

        /// <summary>
        /// Gets the verb handler which is processing currently now.
        /// </summary>
        public Verb ProcessingVerb { get; private set; }

        /// <summary>
        /// Gets or sets the question message print before input.
        /// </summary>
        public string PositionString { get; set; } = "> ";

        /// <summary>
        /// Adds or removes the event handler which will occur before processing.
        /// </summary>
        public event EventHandler<ProcessEventArgs> Processing;

        /// <summary>
        /// Adds or removes the event handler which will occur after processing.
        /// </summary>
        public event EventHandler<ProcessEventArgs> Processed;

        /// <summary>
        /// Gets the verb handlers registered.
        /// </summary>
        /// <returns>A list of verb handler and their match conditions.</returns>
        public IList<Item> VerbsRegistered()
        {
            return items.ToList();
        }

        /// <summary>
        /// Processes.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Process(string args)
        {
            var arguments = new Arguments(args);
            Process(arguments);
        }

        /// <summary>
        /// Processes.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Process(IEnumerable<string> args)
        {
            var arguments = new Arguments(args);
            Process(arguments);
        }

        /// <summary>
        /// Processes.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Process(Arguments args)
        {
            Process(args, true);
        }

        /// <summary>
        /// Processes.
        /// </summary>
        /// <param name="loop">true if process again after done; otherwise, false.</param>
        public void Process(bool loop = false)
        {
            while (true)
            {
                System.Console.Write(PositionString);
                var str = System.Console.ReadLine();
                if (isCanceled) break;
                var args = new Arguments(str);
                var verb = Process(args, false);
                if (isCanceled || !loop || verb is BaseExitVerb) break;
            }

            isCanceled = false;
        }

        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        public void Cancel()
        {
            if (ProcessingVerb == null) return;
            isCanceled = true;
            ProcessingVerb.Cancel();
        }

        /// <summary>
        /// Register a verb handler.
        /// </summary>
        /// <param name="match">The match condition.</param>
        /// <param name="verb">The verb handler.</param>
        /// <param name="matchDesc">The match description.</param>
        public void Register(Regex match, Verb verb, string matchDesc = null)
        {
            if (match == null || verb == null) return;
            items.Add(new Item(match.IsMatch, verb, string.IsNullOrWhiteSpace(matchDesc) ? matchDesc : match.ToString()));
        }

        /// <summary>
        /// Register a verb handler.
        /// </summary>
        /// <param name="match">The match condition.</param>
        /// <param name="verb">The verb handler.</param>
        /// <param name="matchDesc">The match description.</param>
        public void Register(Func<string, bool> match, Verb verb, string matchDesc = null)
        {
            if (match == null || verb == null) return;
            items.Add(new Item(match, verb));
        }

        /// <summary>
        /// Register a verb handler.
        /// </summary>
        /// <param name="match">The verb key.</param>
        /// <param name="verb">The verb handler.</param>
        public void Register(string match, Verb verb)
        {
            Register(new[] { match }, verb);
        }

        /// <summary>
        /// Register a verb handler.
        /// </summary>
        /// <param name="match">The verb keys.</param>
        /// <param name="verb">The verb handler.</param>
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

        /// <summary>
        /// Prepares the verb handler instance by given arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>The verb handler instance filled with the arguments.</returns>
        private Verb PrepareVerb(Arguments args)
        {
            var verb = args.Verb != null ? args.Verb.ToString() : string.Empty;
            Verb processed = null;
            if (string.IsNullOrEmpty(verb)) processed = PrepareVerb(DefaultVerb, args);
            if (processed == null)
            {
                foreach (var item in items)
                {
                    if (!item.Match(args.Verb.ToString())) continue;
                    processed = PrepareVerb(item.Verb, args);
                    if (processed == null) continue;
                    break;
                }
            }

            if (processed == null) processed = PrepareVerb(UnhandledVerb, args);
            return processed;
        }

        /// <summary>
        /// Prepares the verb handler instance by given arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="verb">The initialized verb handler instance.</param>
        /// <returns>The verb handler instance filled with the arguments.</returns>
        private Verb PrepareVerb(Verb verb, Arguments args)
        {
            if (verb == null) return null;
            verb.Arguments = args ?? throw new ArgumentNullException("args");
            args.Deserialize(verb);
            if (!verb.IsValid()) return null;
            return verb;
        }

        /// <summary>
        /// Processes.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="resetCancelState">true if need to reset the cancel state; otherwise, false.</param>
        /// <returns>The verb handler.</returns>
        private Verb Process(Arguments args, bool resetCancelState)
        {
            if (isCanceled)
            {
                if (resetCancelState) isCanceled = false;
                return null;
            }

            var verb = PrepareVerb(args);
            if (verb == null) return null;
            if (ProcessingVerb != null && !ProcessingVerb.IsCancelled)
            {
                try
                {
                    ProcessingVerb.Cancel();
                }
                catch (NullReferenceException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                catch (ArgumentException)
                {
                }
                finally
                {
                    ProcessingVerb = null;
                }
            }

            ProcessingVerb = verb;
            Processing?.Invoke(this, new ProcessEventArgs(args, verb));
            try
            {
                if (!verb.IsCancelled && !isCanceled) verb.Process();
                else if (resetCancelState) isCanceled = false;
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                ProcessingVerb = null;
            }

            Processed?.Invoke(this, new ProcessEventArgs(args, verb));
            return verb;
        }
    }
}
