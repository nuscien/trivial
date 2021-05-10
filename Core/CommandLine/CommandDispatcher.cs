using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Trivial.Reflection;

namespace Trivial.CommandLine
{
    /// <summary>
    /// The conversation modes for command processing.
    /// </summary>
    public enum CommandConversationModes
    {
        /// <summary>
        /// Turns on when no arguments input; otherwise, turns off.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// Turns off the conversation mode.
        /// </summary>
        Off = 1,

        /// <summary>
        /// Keeps to turns on.
        /// </summary>
        On = 2
    }

    /// <summary>
    /// Verb dispatcher.
    /// </summary>
    public class CommandDispatcher : IEnumerable<CommandDispatcher.Route>
    {
        /// <summary>
        /// The command handler route.
        /// </summary>
        public sealed class Route
        {
            /// <summary>
            /// The key match handler.
            /// </summary>
            private readonly Func<string, bool> match;

            /// <summary>
            /// The keys store.
            /// </summary>
            private readonly List<string> keys; 

            /// <summary>
            /// Initializes a new instance of the CommandDispatcher.Item class.
            /// </summary>
            /// <param name="key">The key to register.</param>
            /// <param name="handler">The command handler to bind.</param>
            public Route(string key, ICommandHandler handler)
            {
                Handler = handler;
                if (key == null) return;
                key = key?.Trim();
                keys = new List<string> { key?.Trim() };
            }

            /// <summary>
            /// Initializes a new instance of the CommandDispatcher.Item class.
            /// </summary>
            /// <param name="keys">The keys to register.</param>
            /// <param name="handler">The command handler to bind.</param>
            public Route(IEnumerable<string> keys, ICommandHandler handler)
            {
                Handler = handler;
                if (keys == null) return;
                this.keys = keys.Select(k => k?.Trim()).Where(k => !string.IsNullOrEmpty(k)).Distinct().ToList();
            }

            /// <summary>
            /// Initializes a new instance of the CommandDispatcher.Item class.
            /// </summary>
            /// <param name="test">The key test handler.</param>
            /// <param name="handler">The command handler to bind.</param>
            public Route(Func<string, bool> test, ICommandHandler handler)
            {
                Handler = handler;
                match = test;
            }

            /// <summary>
            /// Initializes a new instance of the CommandDispatcher.Item class.
            /// </summary>
            /// <param name="regular">The key regular.</param>
            /// <param name="handler">The command handler to bind.</param>
            public Route(Regex regular, ICommandHandler handler)
            {
                Handler = handler;
                if (regular == null) return;
                match = regular.IsMatch;
            }

            /// <summary>
            /// Gets a value indicating whether the route item is empty.
            /// </summary>
            public bool IsEmpty => (keys == null || keys.Count < 1) && match is null;

            /// <summary>
            /// Gets the command handler.
            /// </summary>
            public ICommandHandler Handler { get; }

            /// <summary>
            /// Gets the count of static keys registered.
            /// </summary>
            internal int KeyCount => keys?.Count ?? 0;

            /// <summary>
            /// Gets static keys.
            /// </summary>
            internal IReadOnlyList<string> Keys => keys.AsReadOnly();

            /// <summary>
            /// Indicates whether the key matches the one registered.
            /// </summary>
            /// <param name="key">The verb.</param>
            /// <returns>true if matches; otherwise, false.</returns>
            public bool IsMatch(string key)
            {
                if (keys != null && keys.Any(k => k.Equals(key, StringComparison.OrdinalIgnoreCase)))
                    return true;
                return match?.Invoke(key) == true;
            }

            /// <summary>
            /// Removes the first occurrence of a specific object from this dispatcher. 
            /// </summary>
            /// <param name="key">The key.</param>
            /// <returns>true if remove succeeded; otherwise, false. This method also returns false if not found.</returns>
            internal bool RemoveKey(string key)
            {
                if (keys == null) return false;
                return keys.RemoveAll(k => k.Equals(key, StringComparison.OrdinalIgnoreCase)) > 0;
            }

