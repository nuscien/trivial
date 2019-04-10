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
            /// <param name="verbFactory">The factory of the verb handler.</param>
            /// <param name="matchDesc">The title of the match function.</param>
            public Item(Func<string, bool> match, Func<Verb> verbFactory, string matchDesc = null)
            {
                Match = match;
                VerbFactory = verbFactory;
                MatchDescription = matchDesc;
            }

            /// <summary>
            /// Gets the match handler.
            /// </summary>
            public Func<string, bool> Match { get; }

            /// <summary>
            /// Gets the factory of the verb handler.
            /// </summary>
            public Func<Verb> VerbFactory { get; }

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
            /// <param name="verb">The verb handler.</param>
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
            /// Gets the verb handler.
            /// </summary>
            public Verb Verb { get; }
        }

        /// <summary>
        /// The verb handlers match table.
        /// </summary>
        private readonly List<Item> items = new List<Item>();

        /// <summary>
        /// A value inidcating whether the processing is cancelled.
        /// </summary>
        private bool isCanceled = false;

        /// <summary>
        /// Gets or sets the default verb handler.
        /// </summary>
        public Func<Verb> DefaultVerbFactory { get; set; }

        /// <summary>
        /// Gets or sets the unhandled verb handler.
        /// </summary>
        public Func<Verb> UnhandledVerbFactory { get; set; }

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
        /// Adds or removes the event handler which will occur after processing succeeded.
        /// </summary>
        public event EventHandler<ProcessEventArgs> Processed;

        /// <summary>
        /// Adds or removes the event handler which will occur after processing failure.
        /// </summary>
        public event EventHandler<ProcessEventArgs> ProcessFailed;

        /// <summary>
        /// Gets the verb handlers registered.
        /// </summary>
        /// <returns>A list of verb handler and their match conditions.</returns>
        public IList<Item> VerbFactorysRegistered()
        {
            return items.ToList();
        }

        /// <summary>
        /// Gets the verb handlers registered.
        /// </summary>
        /// <param name="key">The verb key to match.</param>
        /// <returns>A list of verb handler and their match conditions.</returns>
        public IEnumerable<Item> VerbFactorysRegistered(string key)
        {
            return items.ToList().Where(item =>
            {
                if (item.MatchDescription == null) return false;
                return item.MatchDescription == key || item.MatchDescription.IndexOf(key + " ") == 0;
            });
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
                if (isCanceled || !loop || verb is ExitVerb) break;
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
        /// Register the default verb handler.
        /// </summary>
        /// <typeparam name="T">The type of the verb handler.</typeparam>
        public void RegisterDefault<T>() where T : Verb
        {
            DefaultVerbFactory = Activator.CreateInstance<T>;
        }

        /// <summary>
        /// Register the unhandled verb handler.
        /// </summary>
        /// <typeparam name="T">The type of the verb handler.</typeparam>
        public void RegisterUnhandled<T>() where T : Verb
        {
            UnhandledVerbFactory = Activator.CreateInstance<T>;
        }

        /// <summary>
        /// Register a verb handler.
        /// </summary>
        /// <param name="match">The match condition.</param>
        /// <param name="verbFactory">The verb factory.</param>
        /// <param name="matchDesc">The match description.</param>
        public void Register(Regex match, Func<Verb> verbFactory, string matchDesc = null)
        {
            if (match == null || verbFactory == null) return;
            items.Add(new Item(match.IsMatch, verbFactory, string.IsNullOrWhiteSpace(matchDesc) ? matchDesc : match.ToString()));
        }

        /// <summary>
        /// Register a verb handler.
        /// </summary>
        /// <typeparam name="T">The type of the verb handler.</typeparam>
        /// <param name="match">The match condition.</param>
        /// <param name="matchDesc">The match description.</param>
        public void Register<T>(Regex match, string matchDesc = null) where T: Verb
        {
            Register(match, Activator.CreateInstance<T>, matchDesc);
        }

        /// <summary>
        /// Register a verb handler.
        /// </summary>
        /// <param name="match">The match condition.</param>
        /// <param name="verbFactory">The verb factory.</param>
        /// <param name="matchDesc">The match description.</param>
        public void Register(Func<string, bool> match, Func<Verb> verbFactory, string matchDesc = null)
        {
            if (match == null || verbFactory == null) return;
            items.Add(new Item(match, verbFactory));
        }

        /// <summary>
        /// Register a verb handler.
        /// </summary>
        /// <typeparam name="T">The type of the verb handler.</typeparam>
        /// <param name="match">The match condition.</param>
        /// <param name="matchDesc">The match description.</param>
        public void Register<T>(Func<string, bool> match, string matchDesc = null) where T : Verb
        {
            Register(match, Activator.CreateInstance<T>, matchDesc);
        }

        /// <summary>
        /// Register a verb handler.
        /// </summary>
        /// <param name="match">The verb key.</param>
        /// <param name="verbFactory">The verb factory.</param>
        public void Register(string match, Func<Verb> verbFactory)
        {
            Register(new[] { match }, verbFactory);
        }

        /// <summary>
        /// Register a verb handler.
        /// </summary>
        /// <typeparam name="T">The type of the verb handler.</typeparam>
        /// <param name="match">The verb key.</param>
        public void Register<T>(string match) where T : Verb
        {
            Register(new[] { match }, Activator.CreateInstance<T>);
        }

        /// <summary>
        /// Register a verb handler.
        /// </summary>
        /// <param name="match">The verb keys.</param>
        /// <param name="verbFactory">The verb factory.</param>
        public void Register(IEnumerable<string> match, Func<Verb> verbFactory)
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
                if (key == null)
                {
                    foreach (var item in list)
                    {
                        if (string.IsNullOrEmpty(item)) return true;
                    }
                }
                else
                {
                    foreach (var item in list)
                    {
                        if (key == item || key.IndexOf(item + " ") == 0) return true;
                    }
                }

                return false;
            }, verbFactory, list[0]));
        }

        /// <summary>
        /// Register a verb handler.
        /// </summary>
        /// <typeparam name="T">The type of the verb handler.</typeparam>
        /// <param name="match">The verb key.</param>
        public void Register<T>(IEnumerable<string> match) where T : Verb
        {
            Register(match, Activator.CreateInstance<T>);
        }

        /// <summary>
        /// Clear all the verb factories registered.
        /// </summary>
        public void ClearRegistered()
        {
            items.Clear();
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
            if (string.IsNullOrEmpty(verb)) processed = PrepareVerb(DefaultVerbFactory, args);
            if (processed == null)
            {
                foreach (var item in items)
                {
                    if (!item.Match(verb)) continue;
                    processed = PrepareVerb(item.VerbFactory, args);
                    if (processed == null) continue;
                    break;
                }
            }

            if (processed == null) processed = PrepareVerb(UnhandledVerbFactory, args);
            return processed;
        }

        /// <summary>
        /// Prepares the verb handler instance by given arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="verbFactory">The initialized verb handler instance.</param>
        /// <returns>The verb handler instance filled with the arguments.</returns>
        private Verb PrepareVerb(Func<Verb> verbFactory, Arguments args)
        {
            if (verbFactory == null) return null;
            var v = verbFactory();
            if (v == null) return null;
            v.Arguments = args ?? throw new ArgumentNullException(nameof(args));
            args.Deserialize(v);
            if (!v.IsValid()) return null;
            return v;
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
            if (ProcessingVerb != null && !ProcessingVerb.HasDisposed)
            {
                try
                {
                    if (!ProcessingVerb.IsCancelled) ProcessingVerb.Cancel();
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
                catch (OperationCanceledException)
                {
                }
                finally
                {
                    ProcessingVerb.Dispose();
                }
            }

            if (ProcessingVerb == verb) return null;
            ProcessingVerb = verb;
            Processing?.Invoke(this, new ProcessEventArgs(args, verb));
            try
            {
                if (!verb.IsCancelled && !isCanceled && !verb.HasDisposed) verb.Init(this);
                if (!verb.IsCancelled && !isCanceled && !verb.HasDisposed) verb.Process();
                else if (resetCancelState) isCanceled = false;
                Processed?.Invoke(this, new ProcessEventArgs(args, verb));
            }
            catch (OperationCanceledException)
            {
                ProcessFailed?.Invoke(this, new ProcessEventArgs(args, verb));
            }
            catch (Exception)
            {
                ProcessFailed?.Invoke(this, new ProcessEventArgs(args, verb));
                throw;
            }
            finally
            {
                verb.Dispose();
                ProcessingVerb = null;
            }

            return verb;
        }
    }
}
