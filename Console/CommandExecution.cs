using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trivial.Console
{
    /// <summary>
    /// The event arguments for writing a line to the standard output stream.
    /// </summary>
    public class LineWritenEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the LineWritenEventArgs class.
        /// </summary>
        /// <param name="line">The output line.</param>
        public LineWritenEventArgs(string line) : base()
        {
            Result = line;
        }

        /// <summary>
        /// Gets the string to output.
        /// </summary>
        public string Result { get; }
    }

    /// <summary>
    /// The command execution instance.
    /// </summary>
    /// <remarks>
    /// Only for Windows operation system.
    /// </remarks>
    public class CommandExecution
    {
        /// <summary>
        /// A value indicating whether it has loaded.
        /// </summary>
        private bool done = false;

        /// <summary>
        /// Initializes a new instance of the CommandExecution.
        /// </summary>
        /// <param name="cmd">The command to process.</param>
        public CommandExecution(string cmd) => CommandLine = cmd;

        /// <summary>
        /// Adds or removes the handler to catch the line output.
        /// </summary>
        public event EventHandler<LineWritenEventArgs> WrotenLine;

        /// <summary>
        /// Gets the command to process.
        /// </summary>
        public string CommandLine { get; }

        /// <summary>
        /// Gets or sets a value indicating whether write to standard output stream immediately.
        /// </summary>
        public bool AutoFlush { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether the task is cancelled.
        /// </summary>
        public bool IsCancellationRequested { get; private set; }

        /// <summary>
        /// Processes a specific command line.
        /// </summary>
        /// <returns>The async processing task.</returns>
        public async Task ProcessAsync()
        {
            if (done) return;
            done = true;

            ThrowIfCancellationRequested();
            using var p = new Process();
            void closeProcess() => p.Close();
            p.StartInfo.FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "bash";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            ThrowIfCancellationRequested(closeProcess);

            var cmd = CommandLine;
            var lastCharIndex = cmd.Length - 1;
            if (cmd.LastIndexOf("&") == lastCharIndex) cmd = cmd.Substring(0, lastCharIndex);
            p.StandardInput.WriteLine(cmd + "&exit");
            p.StandardInput.AutoFlush = true;
            while (!p.HasExited)
            {
                ThrowIfCancellationRequested(closeProcess);
                var line = await p.StandardOutput.ReadLineAsync();
                if (AutoFlush) System.Console.WriteLine(line);
                WrotenLine?.Invoke(this, new LineWritenEventArgs(line));
            }

            p.Close();
        }

        /// <summary>
        /// Processes a specific command line.
        /// </summary>
        /// <returns>The async processing task.</returns>
        public void Process()
        {
            ProcessAsync().Wait();
        }

        /// <summary>
        /// Communicates a request for cancellation.
        /// </summary>
        public void Cancel()
        {
            IsCancellationRequested = true;
        }

        /// <summary>
        /// Throws an operation canceled exception if this token has had cancellation requested.
        /// </summary>
        /// <param name="callback">A callback.</param>
        private void ThrowIfCancellationRequested(Action callback = null)
        {
            if (!IsCancellationRequested) return;
            callback?.Invoke();
            throw new OperationCanceledException();
        }

        /// <summary>
        /// Processes a specific command line.
        /// </summary>
        /// <returns>The output string.</returns>
        public static async Task<string> Process(string cmd, CancellationToken? cancellationToken = null)
        {
            var cancel = cancellationToken ?? CancellationToken.None;

            var p = new Process();
            p.StartInfo.FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "cmd.exe" : "bash";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            cancel.ThrowIfCancellationRequested();
            p.Start();

            cancel.Register(() => p.Close());
            var lastCharIndex = cmd.Length - 1;
            if (cmd.LastIndexOf("&") == lastCharIndex) cmd = cmd.Substring(0, lastCharIndex);
            p.StandardInput.WriteLine(cmd + "&exit");
            p.StandardInput.AutoFlush = true;

            var str = new StringBuilder();
            while (!p.HasExited)
            {
                cancel.ThrowIfCancellationRequested();
                var line = await p.StandardOutput.ReadLineAsync();
                System.Console.WriteLine(line);
                str.AppendLine(line);
            }

            p.Close();
            return str.ToString();
        }

        /// <summary>
        /// Open a directory in file system.
        /// Only for Windows NT OS.
        /// </summary>
        /// <param name="dir">The directory path.</param>
        /// <returns>true if a process resource is started; false if no new process resource is started.</returns>
        public static bool Directory(string dir)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;
            using var p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = dir;
            return p.Start();
        }

        /// <summary>
        /// Open a directory in file system.
        /// Only for Windows NT OS.
        /// </summary>
        /// <param name="dir">The directory information instance.</param>
        /// <returns>true if a process resource is started; false if no new process resource is started.</returns>
        public static bool Directory(DirectoryInfo dir)
        {
            return Directory(dir.ToString());
        }

        /// <summary>
        /// Select a file or directory in file browser.
        /// Only for Windows NT OS.
        /// </summary>
        /// <param name="path">The file or directory path.</param>
        /// <returns>true if a process resource is started; false if no new process resource is started.</returns>
        public static bool DirectorySelect(string path)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return false;
            using var p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "explorer.exe";
            p.StartInfo.Arguments = "/select," + path;
            return p.Start();
        }

        /// <summary>
        /// Open a file or directory in file browser.
        /// Only for Windows NT OS.
        /// </summary>
        /// <param name="path">The file or directory information instance.</param>
        /// <returns>true if a process resource is started; false if no new process resource is started.</returns>
        public static bool DirectorySelect(FileSystemInfo path)
        {
            return DirectorySelect(path.ToString());
        }
    }
}
