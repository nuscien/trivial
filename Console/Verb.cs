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
    public abstract class Verb
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
        /// The input arguments.
        /// </summary>
        public Arguments Arguments { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether it is cancelled.
        /// </summary>
        public bool IsCancelled
        {
            get
            {
                return cancel.IsCancellationRequested;
            }
        }

        /// <summary>
        /// Gets the cancellation token.
        /// </summary>
        public CancellationToken CancellationToken
        {
            get
            {
                return cancel.Token;
            }
        }

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
        public virtual string GetHelp()
        {
            return null;
        }
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
        /// <returns>The task.</returns>
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
            if (!hasArg) WriterUtilities.WriteDoubleLine(GetMessage(dispatcher.DefaultVerbFactory, false));
            foreach (var item in hasArg ? dispatcher.VerbFactorysRegistered(Arguments[1]) : dispatcher.VerbFactorysRegistered())
            {
                var msg = GetMessage(item.VerbFactory, hasArg);
                if (string.IsNullOrWhiteSpace(msg)) continue;
                System.Console.WriteLine(item.MatchDescription);
                WriterUtilities.WriteDoubleLine(msg.Replace("{0}", item.MatchDescription));
            }

            if (!hasArg) WriterUtilities.WriteDoubleLine(FurtherDescription);
        }

        private string GetMessage(Func<Verb> verb, bool details)
        {
            if (verb == null) return null;
            var v = verb();
            if (v == null) return null;
            return details ? v.GetHelp() : v.Description;
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
            if (!Back) WriterUtilities.WriteDoubleLine(ByeMessage);
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

        /// <summary>
        /// Processes a command.
        /// </summary>
        /// <param name="cmd">The command string.</param>
        /// <returns>The output string.</returns>
        public static string Command(string cmd)
        {
            var p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            p.StandardInput.WriteLine(cmd + "&exit");
            p.StandardInput.AutoFlush = true;
            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return output;
        }
    }
}
