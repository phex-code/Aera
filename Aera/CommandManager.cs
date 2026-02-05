using System;
using System.Collections.Generic;
using System.Linq;

namespace Aera
{
    /// <summary>
    /// Central dispatcher and registry for all CLI commands.
    /// Handles command lookup, aliases, piping, sudo execution,
    /// and help/man output.
    /// </summary>
    internal class CommandManager
    {
        // Fast lookup by command name or alias
        private readonly Dictionary<string, ICommand> commands = new();

        // Preserves registration order for help output
        private readonly List<ICommand> orderedCommands = new();

        // ───────────────────── Registration ─────────────────────

        /// <summary>
        /// Registers a command and all of its aliases.
        /// </summary>
        public void Register(ICommand command)
        {
            // Register primary name
            commands[command.Name.ToLower()] = command;

            // Register aliases
            foreach (var alias in command.Aliases)
                commands[alias.ToLower()] = command;

            // Preserve order for help/man
            orderedCommands.Add(command);
        }

        // ───────────────────── Execution Entry ─────────────────────

        /// <summary>
        /// Entry point for executing user input.
        /// Handles pipe splitting and dispatch.
        /// </summary>
        public void Execute(string input, _s tool)
        {
            // Split on first pipe only
            var parts = input.Split('|', 2, StringSplitOptions.TrimEntries);

            // No pipe → normal execution
            if (parts.Length == 1)
            {
                ExecuteSingle(parts[0], tool);
                return;
            }

            // Pipe detected
            string left = parts[0];
            string right = parts[1];

            string pipeOutput = CaptureOutput(left, tool);
            ExecutePiped(right, pipeOutput, tool);
        }

        // ───────────────────── Single Command Execution ─────────────────────

        /// <summary>
        /// Executes a single command without piping.
        /// </summary>
        private void ExecuteSingle(string input, _s tool)
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return;

            string name = parts[0].ToLower();
            string[] args = parts.Skip(1).ToArray();

            if (!commands.TryGetValue(name, out var cmd))
            {
                tool.cwl($"Command '{name}' not recognized.");
                return;
            }

            // Automatic per-command --help
            if (args.Contains("--help"))
            {
                ShowCommandHelp(cmd, tool);
                return;
            }

            cmd.Execute(args, tool);
        }

        // ───────────────────── Piping Support ─────────────────────

        /// <summary>
        /// Executes a command in capture mode and returns its output.
        /// </summary>
        private string CaptureOutput(string command, _s tool)
        {
            tool.CaptureMode = true;
            ExecuteSingle(command, tool);
            tool.CaptureMode = false;

            return tool.FlushPipe();
        }

        /// <summary>
        /// Executes a command with piped input.
        /// Enforces pipe acceptance and destructive safety.
        /// </summary>
        private void ExecutePiped(string command, string input, _s tool)
        {
            var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return;

            string name = parts[0].ToLower();
            string[] args = parts.Skip(1).ToArray();

            if (!commands.TryGetValue(name, out var cmd))
            {
                tool.cwl($"Command '{name}' not recognized.");
                return;
            }

            if (!cmd.AcceptsPipeInput)
            {
                tool.cwlc($"Command '{name}' does not accept piped input.", "Red");
                return;
            }

            if (cmd.IsDestructive && !tool.IsSudo)
            {
                tool.cwlc($"'{name}' is destructive and requires sudo when piped.", "Red");
                return;
            }

            cmd.ExecutePipe(input, args, tool);
        }

        // ───────────────────── Sudo Execution ─────────────────────

        /// <summary>
        /// Executes a command after sudo authentication.
        /// </summary>
        public void ExecuteSudo(string name, string[] args, _s tool)
        {
            name = name.ToLower();

            if (!commands.TryGetValue(name, out var command))
            {
                tool.cwl($"Command '{name}' not recognized.");
                return;
            }

            command.Execute(args, tool);
        }

        // ───────────────────── Help / Man Output ─────────────────────

        /// <summary>
        /// Displays detailed help for a single command.
        /// </summary>
        public void ShowCommandHelp(ICommand cmd, _s tool)
        {
            tool.cwl($"Command: {cmd.Name}");
            tool.cwl($"Description: {cmd.Description}");

            if (cmd.Aliases.Length > 0)
                tool.cwl($"Aliases: {string.Join(", ", cmd.Aliases)}");

            tool.cwl("");
            tool.cwl($"Accepts pipe input: {(cmd.AcceptsPipeInput ? "yes" : "no")}");
            tool.cwl($"Destructive: {(cmd.IsDestructive ? "yes" : "no")}");

            tool.cwl("");
            tool.cwl("  " + cmd.Usage + "\n");
        }

        /// <summary>
        /// Attempts to resolve a command by name or alias.
        /// </summary>
        public bool TryGet(string name, out ICommand cmd)
        {
            return commands.TryGetValue(name, out cmd);
        }

        /// <summary>
        /// Compact help listing (used by `help`).
        /// </summary>
        public void ShowHelp(_s tool)
        {
            tool.cwl("Aera Command Manual:");

            var uniqueCommands = orderedCommands
                .GroupBy(c => c.Name)
                .Select(g => g.First());

            const int nameWidth = 8;
            const int aliasWidth = 26;

            foreach (var cmd in uniqueCommands)
            {
                string aliases = cmd.Aliases.Length > 0
                    ? $"({string.Join(", ", cmd.Aliases)})"
                    : "";

                tool.cwl(
                    $"  {cmd.Name,-nameWidth} {aliases,-aliasWidth} {cmd.Description} {cmd.Usage}"
                );
            }

            tool.cwl("");
            tool.cwl("Use `man <command>` for detailed help.");
        }

        /// <summary>
        /// Verbose help listing (legacy / debug view).
        /// </summary>
        public void ShowAll(_s tool)
        {
            tool.cwl("Aera Command Manual:");

            foreach (var cmd in orderedCommands)
            {
                string aliasText = cmd.Aliases.Length > 0
                    ? $" ({string.Join(", ", cmd.Aliases)})"
                    : "";

                string danger = cmd.IsDestructive ? " !" : "";

                tool.cwl($"  {cmd.Name,-12} {cmd.Description}{aliasText}{danger}");
                tool.cwl("  " + cmd.Usage + "\n");
            }
        }
    }
}