            /// <summary>
            /// Tests if the test handler is the same handler registered.
            /// </summary>
            /// <param name="test">The test handler to compare.</param>
            /// <returns>true if they are the same one; otherwise, false.</returns>
            internal bool EqualsTestHandler(Func<string, bool> test) => test == match;
        }

        /// <summary>
        /// The event arguments of verb dispatcher.
        /// </summary>
        public class ProcessEventArgs : EventArgs
        {
            /// <summary>
            /// Initializes a new instance of the Dispatcher.ProcessEventArgs class.
            /// </summary>
            /// <param name="args">The command arguments.</param>
            /// <param name="handler">The command handler.</param>
            public ProcessEventArgs(CommandArguments args, ICommandHandler handler)
            {
                Arguments = args;
                Handler = handler;
            }

            /// <summary>
            /// Initializes a new instance of the Dispatcher.FailedProcessEventArgs class.
            /// </summary>
            /// <param name="args">The command arguments.</param>
            /// <param name="handler">The command handler.</param>
            /// <param name="ex">The unhandled exception.</param>
            public ProcessEventArgs(CommandArguments args, ICommandHandler handler, Exception ex)
                : this(args, handler)
            {
                Exception = ex;
            }

            /// <summary>
            /// Gets the command arguments.
            /// </summary>
            public CommandArguments Arguments { get; }

            /// <summary>
            /// Gets the command handler.
            /// </summary>
            public ICommandHandler Handler { get; }

            /// <summary>
            /// Gets the exception unhandled.
            /// </summary>
            public Exception Exception { get; }
        }

        /// <summary>
        /// Help keys.
        /// </summary>
        private readonly static IList<string> helpKeys = new List<string>() { "?", "help", "get-help", "gethelp", "帮助", "获取帮助" };

        /// <summary>
        /// Handlers.
        /// </summary>
        private readonly List<Route> list = new();

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
        /// Adds or removes the event handler which will occur after processing is canceled.
        /// </summary>
        public event EventHandler<ProcessEventArgs> ProcessCanceled;

        /// <summary>
        /// Adds or removes the event handler which will occur after getting help.
        /// </summary>
        public event EventHandler<ProcessEventArgs> GotHelp;

        /// <summary>
        /// Gets or sets the conversation mode.
        /// </summary>
        public CommandConversationModes ConversationMode { get; set; }

        /// <summary>
        /// Gets or sets the preload command handler.
        /// </summary>
        public ICommandHandler PreloadHandler { get; set; }

        /// <summary>
        /// Gets or sets the default command handler.
        /// </summary>
        public ICommandHandler DefaultHandler { get; set; }

        /// <summary>
        /// Gets or sets the default command handler.
        /// </summary>
        public ICommandHandler UnhandledHandler { get; set; }

        /// <summary>
        /// Gets or sets the command handler on exit.
        /// </summary>
        public ICommandHandler ExitHandler { get; set; }

        /// <summary>
        /// Gets the exit keys.
        /// </summary>
        public List<string> ExitKeys { get; } = new() { "exit", "quit", "close", "bye", "goodbye", "end", "关闭", "退出" };

        /// <summary>
        /// Gets or sets the input prompt.
        /// </summary>
        public string InputPrompt { get; set; }

        /// <summary>
        /// Registers a command handler.
        /// </summary>
        /// <param name="key">The key to register.</param>
        /// <param name="handler">A handler to register.</param>
        public Route Register(string key, ICommandHandler handler)
        {
            if (key == null) DefaultHandler = handler;
            if (string.IsNullOrWhiteSpace(key)) return null;
            var item = new Route(key, handler);
            if (item.IsEmpty) return null;
            list.Add(item);
            return item;
        }

        /// <summary>
        /// Registers a command handler.
        /// </summary>
        /// <param name="keys">The keys to register.</param>
        /// <param name="handler">A handler to register.</param>
        public Route Register(IEnumerable<string> keys, ICommandHandler handler)
        {
            if (keys == null) return null;
            var item = new Route(keys, handler);
            if (item.IsEmpty) return null;
            list.Add(item);
            return item;
        }

