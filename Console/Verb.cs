using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Console
{
    /// <summary>
    /// The verb handler.
    /// It can be used for sub-application.
    /// </summary>
    public abstract class Verb : IDisposable
    {
        /// <summary>
        /// The cancllation token source.
        /// </summary>
        private CancellationTokenSource cancel = new CancellationTokenSource();

        /// <summary>
        /// Gets the descripiton of the verb handler.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the input arguments.
        /// </summary>
        public Arguments Arguments { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the parameter is existed in arguments.
        /// </summary>
        /// <param name="key">The parameter key in arguments.</param>
        /// <returns>true if has this key; otherwise, false.</returns>
        public bool HasParameter(params string[] key) => Arguments?.Has(key) ?? false;

        /// <summary>
        /// Gets the parameters from arguments.
        /// </summary>
        /// <param name="key">The parameter key in arguments.</param>
        /// <param name="additionalKeys">The additional keys.</param>
        /// <returns>true if has this key; otherwise, false.</returns>
        public Parameters GetParameters(string key, params string[] additionalKeys) => Arguments?.Get(key, additionalKeys);

        /// <summary>
        /// Gets a value indicating whether the verb is existed in arguments.
        /// </summary>
        public bool HasVerbParameter => Arguments?.HasVerb ?? false;

        /// <summary>
        /// Gets the word in parameter.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A word in parameter.</returns>
        public string GetWordInParameter(int index) => Arguments?[index];

        /// <summary>
        /// Gets the word in parameter.
        /// </summary>
        /// <param name="key">The parameter key in arguments.</param>
        /// <returns>A word in parameter.</returns>
        public string GetParameterString(string key) => Arguments?[key];

        /// <summary>
        /// Gets the verb in arguments.
        /// </summary>
        public Parameter VerbParameter => Arguments?.Verb;

        /// <summary>
        /// Gets a value indicating whether it is cancelled.
        /// </summary>
        public bool IsCancelled => cancel.IsCancellationRequested;

        /// <summary>
        /// Gets the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken => cancel.Token;

        /// <summary>
        /// Gets the default console line client.
        /// </summary>
        public Line ConsoleLine { get; } = new Line();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="dispatcher">The caller.</param>
        public virtual void Init(Dispatcher dispatcher)
        {
        }

        /// <summary>
        /// Processes.
        /// </summary>
        public abstract void Process();

        /// <summary>
        /// Gets a value indicating whether the input parameter or the current environment is valid.
        /// </summary>
        /// <returns>true if the input parameter or the current environment is valid; otherwise, false.</returns>
        public virtual bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        public virtual void Cancel()
        {
            cancel.Cancel();
        }

        /// <summary>
        /// Gets details help message.
        /// </summary>
        /// <returns>The string of the usage documentation content.</returns>
        public virtual void GetHelp()
        {
            if (!string.IsNullOrWhiteSpace(Description)) System.Console.WriteLine(Description);
        }

        /// <summary>
        /// Throws an operation canceled exception if this token has had cancellation requested.
        /// </summary>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ObjectDisposedException">The associated cancellation token source has been disposed.</exception>
        protected void ThrowIfCancellationRequested()
        {
            CancellationToken.ThrowIfCancellationRequested();
        }

        #region IDisposable Support

        /// <summary>
        /// A value to mark if the instance is disposed.
        /// </summary>
        public bool HasDisposed { get; private set; }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing">true if also dispose the managed resources; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (HasDisposed) return;
            if (disposing)
            {
                ConsoleLine.End();
                Arguments = null;
                cancel.Dispose();
            }

            HasDisposed = true;
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }

    /// <summary>
    /// The asynchronized verb handler.
    /// </summary>
    public abstract class AsyncVerb: Verb
    {
        /// <summary>
        /// Processes.
        /// </summary>
        /// <exception cref="AggregateException">An exception was thrown during the execution of the task.</exception>
        /// <exception cref="TaskCanceledException">The task was canceled.</exception>
        /// <exception cref="ObjectDisposedException">The task has been disposed.</exception>
        public override void Process()
        {
            var task = ProcessAsync();
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                if (task.IsCanceled) ProcessCancelled(ex.InnerException as TaskCanceledException);
                else ProcessFailed(ex);
            }
        }

        /// <summary>
        /// Processes.
        /// </summary>
        /// <returns>The async processing task.</returns>
        public abstract Task ProcessAsync();

        /// <summary>
        /// Occurs when it is cancelled.
        /// </summary>
        /// <param name="ex">The task cancelled exception.</param>
        public virtual void ProcessCancelled(TaskCanceledException ex)
        {
            throw ex;
        }

        /// <summary>
        /// Occurs when it is failed to process.
        /// </summary>
        /// <param name="ex">The exception. Its inner exceptions property contains information about the exception or exceptions.</param>
        public virtual void ProcessFailed(AggregateException ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Help verb handler.
    /// </summary>
    public class HelpVerb : Verb
    {
        /// <summary>
        /// All the other verbs.
        /// </summary>
        private Dispatcher dispatcher = null;

        /// <summary>
        /// Gets or sets the description message.
        /// </summary>
        public string DescriptionMessage { get; set; } = "Get help.";

        /// <summary>
        /// Gets the descripiton of the verb handler.
        /// </summary>
        public override string Description => DescriptionMessage;

        /// <summary>
        /// Gets the additional description which will be appended to the last.
        /// </summary>
        public string FurtherDescription { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="dispatcher">The caller.</param>
        public override void Init(Dispatcher dispatcher)
        {
            base.Init(dispatcher);
            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// Processes.
        /// </summary>
        public override void Process()
        {
            if (dispatcher == null) return;
            var hasArg = Arguments.Count > 1 && !string.IsNullOrWhiteSpace(Arguments[1]);
            if (!hasArg)
            {
                WriteMessage(dispatcher.DefaultVerbFactory, false);
                System.Console.WriteLine();
            }

            foreach (var item in hasArg ? dispatcher.VerbFactorysRegistered(Arguments[1]) : dispatcher.VerbFactorysRegistered())
            {
                System.Console.WriteLine(item.MatchDescription);
                if (hasArg) System.Console.WriteLine();
                WriteMessage(item.VerbFactory, hasArg);
                System.Console.WriteLine();
            }

            if (!hasArg) LineUtilities.WriteDoubleLines(FurtherDescription);
        }

        private void WriteMessage(Func<Verb> verb, bool details)
        {
            if (verb == null) return;
            var v = verb();
            if (v == null) return;
            if (details) v.GetHelp();
            else if (!string.IsNullOrWhiteSpace(v.Description)) System.Console.WriteLine(v.Description);
        }
    }

    /// <summary>
    /// Base exit verb handler.
    /// </summary>
    public abstract class BaseExitVerb : Verb
    {
    }

    /// <summary>
    /// Exit verb handler.
    /// </summary>
    public class ExitVerb : BaseExitVerb
    {
        /// <summary>
        /// Gets or sets a value indicating whether it is only for turning back parent dispatcher.
        /// </summary>
        public bool Back { get; set; }

        /// <summary>
        /// Gets or sets the description string for back.
        /// </summary>
        public string BackMessage { get; set; } = "Close the current conversation.";

        /// <summary>
        /// Gets or sets the description string for exit.
        /// </summary>
        public string ExitMessage { get; set; } = "Exit this application.";

        /// <summary>
        /// Gets or sets the exit string.
        /// </summary>
        public string ByeMessage { get; set; } = "Bye!";

        /// <summary>
        /// Gets the descripiton of the verb handler.
        /// </summary>
        public override string Description
        {
            get
            {
                return Back ? BackMessage : ExitMessage;
            }
        }

        /// <summary>
        /// Processes.
        /// </summary>
        public override void Process()
        {
            if (!Back) LineUtilities.WriteDoubleLines(ByeMessage);
        }
    }

    /// <summary>
    /// The utilities.
    /// </summary>
    public static class VerbExtension
    {
        /// <summary>
        /// Registers a help verb.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="furtherDesc">Additional description which will be appended to the last.</param>
        /// <returns>The help verb instance.</returns>
        public static void RegisterHelp(this Dispatcher dispatcher, string furtherDesc = null)
        {
            dispatcher.Register<HelpVerb>(new[] { "help", "?", "gethelp", "get-help" });
        }

        /// <summary>
        /// Registers a exit verb.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="back">true if only for turning back; otherwise, false.</param>
        /// <returns>The exit verb instance.</returns>
        public static void RegisterExit(this Dispatcher dispatcher, bool back = false)
        {
            dispatcher.Register<ExitVerb>(new[] { "exit", "quit", "bye", "goodbye" });
        }
    }
}
