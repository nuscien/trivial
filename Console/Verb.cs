using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Console
{
    public abstract class Verb
    {
        private CancellationTokenSource cancel = new CancellationTokenSource();

        public abstract string Description { get; }

        public Arguments Arguments { get; internal set; }

        public BackModes DefaultBackMode { get; internal set; }

        public BackModes BackMode { get; set; } = BackModes.Default;

        public bool IsAborted
        {
            get
            {
                return cancel.IsCancellationRequested;
            }
        }

        public CancellationToken CancellationToken
        {
            get
            {
                return cancel.Token;
            }
        }

        public virtual void Init(Dispatcher dispatcher)
        {
        }

        public abstract void Process();

        public virtual bool IsValid()
        {
            return true;
        }

        public virtual void Cancel()
        {
            cancel.Cancel();
        }

        public virtual string GetHelp()
        {
            return null;
        }
    }

    public abstract class AsyncVerb: Verb
    {
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

        protected abstract Task ProcessAsync(CancellationToken cancellationToken);

        public Task ProcessAsync()
        {
            return ProcessAsync(CancellationToken);
        }

        public virtual void ProcessCancelled(TaskCanceledException ex)
        {
        }

        public virtual void ProcessFailed(AggregateException ex)
        {
        }
    }

    public class HelpVerb : Verb
    {
        internal class Item
        {
            public Item(string key, string value)
            {
                Key = key;
                Value = value;
            }

            public string Key { get; }

            public string Value { get; }
        }

        private string defaultUsage;

        private List<Item> items = new List<Item>();

        public override void Init(Dispatcher dispatcher)
        {
            base.Init(dispatcher);
            var defaultVerb = dispatcher.DefaultVerb;
            if (defaultVerb != null) defaultUsage = defaultVerb.Description;
            foreach (var item in dispatcher.VerbsRegistered())
            {
                if (string.IsNullOrWhiteSpace(item.MatchDescription)) continue;
                items.Add(new Item(item.MatchDescription, item.Verb.Description));
            }
        }

        public override string Description
        {
            get
            {
                return "Get help.";
            }
        }

        public string FurtherDescription { get; set; }

        public override void Process()
        {
            WriteLine(defaultUsage);
            foreach (var item in items)
            {
                WriteLine(item.Key);
                if (item.Value != null) WriteLine(item.Value.Replace("{0}", item.Key));
            }

            WriteLine(FurtherDescription);
        }

        private static void WriteLine(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                System.Console.WriteLine(str);
                System.Console.WriteLine();
            }
        }
    }

    public class ExitVerb : Verb
    {
        public bool ExitApp { get; set; }

        public override string Description
        {
            get
            {
                return ExitApp ? "Exit this application." : "Close the current conversation.";
            }
        }

        public override void Process()
        {
            if (!ExitApp)
            {
                BackMode = BackModes.Parent;
                return;
            }

            BackMode = BackModes.Exit;
            System.Console.WriteLine("Bye!");
            System.Console.WriteLine();
        }
    }
}