        /// <summary>
        /// Registers a command handler.
        /// </summary>
        /// <param name="test">The key test handler.</param>
        /// <param name="handler">A handler to register.</param>
        public Route Register(Func<string, bool> test, ICommandHandler handler)
        {
            if (test is null) return null;
            var item = new Route(test, handler);
            if (item.IsEmpty) return null;
            list.Add(item);
            return item;
        }

        /// <summary>
        /// Registers a command handler.
        /// </summary>
        /// <param name="regular">The key regular.</param>
        /// <param name="handler">A handler to register.</param>
        public Route Register(Regex regular, ICommandHandler handler)
        {
            if (regular is null) return null;
            var item = new Route(regular, handler);
            if (item.IsEmpty) return null;
            list.Add(item);
            return item;
        }

        /// <summary>
        /// Registers a command handler.
        /// </summary>
        /// <param name="handler">The command handler with key.</param>
        public Route Register(Route handler)
        {
            if (handler is null) return null;
            if (!list.Contains(handler)) list.Add(handler);
            return handler;
        }

        /// <summary>
        /// Registers a command handler.
        /// </summary>
        /// <param name="key">The key to register.</param>
        public Route Register<T>(string key) where T : BaseCommandVerb
            => Register(key, new CommandVerbHandler<T>());

        /// <summary>
        /// Registers a command handler.
        /// </summary>
        /// <param name="keys">The keys to register.</param>
        public Route Register<T>(IEnumerable<string> keys) where T : BaseCommandVerb
            => Register(keys, new CommandVerbHandler<T>());

        /// <summary>
        /// Registers a command handler.
        /// </summary>
        /// <param name="test">The key test handler.</param>
        public Route Register<T>(Func<string, bool> test) where T : BaseCommandVerb
            => Register(test, new CommandVerbHandler<T>());

        /// <summary>
        /// Registers a command handler.
        /// </summary>
        /// <param name="regular">The key regular.</param>
        public Route Register<T>(Regex regular) where T : BaseCommandVerb
            => Register(regular, new CommandVerbHandler<T>());

        /// <summary>
        /// Removes the specific command handler from this dispatcher. 
        /// </summary>
        /// <param name="handler"></param>
        /// <returns>true if remove succeeded; otherwise, false. This method also returns false if not found.</returns>
        public bool Remove(Route handler)
        {
            if (!list.Remove(handler)) return false;
            list.Remove(handler);
            return true;
        }

        /// <summary>
        /// Removes a route by key from this dispatcher. 
        /// </summary>
        /// <param name="key">The key to register.</param>
        /// <returns>true if remove succeeded; otherwise, false. This method also returns false if not found.</returns>
        public bool Remove(string key)
        {
            key = key?.Trim();
            if (key == null)
            {
                if (DefaultHandler is null) return false;
                DefaultHandler = null;
                return true;
            }

            var flag = false;
            foreach (var item in list)
            {
                if (item.RemoveKey(key))
                    flag = true;
            }

            list.RemoveAll(ele => ele.IsEmpty);
            return flag;
        }

        /// <summary>
        /// Removes a route by key from this dispatcher. 
        /// </summary>
        /// <param name="keys">The keys to register.</param>
        /// <returns>true if remove succeeded; otherwise, false. This method also returns false if not found.</returns>
        public bool Remove(IEnumerable<string> keys)
        {
            if (keys == null) return false;
            var flag = false;
            keys = keys.Select(k => k?.Trim()).Where(k => !string.IsNullOrEmpty(k)).Distinct().ToList();
            foreach (var item in list)
            {
                foreach (var key in keys)
                {
                    if (item.RemoveKey(key))
                        flag = true;
                }
            }

            list.RemoveAll(ele => ele.IsEmpty);
            return flag;
        }

        /// <summary>
        /// Removes a route by key from this dispatcher. 
        /// </summary>
        /// <param name="test">The key test handler.</param>
        /// <returns>true if remove succeeded; otherwise, false. This method also returns false if not found.</returns>
        public bool Remove(Func<string, bool> test)
        {
            if (test is null) return false;
            return list.RemoveAll(ele => ele.EqualsTestHandler(test) || ele.IsEmpty) > 0;
        }

        /// <summary>
        /// Removes a specifc command handler.
        /// </summary>
        /// <param name="handler">A handler to remove.</param>
        /// <returns>The number of elements removed from the command handlers registry.</returns>
        public int RemoveAll(ICommandHandler handler)
        {
            if (handler is null) return 0;
            return list.RemoveAll(ele => ele.Handler == handler);
        }

        /// <summary>
        /// Gets the command handler.
        /// </summary>
        /// <param name="verb">The verb.</param>
        /// <returns>A command handler matched.</returns>
        public ICommandHandler Get(string verb)
        {
            verb = verb?.Trim();
            return string.IsNullOrEmpty(verb) ? DefaultHandler : GetVerb(verb)?.Handler;
        }

        /// <summary>
        /// Processes.
        /// </summary>
        /// <param name="args">The command arguments.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task ProcessAsync(string args, CancellationToken cancellationToken = default)
            => ProcessAsync(new CommandArguments(args), null, cancellationToken);

        /// <summary>
        /// Processes.
        /// </summary>
        /// <param name="args">The command arguments.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task ProcessAsync(string[] args, CancellationToken cancellationToken = default)
            => ProcessAsync(new CommandArguments(args), null, cancellationToken);

        /// <summary>
        /// Processes.
        /// </summary>
        /// <param name="args">The command arguments.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public Task ProcessAsync(CommandArguments args, CancellationToken cancellationToken = default)
            => ProcessAsync(args, null, cancellationToken);

        /// <summary>
        /// Processes.
        /// </summary>
        /// <param name="args">The command arguments.</param>
        /// <param name="conversationMode">The conversation mode.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        /// <returns>true if process succeeded; otherwise, false.</returns>
        public async Task ProcessAsync(CommandArguments args, CommandConversationModes? conversationMode, CancellationToken cancellationToken = default)
        {
            var verb = args?.Verb?.ToString()?.Trim();
            await ProcessHandlerAsync(PreloadHandler, args, cancellationToken);
            var loop = string.IsNullOrEmpty(verb);
            if (loop)
                await ProcessHandlerAsync(DefaultHandler, args, cancellationToken);
            else
                await ProcessHandlerAsync(GetVerb(verb)?.Handler, args, cancellationToken);

            if (conversationMode == null)
            {
                if (ConversationMode == CommandConversationModes.Off)
                    loop = false;
                else if (ConversationMode == CommandConversationModes.On)
                    loop = true;
            }
            else
            {
                if (conversationMode == CommandConversationModes.Off)
                    loop = false;
                else if (conversationMode == CommandConversationModes.On)
                    loop = true;
            }

            while (loop)
            {
                if (conversationMode == null && ConversationMode == CommandConversationModes.Off) break;
                try
                {
                    if (Console.CursorLeft > 0) Console.WriteLine();
                }
                catch (System.Security.SecurityException)
                {
                }
                catch (System.IO.IOException)
                {
                }
                catch (System.Runtime.InteropServices.COMException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                catch (NotSupportedException)
                {
                }
                catch (NotImplementedException)
                {
                }
                catch (NullReferenceException)
                {
                }

                Console.Write(InputPrompt ?? "> ");
                string str = null;
                while (string.IsNullOrEmpty(str))
                {
                    try
                    {
                        str = Console.ReadLine()?.Trim();
                    }
                    catch (ArgumentException)
                    {
                        break;
                    }
                    catch (InvalidOperationException)
                    {
                        break;
                    }
                }

                if (string.IsNullOrEmpty(str)) break;
                args = new CommandArguments(str);
                verb = args?.Verb?.ToString()?.Trim();
                var item = GetVerb(verb);
                if (item?.Handler == null)
                {
                    var unhandled = UnhandledHandler;
                    if (unhandled != null)
                        await ProcessHandlerAsync(unhandled, args, cancellationToken);
                }
                else
                {
                    await ProcessHandlerAsync(item.Handler, args, cancellationToken);
                }

                if (IsExitKey(verb)) break;
            }

            await ProcessHandlerAsync(ExitHandler, args, cancellationToken);
            return;
        }

        /// <summary>
        /// Test if a verb is the exit key.
        /// </summary>
        /// <param name="verb">The verb.</param>
        /// <returns>true if it can be used to exit; otherwise, false.</returns>
        public bool IsExitKey(string verb)
            => string.IsNullOrEmpty(verb)
                    ? false
                    : ExitKeys.Any(k => verb.Equals(k, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Gets static keys.
        /// </summary>
        public IEnumerable<string> GetStaticKeys() => list
            .Where(ele => ele?.IsEmpty == false && !ele.Handler.IsDisabled && ele.KeyCount > 0)
            .Select(ele => ele.Keys.FirstOrDefault()?.Trim())
            .Where(ele => !string.IsNullOrEmpty(ele))
            .Distinct();

        /// <summary>
        /// Gets help.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void GetHelp(string args)
        {
            args = args.Trim();
            var firstKeyLen = args.IndexOf(" ");
            if (firstKeyLen > 0)
            {
                var firstKey = args.Substring(0, firstKeyLen);
                if (firstKey.StartsWith("--")) firstKey = firstKey.Substring(2);
                else if (firstKey.StartsWith("-") || firstKey.StartsWith("/")) firstKey = firstKey.Substring(1);
                if (helpKeys.Any(ele => args.Equals(ele, StringComparison.OrdinalIgnoreCase)))
                    args = args.Substring(firstKeyLen).Trim();
            }

            GetHelp(args, false);
        }

        /// <summary>
        /// Gets help.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="removeFirstKey">true if need remove the first key; otherwise, false.</param>
        public void GetHelp(string args, bool removeFirstKey)
        {
            args = args.Trim();
            if (removeFirstKey)
            {
                var firstKeyLen = args.IndexOf(" ");
                args = firstKeyLen > 0 ? args.Substring(firstKeyLen) : null;
            }

            var a = new CommandArguments(args);
            if (!a.HasVerb)
            {
                GetHelp(DefaultHandler ?? UnhandledHandler, a);
                return;
            }

            var verb = GetVerb(a.Verb.ToString());
            if (verb is null)
                verb = GetVerb(a.Verb.FirstOrDefault());
            GetHelp(verb?.Handler ?? (IsExitKey(a.Verb.ToString()) ? ExitHandler : UnhandledHandler), a);
        }

        /// <summary>
        /// Returns an enumerator about the route that iterates through this dispatcher.
        /// </summary>
        /// <returns>An enumerator object that can be used to iterate through this dispatcher.</returns>
        public IEnumerator<Route> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator about the route that iterates through this dispatcher.
        /// </summary>
        /// <returns>An enumerator object that can be used to iterate through this dispatcher.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        private async Task ProcessHandlerAsync(ICommandHandler handler, CommandArguments args, CancellationToken cancellationToken)
        {
            if (handler is null) return;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                Processing?.Invoke(this, new ProcessEventArgs(args, handler));
                await handler.ProcessAsync(args, cancellationToken);
                Processed?.Invoke(this, new ProcessEventArgs(args, handler));
            }
            catch (Exception ex)
            {
                var isCanceled = false;
                try
                {
                    isCanceled = cancellationToken.IsCancellationRequested;
                }
                catch(InvalidOperationException)
                {
                }

                if (isCanceled)
                    ProcessCanceled?.Invoke(this, new ProcessEventArgs(args, handler, ex));
                else
                    ProcessFailed?.Invoke(this, new ProcessEventArgs(args, handler, ex));
                throw;
            }
        }

        private void GetHelp(ICommandHandler handler, CommandArguments args)
        {
            if (handler is null) return;
            handler.GetHelp(args);
            GotHelp?.Invoke(this, new ProcessEventArgs(args, handler));
        }

        private Route GetVerb(string verb)
            => string.IsNullOrEmpty(verb) ? null : list.FindLast(ele => ele.IsMatch(verb));
    }
}
